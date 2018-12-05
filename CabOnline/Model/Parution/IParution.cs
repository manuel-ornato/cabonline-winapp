using System;

namespace CabOnline.Model.Parution
{
    internal interface IParution
    {
        IEditeur Editeur { get; }
        IDistributeur Distributeur { get; }
        IPôle Pôle { get; }
        string CodeSérie { get; }
        string Codif { get; }
        int? Numéro { get; }
        decimal? Prix { get; }
        IPériodicité Périodicité { get; }
        IPériodicité PériodicitéEffective { get; }
        IQualificationRéseau Qualif { get; }
        DateTime? DateDhl { get; }
        bool SupplémentAutreProduit { get; }
        bool FaitSortie { get; }
        bool FaitPao { get; }
        string StatutPlanning { get; }
        string Titre { get; }
        string Prestataire { get; }
        string CodeProduit { get; }

        string LibelléCourt { get; }
        string LibelléLong { get; }

        bool Correspond(string codeSérie, int numéro);
    }
}