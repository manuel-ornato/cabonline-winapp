using System;

namespace CabOnline.Model.Parution.Generic
{
    internal abstract class EditeurBase: IEditeur
    {
        public abstract string Libell�Court { get; }
        public abstract string Libell�Long { get; }
        public abstract TypeEditeur TypeEditeur { get; }
        public int CompareTo(ICaract�ristiqueScalaire caract�ristiqueScalaire)
        {
            var �diteur = caract�ristiqueScalaire as IEditeur;
            if (�diteur == null)
                throw new ArgumentException("caract�ristiqueScalaire doit �tre un Editeur non null",
                                            nameof(caract�ristiqueScalaire));

            if (TypeEditeur == TypeEditeur.Inconnu || TypeEditeur == TypeEditeur.Na)
                return 0;

            if (�diteur.TypeEditeur == TypeEditeur.Inconnu || �diteur.TypeEditeur == TypeEditeur.Na)
                return 0;

            return Libell�Court.CompareTo(�diteur.Libell�Court);
        }

        public override string ToString()
        {
            return Libell�Court;
        }
    }
}