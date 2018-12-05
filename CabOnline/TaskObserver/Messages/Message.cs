using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CabOnline.TaskObserver.Messages
{
    internal sealed class Message
    {
        private readonly string _body;
        private readonly DateTime _time;
        private readonly MessageLevel _level;

        public Message(string body, DateTime time, MessageLevel level)
        {
            _body = body ?? throw new ArgumentNullException(nameof(body));
            _level = level;
            _time = time;
        }

        public MessageLevel Level => _level;

        public DateTime Time => _time;

        public string Body => _body;
    }
}
