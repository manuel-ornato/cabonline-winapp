

using System;
using System.Linq;

namespace CabOnline.Extensions.Exception
{
    public static class ExceptionExtension
    {
        public static string DetailedMessage(this System.Exception exception)
        {
            string details = String.Empty;
            if(exception is AggregateException)
            {
                var ae = exception as AggregateException;
                var erreurs = from e in ae.InnerExceptions select e.DetailedMessage();
                return String.Join(" | ", erreurs.ToArray());
            }
            else if (exception.InnerException != null)
                details = $" ({exception.InnerException.DetailedMessage()})";
            return $"{exception.Message}{details}";
        }
    }
}
