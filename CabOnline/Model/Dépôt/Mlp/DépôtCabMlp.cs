using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using CabOnline.Model.Cab;
using CabOnline.Model.Parution;
using CabOnline.TaskObserver;

namespace CabOnline.Model.Dépôt.Mlp
{
    internal class DépôtCabMlp : DépôtCabOnline, IDépôtCabMlp
    {
        public Configuration Configuration { get; }
        //protected internal const string MlpCabsUrl = "http://cabs.mlp.fr";
        //protected internal const string MlpCabsRootFolder = "/RecupCabs/";
        //protected internal const string MlpCabPassword = "ACQ2WJA";

        public DépôtCabMlp(ICabFactory cabFactory, IDépôtParutions dépôtParutions, Configuration configuration)
            : base(cabFactory, dépôtParutions)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override string LibelléCourt => "Site MLP";

        public override void ChargerCabs(ITaskObserver observer)
        {
            try
            {
                InitialiserLogins();
                ChargerListeDonnéesCab(observer);
            }
            catch (Exception ex)
            {
                throw new Exception("Problème durant la recherche des CAB sur le site MLP", ex);
            }
        }

        public override Stream ObtenirContenu(ICab cab)
        {
            try
            {
                var compte = Configuration.Comptes.FirstOrDefault(c => c.Editeur == cab.Editeur.TypeEditeur)
                              ?? throw new ArgumentException($"Type d'éditeur inconnu pour le téléchargement du CAB de [{cab.ParutionCible.LibelléCourt}] depuis le site MLP", nameof(cab));

                Stream s;
                var request = RequêteUrl(compte, cab.Url);
                try
                {
                    s = OuvrirFlux(request);
                    return s;
                }
                catch
                {
                    // On donne une seconde chance en cas de souci réseau:
                    Thread.Sleep(5000);

                    request = RequêteUrl(compte, cab.Url);
                    s = OuvrirFlux(request);
                    return s;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Problème durant le téléchargement du CAB de [{cab.ParutionCible.LibelléCourt}] depuis le site MLP", ex);
            }
        }

        protected override IEnumerable<ICab> DonnéesCab()
        {
            return _listeDonnéesCab;
        }

        private static readonly Regex _RegexCab = new Regex(
            @"<br>(?<jour>\d{1,2})/(?<mois>\d{1,2})/(?<annee>\d{4})\s*(?<heure>\d{2}):(?<minute>\d{2})\s*\d*\s*" +
            @"<A HREF=""(?<url>/RecupCabs/([^/]*/){1,2}(?<codif>\d{5})-(?<nparu>\d{4})(H|S)?-(?<titre>([A-Z%\d]|-){4,9})-(?<prix>(\d|\.)*)-(?<per>[A-Z])(-(?<qualif>[A-Z]{2}))?\.eps)""",
            RegexOptions.Singleline |
            RegexOptions.ExplicitCapture |
            RegexOptions.IgnoreCase |
            RegexOptions.Multiline |
            RegexOptions.Compiled);

        private static readonly Regex _RegexDossier = new Regex(
            @"&lt;dir&gt;\s*<A HREF=""(?<url_rep>[^""]*)""",
            RegexOptions.Singleline |
            RegexOptions.ExplicitCapture |
            RegexOptions.IgnoreCase |
            RegexOptions.Multiline |
            RegexOptions.Compiled);

        private List<ICab> _listeDonnéesCab;
        //private Dictionary<string, TypeEditeur> _logins;

        private void InitialiserLogins()
        {
            //_logins = new Dictionary<string, TypeEditeur>(3)
            //{
            //    ["E001395"] = TypeEditeur.Esi,
            //    ["E001864"] = TypeEditeur.Go,
            //    ["E100531"] = TypeEditeur.Cyber
            //};
        }

        private void ChargerListeDonnéesCab(ITaskObserver observer)
        {
            _listeDonnéesCab =
                (from compte in Configuration.Comptes.AsParallel()
                 select CabsPourCompte(compte, observer.InPart())).SelectMany(
                     cabEnum => cabEnum).ToList();
        }

        private IEnumerable<ICab> CabsPourCompte(CompteCab compte, ITaskObserver observer)
        {
            observer.NotifyProgress(this, 0);
            var cabs = new List<ICab>();

            string pageRacine = OuvrirPage(RequêteDossierRacine(compte), observer.InSlice(0, 7));
            cabs.AddRange(CabsPourPage(pageRacine, compte.Editeur, observer.InSlice(7, 10)));

            ITaskObserver loopObs = observer.InSlice(10, 100);
            var matches = _RegexDossier.Matches(pageRacine);
            var slices = loopObs.GetSlices(matches.Count).GetEnumerator();
            foreach (Match mDossier in matches)
            {
                ITaskObserver sliceObs = loopObs.InSlice(slices.Current);
                var gDossier = mDossier.Groups["url_rep"];
                if (gDossier != null && !String.IsNullOrEmpty(gDossier.Value))
                {
                    string dossier = gDossier.Value;
                    string pageDossier = OuvrirPage(RequêteDossier(compte, dossier), sliceObs.InSlice(0, 70));
                    if (!String.IsNullOrEmpty(pageDossier))
                        cabs.AddRange(CabsPourPage(pageDossier, compte.Editeur, sliceObs.InSlice(70, 100)));
                }
                slices.MoveNext();
            }
            observer.NotifyProgress(this, 100);
            return cabs;
        }

        private IEnumerable<ICab> CabsPourPage(string page, TypeEditeur éditeur, ITaskObserver observer)
        {
            observer.NotifyProgress(this, 0);
            var cabs = new List<ICab>();
            var matches = _RegexCab.Matches(page);
            using (var steps = observer.GetSteps(matches.Count).GetEnumerator())
            {
                foreach (Match mCab in matches)
                {
                    observer.NotifyProgress(this, steps.Current);
                    var donnéesBrutes = CréerDonnéesBrutes(mCab, éditeur);
                    if (donnéesBrutes != null)
                    {
                        var cab = CréerCab(donnéesBrutes);
                        if (cab != null)
                            cabs.Add(cab);
                    }
                    steps.MoveNext();
                }
            }
            observer.NotifyProgress(this, 100);
            return cabs;
        }

        private static DonnéesCabBrutesMlp CréerDonnéesBrutes(Match mCab, TypeEditeur éditeur)
        {
            try
            {
                return
                    new DonnéesCabBrutesMlp
                    {
                        TypeEditeur = éditeur,
                        Codif = mCab.Groups["codif"].Value,
                        Numéro = int.Parse(mCab.Groups["nparu"].Value),
                        Prix = decimal.Parse(mCab.Groups["prix"].Value.Replace(".", ",")),
                        SPériodicité = mCab.Groups["per"].Value,
                        SQualification = mCab.Groups["qualif"].Value,
                        Url = $"http://cabs.mlp.fr{mCab.Groups["url"].Value}",
                        Jour = int.Parse(mCab.Groups["jour"].Value),
                        Mois = int.Parse(mCab.Groups["mois"].Value),
                        Année = int.Parse(mCab.Groups["annee"].Value),
                        Heure = int.Parse(mCab.Groups["heure"].Value),
                        Minutes = int.Parse(mCab.Groups["minute"].Value)
                    };
            }
            catch (Exception)
            {
                // TODO: tracer exception
                return null;
            }
        }

        private ICab CréerCab(DonnéesCabBrutesMlp donnéesCabBrutesMlp)
        {
            try
            {
                var parutionCible =
                    DépôtParutions.Parutions.FirstOrDefault(
                        p => p.Codif == donnéesCabBrutesMlp.Codif && p.Numéro == donnéesCabBrutesMlp.Numéro);

                if (parutionCible == null)
                    return null;


                return CabFactory.CréerCab(this,
                                            donnéesCabBrutesMlp.Url,
                                            donnéesCabBrutesMlp.TypeEditeur,
                                            TypeDistributeur.Mlp,
                                            donnéesCabBrutesMlp.Codif,
                                            donnéesCabBrutesMlp.Numéro,
                                            donnéesCabBrutesMlp.Prix,
                                            PériodicitéMlpVersPériodicité(donnéesCabBrutesMlp.SPériodicité),
                                            QualificationMlpVersTypeQualification(donnéesCabBrutesMlp.SQualification),
                                            new DateTime(donnéesCabBrutesMlp.Année,
                                                         donnéesCabBrutesMlp.Mois,
                                                         donnéesCabBrutesMlp.Jour,
                                                         donnéesCabBrutesMlp.Heure,
                                                         donnéesCabBrutesMlp.Minutes,
                                                         0),
                                            parutionCible);
            }
            catch (Exception)
            {
                // TODO: tracer exception
                return null;
            }
        }

        private static TypeQualificationRéseau QualificationMlpVersTypeQualification(string sQualification)
        {
            if (sQualification == "")
                return TypeQualificationRéseau.Rd;
            else
            {
                if (Enum.TryParse(sQualification, ignoreCase: true, result: out TypeQualificationRéseau typeQualification))
                    return typeQualification;
                throw new ArgumentException("Qualification de CAB MLP inconnue", nameof(sQualification));
            }
        }

        private static TypePériodicité PériodicitéMlpVersPériodicité(string sPériodicité)
        {
            if (sPériodicité == "M")
                return TypePériodicité.Mensuelle;
            if (sPériodicité == "B")
                return TypePériodicité.Bimestrielle;
            if (sPériodicité == "T")
                return TypePériodicité.Trimestrielle;
            if (sPériodicité == "I")
                return TypePériodicité.Irrégulière;
            throw new ArgumentException("Périodicité CAB MLP inconnue", nameof(sPériodicité));
        }

        private HttpWebRequest RequêteDossier(CompteCab compte, string dossier)
        {
            string url = $"{Configuration.Url}/{dossier}/";
            return RequêteUrl(compte, url);
        }

        private HttpWebRequest RequêteUrl(CompteCab compte, string url)
        {
            var repRequest =
                (HttpWebRequest)WebRequest.Create(url);
            repRequest.Credentials = new NetworkCredential(compte.Identifiant, compte.MotDePasse, "");
            repRequest.UserAgent =
                "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            return repRequest;
        }

        private HttpWebRequest RequêteDossierRacine(CompteCab compte)
        {
            string dossierRacine = $"{Configuration.DossierRacine}/{compte.Dossier}";
            return RequêteDossier(compte, dossierRacine);
        }

        public class DonnéesCabBrutesMlp
        {
            public TypeEditeur TypeEditeur { get; set; }
            public string Codif { get; set; }
            public int Numéro { get; set; }
            public decimal Prix { get; set; }
            public string SPériodicité { get; set; }
            public string SQualification { get; set; }
            public string Url { get; set; }
            public int Jour { get; set; }
            public int Année { get; set; }
            public int Heure { get; set; }
            public int Minutes { get; set; }
            public int Mois { get; set; }
        }
    }
}