using CabOnline.Model.Cab;
using CabOnline.Model.Parution;

namespace CabOnline.Model.Dépôt
{
    internal interface IInfosOpérationDépôt
    {
        ICab CabDestination { get; }
        ICab CabSource { get; }
        ICab CabExistant { get; }
        EtatCabOnline EtatCabOnline { get; }
        IParution ParutionCible { get; }
    }
}