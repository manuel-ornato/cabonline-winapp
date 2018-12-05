using System;
using CabOnline.TaskObserver.Messages;

namespace CabOnline.ViewModel
{
    internal class MessageTâcheViewModel : ViewModel
    {
        private readonly Message _message;

        public MessageTâcheViewModel(Message message)
        {
            _message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public MessageLevel Niveau => _message.Level;

        public string Message => _message.Body;

        public DateTime Heure => _message.Time;
    }
}