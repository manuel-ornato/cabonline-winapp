using System;

namespace CabOnline.TaskObserver
{
    internal class ObserverEventArgs : EventArgs
    {
        private readonly object _sender;

        public ObserverEventArgs(object sender)
        {
            _sender = sender;
        }
        
        public object Sender => _sender;
    }
}