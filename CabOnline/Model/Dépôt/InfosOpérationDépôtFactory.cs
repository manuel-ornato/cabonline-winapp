using CabOnline.Model.Cab;
using CabOnline.Model.Parution;

namespace CabOnline.Model.Dépôt
{
    class InfosOpérationDépôtFactory : IInfosOpérationDépôtFactory
    {
        public delegate IInfosOpérationDépôt FactoryFunc(ICab cabSource, ICab cabDestination, ICab cabExistant, EtatCabOnline étatCabOnline, IParution parutionCible);

        private readonly FactoryFunc _factoryFunc;

        public InfosOpérationDépôtFactory(FactoryFunc factoryFunc)
        {
            _factoryFunc = factoryFunc;
        }

        public IInfosOpérationDépôt CréerInfosOpérationDépôt(ICab cabSource, ICab cabDestination, ICab cabExistant, EtatCabOnline étatCabOnline, IParution parutionCible)
        {
            return _factoryFunc(cabSource, cabDestination, cabExistant, étatCabOnline, parutionCible);
        }
    }
}
