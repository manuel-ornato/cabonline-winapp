namespace CabOnline.AppSettings
{
    using System;

    public sealed class Configuration
    {
        public Configuration(string racinePao,
                             string dossierCodesbarres,
                             string adresseMailRapportFrom,
                             string adresseMailRapportTo,
                             int trancheDhlSup,
                             int trancheDhlInf,
                             string serveurSmtp,
                             int portSmtp,
                             string utilisateurSmtp,
                             string motDePasseSmtp,
                             bool autentificationSmtp,
                             string serveurBdi,
                             string utilisateurBdi,
                             string motDePasseBdi,
                             string databaseBdi,
                             bool sslSmtp)
        {
            RacinePao = racinePao ?? throw new ArgumentNullException(nameof(racinePao));
            DossierCodesbarres = dossierCodesbarres ?? throw new ArgumentNullException(nameof(dossierCodesbarres));
            AdresseMailRapportFrom = adresseMailRapportFrom ?? throw new ArgumentNullException(nameof(adresseMailRapportFrom));
            AdresseMailRapportTo = adresseMailRapportTo ?? throw new ArgumentNullException(nameof(adresseMailRapportTo));
            TrancheDhlSup = trancheDhlSup;
            TrancheDhlInf = trancheDhlInf;
            ServeurSmtp = serveurSmtp ?? throw new ArgumentNullException(nameof(serveurSmtp));
            UtilisateurSmtp = utilisateurSmtp;
            MotDePasseSmtp = motDePasseSmtp;
            AutentificationSmtp = autentificationSmtp;
            ServeurBdi = serveurBdi ?? throw new ArgumentNullException(nameof(serveurBdi));
            UtilisateurBdi = utilisateurBdi ?? throw new ArgumentNullException(nameof(utilisateurBdi));
            MotDePasseBdi = motDePasseBdi ?? throw new ArgumentNullException(nameof(motDePasseBdi));
            DatabaseBdi = databaseBdi ?? throw new ArgumentNullException(nameof(databaseBdi));
            SslSmtp = sslSmtp;
            PortSmtp = portSmtp;
        }

        public string RacinePao { get; }
        public string DossierCodesbarres { get; }
        public string AdresseMailRapportFrom { get; }
        public string AdresseMailRapportTo { get; }
        public int TrancheDhlSup { get; }
        public int TrancheDhlInf { get; }
        public string ServeurSmtp { get; }
        public int PortSmtp { get; }
        public string UtilisateurSmtp { get; }
        public string MotDePasseSmtp { get; }
        public bool AutentificationSmtp { get; }
        public bool SslSmtp { get; }
        public string ServeurBdi { get; }
        public string UtilisateurBdi { get; }
        public string MotDePasseBdi { get; }
        public string DatabaseBdi { get; }
    }
}