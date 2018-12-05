using System;
using CabOnline.Model.Dépôt;
using CabOnline.Model.Parution;

namespace CabOnline.Model.Cab
{
    internal class CabFactory : ICabFactory
    {
        public delegate ICab FactoryFunc(IDépôtCab dépôtCab, string url, IEditeur éditeur,
                             IDistributeur distributeur, string codif, int? numéro, decimal? prix,
                             IPériodicité périodicité, IQualificationRéseau qualif, DateTime dateCréation,
                             IParution parutionCible);

        private readonly IDépôtParutions _dépôtParutions;

        private readonly FactoryFunc _factoryFunc;

        public CabFactory(IDépôtParutions dépôtParutions,
                          FactoryFunc factoryFunc)
        {
            _dépôtParutions = dépôtParutions ?? throw new ArgumentNullException(nameof(dépôtParutions));
            _factoryFunc = factoryFunc ?? throw new ArgumentNullException(nameof(factoryFunc));
        }

        public ICab CréerCab(IDépôtCab dépôtCab, string url, TypeEditeur éditeur,
                             TypeDistributeur distributeur, string codif, int? numéro, decimal? prix,
                             TypePériodicité périodicité, TypeQualificationRéseau qualif, DateTime dateCréation,
                             IParution parutionCible)
        {
            return _factoryFunc(dépôtCab, url, _dépôtParutions.Editeur(éditeur),
                                _dépôtParutions.Distributeur(distributeur), codif, numéro, prix,
                                _dépôtParutions.Périodicité(périodicité),
                                _dépôtParutions.QualificationRéseau(qualif), dateCréation, parutionCible);
        }

        public ICab ClonerCab(ICab cabSource, IDépôtCab dépôtCab, string url, DateTime dateCréation)
        {
            return _factoryFunc(dépôtCab, url, cabSource.Editeur, cabSource.Distributeur, cabSource.Codif,
                                cabSource.Numéro, cabSource.Prix, cabSource.Périodicité, cabSource.Qualif, dateCréation,
                                cabSource.ParutionCible);
        }
    }
}