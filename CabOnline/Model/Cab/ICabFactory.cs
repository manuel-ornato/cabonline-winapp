using System;
using CabOnline.Model.Dépôt;
using CabOnline.Model.Parution;

namespace CabOnline.Model.Cab
{
    internal interface ICabFactory
    {
        ICab CréerCab(IDépôtCab dépôtCab, string url, TypeEditeur éditeur, TypeDistributeur distributeur,
                      string codif, int? numéro, decimal? prix, TypePériodicité périodicité,
                      TypeQualificationRéseau qualif, DateTime dateCréation, IParution parutionCible);

        ICab ClonerCab(ICab cabSource, IDépôtCab dépôtCab, string url, DateTime dateCréation);
    }
}