using CabOnline.Model.Cab;
using CabOnline.Model.Parution;

namespace CabOnline.Model.D�p�t
{
    internal interface IInfosOp�rationD�p�tFactory
    {
        IInfosOp�rationD�p�t Cr�erInfosOp�rationD�p�t(ICab cabSource, ICab cabDestination, ICab cabExistant, EtatCabOnline �tatCabOnline, IParution parutionCible);
    }
}