using CabOnline.Model.Cab;
using CabOnline.TaskObserver;

namespace CabOnline.Model.D�p�t.Pao
{
    interface ID�p�tCabPao : ID�p�tCab
    {
        ICab D�poser(ICab cab, bool cr�erFichierTiff, ITaskObserver observer);
    }
}