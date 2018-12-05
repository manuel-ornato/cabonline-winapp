using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CabOnline.Model.Cab;
using CabOnline.Model.Parution;
using CabOnline.TaskObserver;

namespace CabOnline.Model.Dépôt.DossierCab
{
    //internal class DépôtCabDossierCab : DépôtCab
    //{
    //    public DépôtCabDossierCab(ICabFactory cabFactory, IDépôtParutions dépôtParutions)
    //        : base(cabFactory, dépôtParutions)
    //    {
    //    }

    //    public override string LibelléCourt => "Dossier Codesbarres";

    //    public override void ChargerCabs(ITaskObserver observer)
    //    {
    //        try
    //        {
    //            ChargerDossierCab(observer);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new Exception("Problème durant la recherche des CAB dans 'Codesbarres'", ex);
    //        }
    //    }

    //    public override Stream ObtenirContenu(ICab cab)
    //    {
    //        return new FileStream(cab.Url, FileMode.Open);
    //    }

    //    protected override IEnumerable<ICab> DonnéesCab()
    //    {
    //        return _listeDonnéesCab;
    //    }

    //    private static readonly Regex _regexCab =
    //        new Regex(
    //            @"^(?<code>[A-Z0-9]{4})[-_\s](?<codif>\d{5})[-_\s]N(?<num>\d+)H?[-_\s](?<prix>\d+\.\d+)[-_\s](?<periodicite>[A-Z])[-_\s](?<qualif>[A-Z]{2})$",
    //            RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

    //    private List<ICab> _listeDonnéesCab;

    //    private void ChargerDossierCab(ITaskObserver taskObserver)
    //    {
    //        taskObserver.NotifyStatus(this, "Examen du dossier Codesbarres");
    //        taskObserver.NotifyProgress(this, 0);

    //        DirectoryInfo di = new DirectoryInfo(Settings.Default.DossierCodesbarres);
    //        FileInfo[] fichiersCab = di.GetFiles("*.eps", SearchOption.TopDirectoryOnly);
            
    //        Func<int, int> reportProgress =
    //            progress =>
    //                {
    //                    taskObserver.NotifyProgress(this, progress);
    //                    return progress;
    //                };

    //        int compteur = 0;
    //        int total = fichiersCab.Length;

    //        IEnumerable<ICab> cabs =
    //            from f in fichiersCab
    //            let doProgressSideEffect = reportProgress((compteur++*100)/total)
    //            let nomFichier = Path.GetFileNameWithoutExtension(f.Name)
    //            where nomFichier != null
    //            let m = _regexCab.Match(nomFichier)
    //            where m.Success
    //            let donnéesBrutes = CréerDonnéesBrutes(m, f)
    //            where donnéesBrutes != null
    //            let cab = CréerCab(donnéesBrutes)
    //            where cab != null
    //            select cab;

    //        _listeDonnéesCab = new List<ICab>(cabs);
    //        taskObserver.NotifyProgress(this, 0);
    //    }


    //    private static DonnéesBrutesCabDossierCab CréerDonnéesBrutes(Match m, FileInfo f)
    //    {
    //        try
    //        {
    //            return
    //                new DonnéesBrutesCabDossierCab
    //                    {
    //                        TypeDistributeur =
    //                            m.Groups["codif"].Value[0] == '1'
    //                                ? TypeDistributeur.Mlp
    //                                : TypeDistributeur.Nmpp,
    //                        Codif = m.Groups["codif"].Value,
    //                        Numéro = int.Parse(m.Groups["num"].Value),
    //                        Prix = decimal.Parse(m.Groups["prix"].Value),
    //                        SPériodicité = m.Groups["periodicite"].Value,
    //                        SQualification = m.Groups["qualif"].Value,
    //                        Url = f.FullName,
    //                        DateCréation = f.CreationTime
    //                    };
    //        }
    //        catch (Exception)
    //        {
    //            // TODO: tracer exception
    //            return null;
    //        }
    //    }

    //    private ICab CréerCab(DonnéesBrutesCabDossierCab donnéesBrutesCabDossierCab)
    //    {
    //        try
    //        {
    //            var parutionCible =
    //                (from p in _dépôtParutions.Parutions
    //                 where p.Codif == donnéesBrutesCabDossierCab.Codif && p.Numéro == donnéesBrutesCabDossierCab.Numéro
    //                 select p).FirstOrDefault();

    //            if (parutionCible == null)
    //                return null;

    //            return _cabFactory.CréerCab(this,donnéesBrutesCabDossierCab.Url,
    //                                        TypeEditeur.Inconnu, donnéesBrutesCabDossierCab.TypeDistributeur,
    //                                        donnéesBrutesCabDossierCab.Codif,
    //                                        donnéesBrutesCabDossierCab.Numéro, donnéesBrutesCabDossierCab.Prix,
    //                                        PériodicitéDossierCabVersTypePériodicité(
    //                                            donnéesBrutesCabDossierCab.SPériodicité),
    //                                        QualificationDossierCabVersTypeQualificationRéseau(
    //                                            donnéesBrutesCabDossierCab.SQualification),
    //                                        donnéesBrutesCabDossierCab.DateCréation, parutionCible);
    //        }
    //        catch (Exception)
    //        {
    //            // TODO: tracer l'exception
    //            return null;
    //        }
    //    }

    //    private static TypeQualificationRéseau QualificationDossierCabVersTypeQualificationRéseau(string sQualification)
    //    {
    //        TypeQualificationRéseau qualification;
    //        if (Enum.TryParse(sQualification, ignoreCase: true, result: out qualification))
    //            return qualification;
    //        throw new ArgumentException("Qualification réseau inconnue", "sQualification");
    //    }

    //    private static TypePériodicité PériodicitéDossierCabVersTypePériodicité(string sPériodicité)
    //    {
    //        switch (sPériodicité[0])
    //        {
    //            case 'M':
    //                return TypePériodicité.Mensuelle;
    //            case 'T':
    //                return TypePériodicité.Trimestrielle;
    //            case 'B':
    //                return TypePériodicité.Bimestrielle;
    //            case 'I':
    //                return TypePériodicité.Irrégulière;
    //        }
    //        throw new ArgumentException("Périodicité inconnue", "sPériodicité");
    //    }

    //    private class DonnéesBrutesCabDossierCab
    //    {
    //        public TypeDistributeur TypeDistributeur { get; set; }
    //        public string Codif { get; set; }
    //        public int Numéro { get; set; }
    //        public decimal Prix { get; set; }
    //        public string SPériodicité { get; set; }
    //        public string SQualification { get; set; }
    //        public string Url { get; set; }
    //        public DateTime DateCréation { get; set; }
    //    }
    //}
}