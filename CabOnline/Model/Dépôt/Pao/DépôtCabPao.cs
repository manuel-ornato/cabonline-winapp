using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CabOnline.Extensions.Exception;
using CabOnline.Model.Cab;
using CabOnline.Model.Parution;
using CabOnline.Operators.Tiff;
using CabOnline.TaskObserver;

namespace CabOnline.Model.Dépôt.Pao
{
    internal class DépôtCabPao : DépôtCab, IDépôtCabPao
    {
        public DépôtCabPao(string racinePao, ICabFactory cabFactory,
                           IDépôtParutions dépôtParutions, IConvertisseurEpsVersTiff convertisseurEpsVersTiff)
            : base(cabFactory, dépôtParutions)
        {
            if (String.IsNullOrWhiteSpace(racinePao)) throw new ArgumentNullException(nameof(racinePao));

            _racinePao = racinePao;
            _convertisseurEpsVersTiff = convertisseurEpsVersTiff ?? throw new ArgumentNullException(nameof(convertisseurEpsVersTiff));
        }

        public override string LibelléCourt => "01 En cours";

        public override void ChargerCabs(ITaskObserver observer)
        {
            try
            {
                ChargerDossiersPao(observer);
            }
            catch (Exception ex)
            {
                throw new Exception($"Problème durant la lecture des dossiers de [{LibelléCourt}]", ex);
            }
        }

        public override Stream ObtenirContenu(ICab cab)
        {
            return new FileStream(cab.Url, FileMode.Open);
        }

        public override ICab Cab(IParution parution)
        {
            lock (_listeDonnéesCab)
                return base.Cab(parution);
        }

        public virtual ICab Déposer(ICab cab, bool créerFichierTiff, ITaskObserver observer)
        {
            if (cab == null) throw new ArgumentNullException(nameof(cab));
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            observer.NotifyInfo(this,
                                $"Début du téléchargement du Cab [{cab}] pour [{cab.ParutionCible}] depuis [{cab.Dépôt}] vers [{this}]");

            if (cab.Dépôt == this)
            {
                observer.NotifyWarning(this,
                                       $"Tentative de téléchargement d'un Cab vers lui-même : [{cab.Url}], opération ignorée");
                return cab;
            }

            string cheminDossierCab = CheminDossierCab(cab);
            if (String.IsNullOrWhiteSpace(cheminDossierCab))
                throw new Exception(
                    $"Impossible de trouver le dossier [{cheminDossierCab}] pour y déposer le Cab [{cab}]");

            string nomFichierCab = NomFichierCab(cab);
            string cheminFichierCab = Path.Combine(cheminDossierCab, nomFichierCab);

            string nomFichierTiff = Path.GetFileNameWithoutExtension(nomFichierCab) + ".tif";
            string cheminFichierTiff = Path.Combine(cheminDossierCab, nomFichierTiff);

            EffacerFichiersCab(cab, cheminDossierCab, observer);

            TéléchargerCab(cab, cheminFichierCab, observer);

            Task conversionTask;
            if (créerFichierTiff)
                conversionTask = ConvertirVersTiffAsync(cheminFichierCab, cheminFichierTiff, observer);
            else
                conversionTask = null;


            var newCab = IntégrerCabTéléchargé(cab, cheminFichierCab, observer);
            if (conversionTask != null)
                try
                {
                    conversionTask.Wait();
                }
                catch (Exception exception)
                {
                    observer.NotifyWarning(this,
                                           $"Echec de la conversion en Tiff de [{cheminFichierCab}] vers [{cheminFichierTiff}] : {exception.DetailedMessage()}");
                }

            observer.NotifyInfo(this,
                                $"Fin du téléchargement du Cab [{cab}] pour [{cab.ParutionCible}] depuis [{cab.Dépôt}] vers [{this}]");
            return newCab;
        }

        protected override IEnumerable<ICab> DonnéesCab()
        {
            return _listeDonnéesCab;
        }

        private const string _NomDossierCab = "001_CAB";
        private readonly IConvertisseurEpsVersTiff _convertisseurEpsVersTiff;
        private readonly string _racinePao;

        private readonly Regex _regexDossierProduit =
            new Regex(@"^(?<codeInterne>[A-Z0-9]{4,5})[-_\s]?(?<num>\d{2,3}).*?(?<num_fin>\d{2,3})?$",
                      RegexOptions.IgnoreCase);

        private ILookup<IParution, string> _dossiersParutions;
        private List<ICab> _listeDonnéesCab;

        private Tuple<string, int> AnalyseNomDossier(string cheminDossier)
        {
            string nomDossier = Path.GetFileName(cheminDossier);
            if (nomDossier == null)
                return null;

            var m = _regexDossierProduit.Match(nomDossier);
            if (!m.Success)
                return null;

            string codeInterne = ExtraireCodeInterne(m);
            if (String.IsNullOrWhiteSpace(codeInterne))
                return null;

            var numéro = ExtraireNuméro(m);
            if (!numéro.HasValue)
                return null;

            return new Tuple<string, int>(codeInterne, numéro.Value);
        }

        private IParution Parution(string cheminDossier)
        {
            var infosDossier = AnalyseNomDossier(cheminDossier);
            if (infosDossier == null)
                return null;

            return (from p in DépôtParutions.Parutions
                    where p.Correspond(infosDossier.Item1, infosDossier.Item2)
                    select p).FirstOrDefault();
        }

        private void ChargerDossiersPao(ITaskObserver observer)
        {
            observer.NotifyIsIndeterminate(this);

            var dossiers = Directory.GetDirectories(_racinePao, "*.*",
                                                         SearchOption.TopDirectoryOnly);

            observer.NotifyProgress(this, 0);

            var dossiersParutions =
                (from dossier in observer.InSlice(0, 10).EnumerableProgressTracker(this, dossiers)
                 let parution = Parution(dossier)
                 where parution != null
                 select new {Parution = parution, Dossier = dossier}).ToList();


            _dossiersParutions = dossiersParutions.ToLookup(dp => dp.Parution, dp => dp.Dossier);

            var cabs =
                from dossierParution in observer.InSlice(15, 100).EnumerableProgressTracker(this, dossiersParutions)
                let fi = ExtraireCabFileInfo(dossierParution.Dossier, dossierParution.Parution)
                where fi != null
                let cab = CréerCab(fi, dossierParution.Parution)
                where cab != null
                select cab;

            _listeDonnéesCab = new List<ICab>(cabs);

            observer.NotifyProgress(this, 100);
        }

        private static string ExtraireCodeInterne(Match matchDossierParution)
        {
            return matchDossierParution.Groups["codeInterne"].Value;
        }

        private static int? ExtraireNuméro(Match matchDossierParution)
        {
            int numéro;
            if (int.TryParse(
                matchDossierParution.Groups["num"] != null
                    ? matchDossierParution.Groups["num"].Value
                    : (matchDossierParution.Groups["num_fin"] != null
                           ? matchDossierParution.Groups["num_fin"].Value
                           : "0"), out numéro))
                return numéro;
            return null;
        }

        private static FileInfo ExtraireCabFileInfo(string dossier, IParution parution)
        {
            try
            {
                string cheminCab = Path.Combine(dossier, _NomDossierCab);
                if (!Directory.Exists(cheminCab))
                    return null;

                var di = new DirectoryInfo(cheminCab);
                var infoFichier = (from fileInfo in di.GetFiles("*.eps", SearchOption.TopDirectoryOnly)
                                   where fileInfo.Name.StartsWith(parution.CodeProduit)
                                   orderby fileInfo.CreationTime descending
                                   select fileInfo).FirstOrDefault();

                return infoFichier;
            }
            catch (Exception)
            {
                // TODO: tracer l'exception
                return null;
            }
        }


        private ICab CréerCab(FileInfo fiCab, IParution parutionCible)
        {
            try
            {
                string sDonnées = Path.GetFileNameWithoutExtension(fiCab.Name);
                if (sDonnées == null)
                    return null;

                if (sDonnées[4] == '_')
                    // Code de type Bdi migré
                    sDonnées = $"{sDonnées.Substring(0, 4)}#{sDonnées.Substring(5)}";

                var données = sDonnées.Replace('-', '_').Split('_');
                if (données.Length < 6)
                    return null;

                string codeProduit = données[0];
                if (codeProduit[4] == '#')
                    codeProduit = $"{codeProduit.Substring(0, 4)}_{codeProduit.Substring(5)}";
                string sDistributeur = données[1];
                string codif = données[2];
                string sPrix = données[3].Replace('.', ',');
                string sPériodicité = données[4];
                string sQualification = données[5];
                decimal prix = decimal.Parse(sPrix);
                string sNuméro;
                if (codeProduit.Length == 6)
                    sNuméro = codeProduit.Substring(4);
                else
                    sNuméro = codeProduit.Substring(5);
                int numéro = int.Parse(sNuméro);

                var distributeur = DistributeurPaoVersTypeDistributeur(sDistributeur);
                var périodicité = PériodicitéPaoVersTypePériodicité(sPériodicité);
                var qualificationRéseau = QualificationPaoVersTypeQualificationRéseau(sQualification);

                return CabFactory.CréerCab(
                    this,
                    fiCab.FullName,
                    TypeEditeur.Inconnu,
                    distributeur,
                    codif,
                    numéro,
                    prix,
                    périodicité,
                    qualificationRéseau,
                    fiCab.CreationTime,
                    parutionCible);
            }
            catch (Exception)
            {
                // TOTO: tracer l'exception
                return null;
            }
        }

        private static TypeQualificationRéseau QualificationPaoVersTypeQualificationRéseau(string sQualification)
        {
            switch (sQualification)
            {
                case "AU":
                    return TypeQualificationRéseau.Aucun;
                case "RD":
                    return TypeQualificationRéseau.Rd;
                case "PM":
                    return TypeQualificationRéseau.Pm;
                case "PZ":
                    return TypeQualificationRéseau.Pz;
                case "EY":
                    return TypeQualificationRéseau.Ey;
                case "AL":
                    return TypeQualificationRéseau.Al;
                case "PP":
                    return TypeQualificationRéseau.Pp;
                case "IN":
                default:
                    return TypeQualificationRéseau.Inconnu;
            }
        }

        private static string TypeQualificationRéseauVersQualificationPao(TypeQualificationRéseau qualification)
        {
            return qualification.ToString().Substring(0, 2).ToUpper();
        }

        private static TypePériodicité PériodicitéPaoVersTypePériodicité(string sPériodicité)
        {
            switch (sPériodicité)
            {
                case "M":
                    return TypePériodicité.Mensuelle;
                case "B":
                    return TypePériodicité.Bimestrielle;
                case "T":
                    return TypePériodicité.Trimestrielle;
                case "I":
                    return TypePériodicité.Inconnue;
                case "X":
                    return TypePériodicité.Irrégulière;
                default:
                    throw new ArgumentException("Périodicité inconnue", nameof(sPériodicité));
            }
        }

        private static string TypePériodicitéVersPériodicitéPao(TypePériodicité périodicité)
        {
            switch (périodicité)
            {
                case TypePériodicité.Bimestrielle:
                case TypePériodicité.BiPuisTri:
                    return "B";
                case TypePériodicité.Trimestrielle:
                    return "T";
                case TypePériodicité.Mensuelle:
                    return "M";
                case TypePériodicité.Irrégulière:
                    return "X";
                case TypePériodicité.Inconnue:
                default:
                    return "I";
            }
        }

        private static TypeDistributeur DistributeurPaoVersTypeDistributeur(string sDistributeur)
        {
            TypeDistributeur distributeur;
            if (sDistributeur == "M")
                distributeur = TypeDistributeur.Mlp;
            else if (sDistributeur == "N")
                distributeur = TypeDistributeur.Nmpp;
            else if (sDistributeur == "T")
                distributeur = TypeDistributeur.Tp;
            else
                distributeur = TypeDistributeur.Inconnu;
            return distributeur;
        }

        private static string TypeDistributeurVersDistributeurPao(TypeDistributeur distributeur)
        {
            switch (distributeur)
            {
                case TypeDistributeur.Mlp:
                    return "M";
                case TypeDistributeur.Nmpp:
                    return "N";
                case TypeDistributeur.Tp:
                    return "T";
                default:
                    return "X";
            }
        }

        private ICab IntégrerCabTéléchargé(ICab cab, string cheminFichierCab, ITaskObserver observer)
        {
            observer.NotifyInfo(this, $"Vérification et intégration du nouveau Cab téléchargé [{cheminFichierCab}]");
            var newCab = IntégrerCabTéléchargé(cab, cheminFichierCab);
            observer.NotifyInfo(this, $"Nouveau Cab téléchargé [{cheminFichierCab}] vérifié et intégré : [{newCab}]");
            return newCab;
        }

        private ICab IntégrerCabTéléchargé(ICab cab, string cheminFichierCab)
        {
            var newCab = CréerCab(new FileInfo(cheminFichierCab), cab.ParutionCible);
            if (newCab == null)
                throw new Exception(
                    $"Le fichier du Cab téléchargé [{cheminFichierCab}] est introuvable ou son nom n'est pas dans le bon format");
            lock (_listeDonnéesCab)
            {
                var oldCab =
                    (from c in _listeDonnéesCab where c.ParutionCible == cab.ParutionCible select c).
                        FirstOrDefault();
                if (oldCab != null)
                    _listeDonnéesCab.Remove(oldCab);
                _listeDonnéesCab.Add(newCab);
            }
            return newCab;
        }

        private Task ConvertirVersTiffAsync(string cheminFichierCab, string cheminFichierTiff, ITaskObserver observer)
        {
            if (_convertisseurEpsVersTiff != null)
            {
                return Task.Factory.StartNew(
                    () => ConvertirVersTiff(cheminFichierCab, cheminFichierTiff, observer),
                    TaskCreationOptions.AttachedToParent
                    );
            }
            else
                return null;
        }

        private void ConvertirVersTiff(string cheminFichierCab, string cheminFichierTiff, ITaskObserver observer)
        {
            observer.NotifyInfo(this, $"Conversion de [{cheminFichierCab}] en Tiff");
            ConvertirVersTiff(cheminFichierCab, cheminFichierTiff);
            observer.NotifyInfo(this, $"[{cheminFichierCab}] a été converti en Tiff dans [{cheminFichierTiff}]");
        }

        private void ConvertirVersTiff(string cheminFichierCab, string cheminFichierTiff)
        {
            _convertisseurEpsVersTiff.Convertir(cheminFichierCab, cheminFichierTiff);
        }

        private void TéléchargerCab(ICab cab, string cheminFichierCab, ITaskObserver observer)
        {
            observer.NotifyInfo(this,
                                $"Transfert du Cab depuis [{cab.Url}] vers [{cheminFichierCab}]");
            TéléchargerCab(cab, cheminFichierCab);
            observer.NotifyInfo(this, $"Cab [{cab.Url}] récupéré dans [{cheminFichierCab}]");
        }

        private static void TéléchargerCab(ICab cab, string cheminFichierCab)
        {
            using (var input = cab.ObtenirContenu())
            {
                using (var output = new FileStream(cheminFichierCab, FileMode.Create))
                {
                    input.CopyTo(output);
                }
            }
            File.SetCreationTime(cheminFichierCab, cab.DateCréation);
        }

        private void EffacerFichiersCab(ICab cab, string cheminDossierCab, ITaskObserver observer)
        {
            observer.NotifyInfo(this, $"Effacement des anciens fichiers Cab et Tiff pour [{cab.ParutionCible}]");
            EffacerFichiersCabs(cheminDossierCab);
            observer.NotifyInfo(this, $"Anciens fichiers Cab et Tiff pour [{cab.ParutionCible}] effacés");
        }

        private static void EffacerFichiersCabs(string cheminDossierCab)
        {
            Helpers.Helpers.EffacerContenuDossier(cheminDossierCab, "*.eps");
            Helpers.Helpers.EffacerContenuDossier(cheminDossierCab, "*.tif");
        }

        private static string NomFichierCab(ICab cab)
        {
            return
                $"{cab.ParutionCible.CodeProduit}_{TypeDistributeurVersDistributeurPao(cab.Distributeur.TypeDistributeur)}_{cab.Codif}_{cab.Prix.ToString().Replace(",", ".")}_{TypePériodicitéVersPériodicitéPao(cab.Périodicité.TypePériodicité)}_{TypeQualificationRéseauVersQualificationPao(cab.Qualif.TypeQualificationRéseau)}.eps";
        }

        protected virtual string CheminDossierCab(ICab cab)
        {
            if (!_dossiersParutions.Contains(cab.ParutionCible))
                throw new Exception(
                    $"Impossible de trouver le dossier de Magazines en cours pour [{cab.ParutionCible}] afin d'y déposer le Cab [{cab}]");
            var cheminDossierCab =
                (from chemin in _dossiersParutions[cab.ParutionCible]
                 let cheminCab = Path.Combine(chemin, _NomDossierCab)
                 where !String.IsNullOrWhiteSpace(cheminCab) && Directory.Exists(cheminCab)
                 select cheminCab).FirstOrDefault();

            if (cheminDossierCab == null)
                throw new Exception(
                    $"Impossible de trouver le dossier [{_NomDossierCab}] pour [{cab.ParutionCible}] afin d'y déposer le Cab [{cab}]");
            return cheminDossierCab;
        }
    }
}