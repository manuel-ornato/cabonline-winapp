using CabOnline.Model.Dépôt;

namespace CabOnline.Model.Parution
{
    internal class CalculateurPériodicitéEffective : ICalculateurPériodicitéEffective
    {
        private readonly IDépôtParutions _dépôtParutions;

        public CalculateurPériodicitéEffective(IDépôtParutions dépôtParutions)
        {
            _dépôtParutions = dépôtParutions;
        }

        public IPériodicité PériodicitéEffective(IPériodicité périodicité, int? numéro)
        {
            if (périodicité.TypePériodicité != TypePériodicité.BiPuisTri || !numéro.HasValue)
                return périodicité;

            if (numéro < 3)
                return _dépôtParutions.Périodicité(TypePériodicité.Bimestrielle);

            return _dépôtParutions.Périodicité(TypePériodicité.Trimestrielle);
        }
    }
}