using System;
using System.Collections.Generic;
using System.IO;
using CabOnline.Model.AnomalieCab;
using CabOnline.Model.Dépôt;
using CabOnline.Model.Parution;

namespace CabOnline.Model.Cab
{
    internal interface ICab
    {
        IDépôtCab Dépôt { get; }
        IParution ParutionCible { get; }
        IEditeur Editeur { get; }
        IDistributeur Distributeur { get; }
        string Codif { get; }
        int? Numéro { get; }
        decimal? Prix { get; }
        IPériodicité Périodicité { get; }
        IQualificationRéseau Qualif { get; }
        string Url { get; }
        DateTime DateCréation { get; }
        bool EstCompatible { get; }
        bool NécessiteCorrection { get; }
        IEnumerable<IAnomalieCab> Anomalies { get; }

        string LibelléLong { get;}
        string LibelléCourt { get; }

        Stream ObtenirContenu();
    }
}