using CabOnline.Model.Cab;
using CabOnline.Model.Parution;

namespace CabOnline.Model.D�p�t
{
    internal interface IInfosOp�rationD�p�t
    {
        ICab CabDestination { get; }
        ICab CabSource { get; }
        ICab CabExistant { get; }
        EtatCabOnline EtatCabOnline { get; }
        IParution ParutionCible { get; }
    }
}