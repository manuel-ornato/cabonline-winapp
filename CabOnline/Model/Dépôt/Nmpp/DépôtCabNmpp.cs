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

namespace CabOnline.Model.Dépôt.Nmpp
{
    internal class DépôtCabNmpp : DépôtCabOnline, IDépôtCabNmpp
    {
        public DépôtCabNmpp(ICabFactory cabFactory, IDépôtParutions dépôtParutions)
            : base(cabFactory, dépôtParutions)
        {
        }

        public override string LibelléCourt => "Site NMPP";

        public override void ChargerCabs(ITaskObserver observer)
        {
            try
            {
                ChargerListeDonnéesCab(observer);
            }
            catch (Exception ex)
            {
                throw new Exception("Problème durant la recherche des CAB sur le site NMPP", ex);
            }
        }

        public override Stream ObtenirContenu(ICab cab)
        {
            try
            {
                var request = RequêteFtp(cab.Url);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                Stream s;
                try
                {
                    s = OuvrirFlux(request);
                    return s;
                }
                catch
                {
                    // On donne une seconde chance en cas de souci réseau:
                    Thread.Sleep(5000);

                    request = RequêteFtp(cab.Url);
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    s = OuvrirFlux(request);
                    return s;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Problème durant le téléchargement du CAB de " + cab.Codif + " n°" + cab.Numéro +
                    " depuis le site NMPP", ex);
            }
        }

        protected override IEnumerable<ICab> DonnéesCab()
        {
            return _listeDonnéesCab;
        }

        private const string _UrlRacine = "ftp://195.101.40.3/";

        private static readonly Regex _RegexCab = new Regex(
            @"[-rwx]{10}\s*\d\s*\d{8}\s*\w*\s*\d*\s*(?<mois>\w{3})\s*(?<jour>\d{1,2})\s*((?<annee>\d{4})|((?<heure>\d{2}):(?<min>\d{2})))\s*" +
            @"(?<nomFichier>(?<codif>\d{5})-(?<num>\d{4})_-(?<prix>\d{4})-(?<qualif>[A-Z]{2})\.eps)",
            RegexOptions.Singleline |
            RegexOptions.ExplicitCapture |
            RegexOptions.IgnoreCase |
            RegexOptions.Multiline |
            RegexOptions.Compiled);

        private List<ICab> _listeDonnéesCab;

        private void ChargerListeDonnéesCab(ITaskObserver taskObserver)
        {
            taskObserver.NotifyStatus(this, "Extraction des Cab NMPP");
            string listingCabFtp = ListingCabFtp(taskObserver);
            ExtraireCabListing(listingCabFtp, taskObserver);
        }

        private void ExtraireCabListing(string listingCabFtp, ITaskObserver taskObserver)
        {
            taskObserver.NotifyProgress(this, 0);

            Func<int, int> reportProgress =
                progress =>
                    {
                        taskObserver.NotifyProgress(this, progress);
                        return progress;
                    };

            var matches = _RegexCab.Matches(listingCabFtp);

            var compteur = 0;
            int total = matches.Count;
            var donnéesCabRacine =
                from Match mCab in matches
                let doProgressSideEffect = reportProgress((compteur++*100)/total)
                let donnéesBrutes = CréerDonnéesBrutes(mCab)
                where donnéesBrutes != null
                let cab = CréerCab(donnéesBrutes)
                where cab != null
                select cab;

            _listeDonnéesCab = new List<ICab>(donnéesCabRacine);
            taskObserver.NotifyProgress(this, 100);
        }

        private static DonnéesCabBrutesNmpp CréerDonnéesBrutes(Match mCab)
        {
            try
            {
                return
                    new DonnéesCabBrutesNmpp
                        {
                            Codif = mCab.Groups["codif"].Value,
                            Numéro = int.Parse(mCab.Groups["num"].Value),
                            Prix =
                                decimal.Parse(mCab.Groups["prix"].Value.Replace(".", ",")),
                            SQualification = mCab.Groups["qualif"].Value,
                            Url = _UrlRacine + mCab.Groups["nomFichier"].Value,
                            Jour = int.Parse(mCab.Groups["jour"].Value),
                            SMois = mCab.Groups["mois"].Value,
                            SAnnée = mCab.Groups["annee"].Value,
                            Heure =
                                int.Parse(mCab.Groups["heure"].Value != ""
                                              ? mCab.Groups["heure"].Value
                                              : "00"),
                            Minutes =
                                int.Parse(mCab.Groups["min"].Value != ""
                                              ? mCab.Groups["min"].Value
                                              : "00")
                        };
            }
            catch (Exception)
            {
                // TODO: tracer exception
                return null;
            }
        }

        private ICab CréerCab(DonnéesCabBrutesNmpp donnéesCabBrutesNmpp)
        {
            try
            {
                var parutionCible =
                    (from p in DépôtParutions.Parutions
                     where p.Codif == donnéesCabBrutesNmpp.Codif && p.Numéro == donnéesCabBrutesNmpp.Numéro
                     select p).FirstOrDefault();

                if (parutionCible == null)
                    return null;

                int mois = ConversionMois(donnéesCabBrutesNmpp.SMois);
                int année = ConversionAnnée(donnéesCabBrutesNmpp.SAnnée, mois, donnéesCabBrutesNmpp.Jour);
                return CabFactory.CréerCab(this, donnéesCabBrutesNmpp.Url, TypeEditeur.Inconnu,
                                            TypeDistributeur.Nmpp, donnéesCabBrutesNmpp.Codif,
                                            donnéesCabBrutesNmpp.Numéro,
                                            donnéesCabBrutesNmpp.Prix, TypePériodicité.Inconnue,
                                            QualificationRéseauNmppVersTypeQualificationRéseau(
                                                donnéesCabBrutesNmpp.SQualification),
                                            new DateTime(année, mois,
                                                         donnéesCabBrutesNmpp.Jour, donnéesCabBrutesNmpp.Heure,
                                                         donnéesCabBrutesNmpp.Minutes, 0),
                                            parutionCible);
            }
            catch (Exception)
            {
                // TODO: tracer exception
                return null;
            }
        }

        private static TypeQualificationRéseau QualificationRéseauNmppVersTypeQualificationRéseau(string sQualification)
        {
            TypeQualificationRéseau qualif;
            if (Enum.TryParse(sQualification, ignoreCase: true, result: out qualif))
                return qualif;

            throw new ArgumentException("Qualification réseau inconnue", nameof(sQualification));
        }

        private static int ConversionAnnée(string sAnnée, int mois, int jour)
        {
            int année;
            if (sAnnée != "")
                année = Convert.ToInt32(sAnnée);
            else
            {
                année = DateTime.Now.Year;
                if ((new DateTime(année, mois, jour)) > DateTime.Now.Date)
                    année -= 1;
            }
            return année;
        }

        private static int ConversionMois(string sMois)
        {
            sMois = sMois.ToUpper();
            switch (sMois)
            {
                case "JAN":
                    return 1;
                case "FEB":
                    return 2;
                case "MAR":
                    return 3;
                case "APR":
                    return 4;
                case "MAY":
                    return 5;
                case "JUN":
                    return 6;
                case "JUL":
                    return 7;
                case "AUG":
                    return 8;
                case "SEP":
                    return 9;
                case "OCT":
                    return 10;
                case "NOV":
                    return 11;
                default:
                    return 12;
            }
        }

        private string ListingCabFtp(ITaskObserver observer)
        {
            var request = RequêteFtp(_UrlRacine);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            return OuvrirPage(request, observer);
        }

        private static FtpWebRequest RequêteFtp(string url)
        {
            var request = (FtpWebRequest) WebRequest.Create(url);
            request.Credentials = new NetworkCredential("00012818", "450E7621");
            request.UsePassive = true;
            return request;
        }

        private class DonnéesCabBrutesNmpp
        {
            public string Codif { get; set; }

            public int Numéro { get; set; }

            public decimal Prix { get; set; }

            public string SQualification { get; set; }

            public string Url { get; set; }

            public int Jour { get; set; }

            public string SMois { get; set; }

            public string SAnnée { get; set; }

            public int Heure { get; set; }

            public int Minutes { get; set; }
        }
    }
}