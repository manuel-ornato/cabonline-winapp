namespace CabOnline.Operators.Tiff
{
    using System;
    using System.IO;
    using ImageMagick;

    internal class ConvertisseurEpsVersTiff : IConvertisseurEpsVersTiff
    {
        static ConvertisseurEpsVersTiff()
        {
            MagickNET.SetGhostscriptDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"GS"));
        }

        public void Convertir(string cheminSource, string cheminCible)
        {
            var readSettings = new MagickReadSettings {UseMonochrome = true, Density = new Density(1200, 1200)};
            using (var image = new MagickImage(cheminSource, readSettings))
            {
                image.Format = MagickFormat.Tiff;
                image.Write(cheminCible);
            }
        }
    }
}