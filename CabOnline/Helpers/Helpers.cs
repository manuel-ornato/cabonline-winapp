using System;
using System.IO;

namespace CabOnline.Helpers
{
    internal static class Helpers
    {
        public static string CodeProduit(string codeInterne, int? numéro)
        {
            return $"{codeInterne}{(numéro ?? 0).ToString("#00")}";
        }

        public static void EffacerContenuDossier(string cheminDossier, string pattern)
        {
            var di = new DirectoryInfo(cheminDossier);
            foreach (var fi in di.GetFiles(pattern, SearchOption.TopDirectoryOnly))
                try
                {
                    fi.Delete();
                }
                catch
                {
                }
        }
    }
}