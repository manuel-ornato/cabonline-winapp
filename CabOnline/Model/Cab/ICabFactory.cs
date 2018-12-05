using System;
using CabOnline.Model.D�p�t;
using CabOnline.Model.Parution;

namespace CabOnline.Model.Cab
{
    internal interface ICabFactory
    {
        ICab Cr�erCab(ID�p�tCab d�p�tCab, string url, TypeEditeur �diteur, TypeDistributeur distributeur,
                      string codif, int? num�ro, decimal? prix, TypeP�riodicit� p�riodicit�,
                      TypeQualificationR�seau qualif, DateTime dateCr�ation, IParution parutionCible);

        ICab ClonerCab(ICab cabSource, ID�p�tCab d�p�tCab, string url, DateTime dateCr�ation);
    }
}