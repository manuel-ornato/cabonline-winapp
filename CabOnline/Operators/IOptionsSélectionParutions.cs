using System;
using CabOnline.Model.Parution;

namespace CabOnline.Operators
{
    internal interface IOptionsSélectionParutions
    {
        int Numéro { get; set; }
        string CodeInterne { get; set; }
        IPôle Pôle { get; set; }
        DateTime? DateDébut { get; set; }
        DateTime? DateFin { get; set; }
        bool FiltrerParPôle { get; set; }
        bool FiltrerParDate { get; set; }
        bool FiltrerParCodeInterne { get; set; }
        bool FiltrerParNuméro { get; set; }
    }
}