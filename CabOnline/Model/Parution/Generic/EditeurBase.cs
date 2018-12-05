using System;

namespace CabOnline.Model.Parution.Generic
{
    internal abstract class EditeurBase: IEditeur
    {
        public abstract string LibelléCourt { get; }
        public abstract string LibelléLong { get; }
        public abstract TypeEditeur TypeEditeur { get; }
        public int CompareTo(ICaractéristiqueScalaire caractéristiqueScalaire)
        {
            var éditeur = caractéristiqueScalaire as IEditeur;
            if (éditeur == null)
                throw new ArgumentException("caractéristiqueScalaire doit être un Editeur non null",
                                            nameof(caractéristiqueScalaire));

            if (TypeEditeur == TypeEditeur.Inconnu || TypeEditeur == TypeEditeur.Na)
                return 0;

            if (éditeur.TypeEditeur == TypeEditeur.Inconnu || éditeur.TypeEditeur == TypeEditeur.Na)
                return 0;

            return LibelléCourt.CompareTo(éditeur.LibelléCourt);
        }

        public override string ToString()
        {
            return LibelléCourt;
        }
    }
}