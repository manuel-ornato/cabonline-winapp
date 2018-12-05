using System.Collections.Generic;

namespace CabOnline.Operators.RapportMail
{
    internal interface IGénérateurRapportMail
    {
        string RapportMail(IEnumerable<IInfosTraitementParution> infosParutions);
    }
}