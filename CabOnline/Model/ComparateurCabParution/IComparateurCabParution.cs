using System.Collections.Generic;
using CabOnline.Model.AnomalieCab;
using CabOnline.Model.Cab;

namespace CabOnline.Model.ComparateurCabParution
{
    internal interface IComparateurCabParution
    {
        void CalculerCompatbilité(ICab cab, out bool estCompatible, out IEnumerable<IAnomalieCab> anomalies);
    }
}