using CabOnline.TaskObserver;

namespace CabOnline.ViewModel
{
    internal interface IAfficheurT�ches
    {
        void AjouterT�che(string nom, ITaskObserver observer);
    }
}