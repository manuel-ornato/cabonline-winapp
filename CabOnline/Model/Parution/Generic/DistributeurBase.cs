using System;

namespace CabOnline.Model.Parution.Generic
{
    internal abstract class DistributeurBase : IDistributeur
    {
        public abstract TypeDistributeur TypeDistributeur { get; }
        public abstract string LibelléCourt { get; }

        public abstract string LibelléLong { get; }

        public int CompareTo(ICaractéristiqueScalaire caractéristiqueScalaire)
        {
            var distributeur = caractéristiqueScalaire as IDistributeur;
            if (distributeur == null)
                throw new ArgumentException("caractéristiqueScalaire doit être un Distributeur non null",
                                            nameof(caractéristiqueScalaire));

            if (TypeDistributeur == TypeDistributeur.Inconnu ||
                distributeur.TypeDistributeur == TypeDistributeur.Inconnu)
                return 0;

            return LibelléCourt.CompareTo(distributeur.LibelléCourt);
        }

        public override string ToString()
        {
            return LibelléCourt;
        }
    }
}