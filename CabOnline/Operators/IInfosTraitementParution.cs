using System;
using System.Collections.Generic;
using CabOnline.Model.AnomalieCab;
using CabOnline.Model.Cab;
using CabOnline.Model.Dépôt;
using CabOnline.Model.Parution;
using CabOnline.ViewModel;

namespace CabOnline.Operators
{
    internal interface IInfosTraitementParution
    {
        DateTime? DateDhl { get; }
        string CodeProduit { get; }
        string TitreEtNuméro { get; }
        EtatCab EtatCab { get; }
        EtatCabOnline EtatCabOnline { get; }
        bool CabAbsent { get; }
        IAnomalieCab AnomalieEditeur { get; }
        bool EditeurEstAnormal { get; }
        IAnomalieCab AnomalieDistributeur { get; }
        bool DistributeurEstAnormal { get; }
        IAnomalieCab AnomalieCodif { get; }
        bool CodifEstAnormale { get; }
        IAnomalieCab AnomaliePrix { get; }
        bool PrixEstAnormal { get; }
        IAnomalieCab AnomaliePériodicité { get; }
        bool PériodicitéEstAnormale { get; }
        IAnomalieCab AnomalieQualification { get; }
        bool QualificationEstAnormale { get; }
        IAnomalieCab AnomalieNuméro { get; }
        bool NuméroEstAnormal { get; }
        IParution Parution { get; }
        ICab Cab { get; }
        ICab CabOnline { get; }
        bool OnlineEstAnormal { get; }
        IEnumerable<IAnomalieCab> AnomaliesOnline { get; }
        IInfosOpérationDépôt InfosOpérationDépôt { get; }
    }
}