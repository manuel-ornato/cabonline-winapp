using System.IO;
using System.Net;
using System.Text;
using CabOnline.Model.Cab;
using CabOnline.TaskObserver;

namespace CabOnline.Model.D�p�t
{
    internal abstract class D�p�tCabOnline : D�p�tCab
    {
        protected D�p�tCabOnline(ICabFactory cabFactory, ID�p�tParutions d�p�tParutions)
            : base(cabFactory, d�p�tParutions)
        {
        }

        protected string OuvrirPage(WebRequest request, ITaskObserver observer)
        {
            string page;

            observer.NotifyProgress(this, 0);

            var response = request.GetResponse();
            try
            {
                using (var responseStream = response.GetResponseStream())
                {
                    try
                    {
                        var responseReader = new StreamReader(responseStream, Encoding.UTF8);
                        try
                        {
                            page = responseReader.ReadToEnd();
                        }
                        finally
                        {
                            responseReader.Close();
                        }
                    }
                    finally
                    {
                        responseStream.Close();
                    }
                }
            }
            finally
            {
                response.Close();
                observer.NotifyProgress(this, 100);
            }

            return page;
        }

        protected static Stream OuvrirFlux(WebRequest request)
        {
            MemoryStream ms;

            var response = request.GetResponse();
            try
            {
                using (var responseStream = response.GetResponseStream())
                {
                    try
                    {
                        ms = new MemoryStream(0x1F000);
                        var buffer = new byte[0x1000];
                        int bytes;
                        while ((bytes = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                            ms.Write(buffer, 0, bytes);
                    }

                    finally
                    {
                        responseStream.Flush();
                        responseStream.Close();
                    }
                }
            }
            finally
            {
                response.Close();
            }
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}