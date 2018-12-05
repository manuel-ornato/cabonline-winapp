using System;

namespace CabOnline.Model.AnomalieCab
{
    internal class AnomalieCabFactory : IAnomalieCabFactory
    {
        public delegate IAnomalieCab FactoryFunc(TypeAnomalieCab type, string valeurParution, string valeurCab, bool nécessiteAction, bool estIncompatible);

        private readonly FactoryFunc _factoryFunc;

        public AnomalieCabFactory(FactoryFunc factoryFunc)
        {
            _factoryFunc = factoryFunc ?? throw new ArgumentNullException(nameof(factoryFunc));
        }

        public IAnomalieCab CréerAnomalieCab(TypeAnomalieCab type, string valeurParution, string valeurCab, bool nécessiteAction, bool estIncompatible)
        {
            return _factoryFunc(type, valeurParution, valeurCab, nécessiteAction, estIncompatible);
        }
    }
}