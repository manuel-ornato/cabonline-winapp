using System;

namespace CabOnline.TaskObserver.Messages
{
    internal class MessagingManager
    {
        public event EventHandler<MessageEventArgs> OnStatus;
        public event EventHandler<MessageEventArgs> OnDebug;
        public event EventHandler<MessageEventArgs> OnDetail;
        public event EventHandler<MessageEventArgs> OnInfo;
        public event EventHandler<MessageEventArgs> OnWarning;
        public event EventHandler<MessageEventArgs> OnError;
        public event EventHandler<ObserverEventArgs> OnStarted;
        public event EventHandler<ObserverEventArgs> OnCompleted;
        public event EventHandler<MessageEventArgs> OnAborted;
        public event EventHandler<ObserverEventArgs> OnIsIndeterminate;

        public void NotifyStatus(object sender, string status)
        {
            RaiseStatusEvent(sender, status);
        }

        public void NotifyDebug(object sender, string debug)
        {
            RaiseDebugEvent(sender, debug);
        }

        public void NotifyDetail(object sender, string detail)
        {
            RaiseDetailEvent(sender, detail);
        }

        public void NotifyInfo(object sender, string info)
        {
            RaiseInfoEvent(sender, info);
        }

        public void NotifyWarning(object sender, string warning)
        {
            RaiseWarningEvent(sender, warning);
        }

        public void NotifyError(object sender, string error)
        {
            RaiseErrorEvent(sender, error);
        }

        public void NotifyStarted(object sender)
        {
            RaiseStartedEvent(sender);
        }

        public void NotifyCompleted(object sender)
        {
            RaiseCompletedEvent(sender);
        }

        public void NotifyAborted(object sender, string message)
        {
            RaiseAbortedEvent(sender, message);
        }

        public void NotifyIsIndeterminate(object sender)
        {
            RaiseIsIndeterminateEvent(sender);
        }

        private void RaiseDebugEvent(object sender, string message)
        {
            var handler = OnDebug;
            if (handler != null)
                handler(this, new MessageEventArgs(sender, new Message(message, DateTime.Now, MessageLevel.Debug)));
        }

        private void RaiseDetailEvent(object sender, string message)
        {
            var handler = OnDetail;
            if (handler != null)
                handler(this, new MessageEventArgs(sender, new Message(message, DateTime.Now, MessageLevel.Detail)));
        }

        private void RaiseInfoEvent(object sender, string message)
        {
            var handler = OnInfo;
            if (handler != null)
                handler(this, new MessageEventArgs(sender, new Message(message, DateTime.Now, MessageLevel.Info)));
        }

        private void RaiseWarningEvent(object sender, string message)
        {
            var handler = OnWarning;
            if (handler != null)
                handler(this, new MessageEventArgs(sender, new Message(message, DateTime.Now, MessageLevel.Warning)));
        }

        private void RaiseErrorEvent(object sender, string message)
        {
            var handler = OnError;
            if (handler != null)
                handler(this, new MessageEventArgs(sender, new Message(message, DateTime.Now, MessageLevel.Error)));
        }

        private void RaiseStatusEvent(object sender, string message)
        {
            var handler = OnStatus;
            if (handler != null)
                handler(this, new MessageEventArgs(sender, new Message(message, DateTime.Now, MessageLevel.Status)));
        }

        private void RaiseStartedEvent(object sender)
        {
            var handler = OnStarted;
            if (handler != null)
                handler(this, new ObserverEventArgs(sender));
        }

        private void RaiseCompletedEvent(object sender)
        {
            var handler = OnCompleted;
            if (handler != null)
                handler(this, new ObserverEventArgs(sender));
        }

        private void RaiseAbortedEvent(object sender, string message)
        {
            var handler = OnAborted;
            if (handler != null)
                handler(this, new MessageEventArgs(sender, new Message(message, DateTime.Now, MessageLevel.Aborted)));
        }

        private void RaiseIsIndeterminateEvent(object sender)
        {
            var handler = OnIsIndeterminate;
            if (handler != null)
                handler(this, new ObserverEventArgs(sender));
        }
    }
}