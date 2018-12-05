using CabOnline.Model.Cab;
using CabOnline.Model.Parution;

namespace CabOnline.Model.Dépôt
{
    class InfosOpérationDépôt : IInfosOpérationDépôt
    {
        private readonly ICab _cabSource;
        private readonly ICab _cabDestination;
        private readonly ICab _cabExistant;
        private readonly EtatCabOnline _étatCabOnline;
        private readonly IParution _parutionCible;

        public InfosOpérationDépôt(ICab cabSource, ICab cabDestination, ICab cabExistant, EtatCabOnline étatCabOnline, IParution parutionCible)
        {
            _cabSource = cabSource;
            _parutionCible = parutionCible;
            _étatCabOnline = étatCabOnline;
            _cabDestination = cabDestination;
            _cabExistant = cabExistant;
        }

        public IParution ParutionCible => _parutionCible;

        public EtatCabOnline EtatCabOnline => _étatCabOnline;

        public ICab CabExistant => _cabExistant;

        public ICab CabDestination => _cabDestination;

        public ICab CabSource => _cabSource;
    }
}
