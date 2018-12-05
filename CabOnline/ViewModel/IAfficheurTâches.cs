using CabOnline.TaskObserver;

namespace CabOnline.ViewModel
{
    internal interface IAfficheurTâches
    {
        void AjouterTâche(string nom, ITaskObserver observer);
    }
}