
namespace CabOnline.Model.Dépôt.Mlp
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Parution;

    internal sealed class CompteCab
    {
        public CompteCab(TypeEditeur editeur, string identifiant, string motDePasse, string dossier)
        {
            Editeur = editeur;
            Identifiant = identifiant ?? throw new ArgumentNullException(nameof(identifiant));
            MotDePasse = motDePasse ?? throw new ArgumentNullException(nameof(motDePasse));
            Dossier = dossier ?? throw new ArgumentNullException(nameof(dossier));
        }

        public TypeEditeur Editeur { get; }
        public string Identifiant { get; }
        public string MotDePasse { get; }
        public string Dossier { get; }
    }

    internal sealed class Configuration
    {
        public Configuration(string url, string dossierRacine, IEnumerable<CompteCab> comptes)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            DossierRacine = dossierRacine ?? throw new ArgumentNullException(nameof(dossierRacine));
            Comptes = comptes?.ToImmutableArray() ?? throw new ArgumentNullException(nameof(comptes));
        }

        public IEnumerable<CompteCab> Comptes { get; }
        public string Url { get; }
        public string DossierRacine { get; }
    }
}
