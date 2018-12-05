using System;
using System.Collections.Generic;
using CabOnline.Model.AnomalieCab;
using CabOnline.Model.Cab;
using CabOnline.Model.D�p�t;
using CabOnline.Model.Parution;
using CabOnline.ViewModel;

namespace CabOnline.Operators
{
    internal interface IInfosTraitementParution
    {
        DateTime? DateDhl { get; }
        string CodeProduit { get; }
        string TitreEtNum�ro { get; }
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
        IAnomalieCab AnomalieP�riodicit� { get; }
        bool P�riodicit�EstAnormale { get; }
        IAnomalieCab AnomalieQualification { get; }
        bool QualificationEstAnormale { get; }
        IAnomalieCab AnomalieNum�ro { get; }
        bool Num�roEstAnormal { get; }
        IParution Parution { get; }
        ICab Cab { get; }
        ICab CabOnline { get; }
        bool OnlineEstAnormal { get; }
        IEnumerable<IAnomalieCab> AnomaliesOnline { get; }
        IInfosOp�rationD�p�t InfosOp�rationD�p�t { get; }
    }
}