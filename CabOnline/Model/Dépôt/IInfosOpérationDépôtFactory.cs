using CabOnline.Model.Cab;
using CabOnline.Model.Parution;

namespace CabOnline.Model.Dépôt
{
    internal interface IInfosOpérationDépôtFactory
    {
        IInfosOpérationDépôt CréerInfosOpérationDépôt(ICab cabSource, ICab cabDestination, ICab cabExistant, EtatCabOnline étatCabOnline, IParution parutionCible);
    }
}