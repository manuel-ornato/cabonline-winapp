using System;
using Antlr3.ST;

namespace CabOnline.Operators.RapportMail.StRenderers
{
    internal class DateRenderer : IAttributeRenderer
    {
        public string ToString(object o)
        {
            return ToString(o, null);
        }

        public string ToString(object o, string format)
        {
            if (o == null)
                return null;

            if (string.IsNullOrEmpty(format))
                return o.ToString();

            var dt = Convert.ToDateTime(o);

            return dt.ToString(format);
        }
    }
}