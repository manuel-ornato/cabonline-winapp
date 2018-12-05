using System;
using System.Linq;

namespace CabOnline.Model.Parution.BdiEF
{
    internal class ParutionBdi : IParution
    {
        public ParutionBdi(DistributeurBdi distributeur, EditeurBdi éditeur, QualificationRéseauBdi qualificationRéseau, PôleBdi pôle, PériodicitéBdi périodicité, string refDistributeur, string titre, string référence, decimal? prixImprimé, int numéro, DateTime? vDateDépartFab, DateTime? dateDépartFab, string statutSérie, string régulateur, bool faitSortie, dynamic faitPao)
        {
            Editeur = éditeur;
            Distributeur = distributeur;
            Périodicité = périodicité;
            Qualif = qualificationRéseau;
            Pôle = pôle;
            CodeSérie = référence!=null ?  référence.Substring(0, 5) : "";
            Codif = refDistributeur != null ? refDistributeur.Trim() : "";
            Numéro = numéro;
            Prix = prixImprimé;
            DateDhl = vDateDépartFab ?? dateDépartFab;
            FaitSortie = faitSortie;
            StatutPlanning = statutSérie;
            Titre = titre != null ? titre.Trim() : "";
            Prestataire = régulateur != null ? régulateur.Trim() : "";
            FaitPao = faitPao;
        }

        public IEditeur Editeur { get; private set; }

        public IDistributeur Distributeur { get; private set; }

        public IPôle Pôle { get; private set; }


        public string CodeSérie { get; private set; }

        public string Codif { get; private set; }

        public int? Numéro { get; private set; }

        public decimal? Prix { get; private set; }

        public IPériodicité Périodicité { get; private set; }

        public IPériodicité PériodicitéEffective => Périodicité;
        public IQualificationRéseau Qualif { get; private set; }

        public DateTime? DateDhl { get; private set; }

        public bool SupplémentAutreProduit => false;
        public bool FaitSortie { get; private set; }

        public bool FaitPao { get; private set; }

        public string StatutPlanning { get; private set; }

        public string Titre { get; private set; }

        public string Prestataire { get; private set; }

        public string CodeProduit => $"{CodeSérie}{Numéro:000}";

        public string LibelléCourt => CodeProduit;

        public string LibelléLong => $"{CodeProduit} - {Titre}";

        public bool Correspond(string codeSérie, int numéro)
        {
            if (codeSérie.Length == 4)
                return (CodeSérie == codeSérie + "_") && numéro == Numéro;
            return (CodeSérie == codeSérie && numéro == Numéro);
        }

        public override string ToString()
        {
            return LibelléCourt;
        }
    }
}