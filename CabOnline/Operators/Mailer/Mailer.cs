namespace CabOnline.Operators.Mailer
{
    using MailKit.Net.Smtp;
    using MimeKit;

    internal class Mailer : IMailer
    {
        private readonly bool _authentication;
        private readonly string _from;
        private readonly string _fromLabel;
        private readonly string _password;
        private readonly int _port;
        private readonly string _server;
        private readonly bool _ssl;
        private readonly string _to;
        private readonly string _toLabel;
        private readonly string _user;

        public Mailer(string server,
                      int? port,
                      bool authentication,
                      string user,
                      string password,
                      string to,
                      string toLabel,
                      string from,
                      string fromLabel,
                      bool ssl)
        {
            _server = server;
            _port = port ?? 25;
            _authentication = authentication;
            _user = user;
            _password = password;
            _toLabel = toLabel;
            _fromLabel = fromLabel;
            _ssl = ssl;
            _from = from;
            _to = to;
        }

        public void Send(string subject, string body)
        {
            var bodyBuilder = new BodyBuilder {HtmlBody = body};
            var message =
                new MimeMessage
                {
                    From = {new MailboxAddress(_fromLabel, _from)},
                    To = {new MailboxAddress(_toLabel, _to)},
                    Subject = subject,
                    Body = bodyBuilder.ToMessageBody()
                };
            using (var smtp = new SmtpClient())
            {
                smtp.Connect(_server, _port);
                if (_authentication)
                    smtp.Authenticate(_user, _password);
                smtp.Send(message);
            }
        }
    }
}