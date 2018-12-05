namespace CabOnline.Operators.Mailer
{
    internal interface IMailer
    {
        void Send(string subject, string body);
    }
}