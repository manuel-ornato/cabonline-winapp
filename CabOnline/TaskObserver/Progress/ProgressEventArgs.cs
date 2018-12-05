namespace CabOnline.TaskObserver.Progress
{
    internal class ProgressEventArgs : ObserverEventArgs
    {
        private readonly int _progress;

        public ProgressEventArgs(object sender, int progress)
            : base(sender)
        {
            _progress = progress;
        }

        public int Progress => _progress;
    }
}