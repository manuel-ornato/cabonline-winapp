using System.IO;
using CabOnline.Model.Cab;
using CabOnline.Model.Parution;
using CabOnline.TaskObserver;

namespace CabOnline.Model.Dépôt
{
    internal interface IDépôtCab
    {
        //IEnumerable<ICab> Cabs { get; }
        string LibelléCourt { get; }
        string LibelléLong { get; }
        void ChargerCabs(ITaskObserver observer);
        ICab Cab(IParution parution);
        Stream ObtenirContenu(ICab cab);
//        ICab Déposer(ICab cab, ITaskObserver observer);
    }
}