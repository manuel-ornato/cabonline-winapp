using CabOnline.Model.Cab;
using CabOnline.TaskObserver;

namespace CabOnline.Model.Dépôt.Pao
{
    interface IDépôtCabPao : IDépôtCab
    {
        ICab Déposer(ICab cab, bool créerFichierTiff, ITaskObserver observer);
    }
}