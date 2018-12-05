using System;

namespace CabOnline.TaskObserver.Messages
{
    internal class MessageEventArgs : ObserverEventArgs
    {
        private readonly Message _message;

        public MessageEventArgs(object sender, Message message)
            : base(sender)
        {
            _message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public Message Message => _message;
    }
}