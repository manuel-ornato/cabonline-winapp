namespace CabOnline.TaskObserver
{
    internal interface ITaskObserverFactory
    {
        ITaskObserver CreateTaskObserver();
    }
}