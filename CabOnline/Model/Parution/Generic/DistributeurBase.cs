using System;

namespace CabOnline.Model.Parution.Generic
{
    internal abstract class DistributeurBase : IDistributeur
    {
        public abstract TypeDistributeur TypeDistributeur { get; }
        public abstract string Libell�Court { get; }

        public abstract string Libell�Long { get; }

        public int CompareTo(ICaract�ristiqueScalaire caract�ristiqueScalaire)
        {
            var distributeur = caract�ristiqueScalaire as IDistributeur;
            if (distributeur == null)
                throw new ArgumentException("caract�ristiqueScalaire doit �tre un Distributeur non null",
                                            nameof(caract�ristiqueScalaire));

            if (TypeDistributeur == TypeDistributeur.Inconnu ||
                distributeur.TypeDistributeur == TypeDistributeur.Inconnu)
                return 0;

            return Libell�Court.CompareTo(distributeur.Libell�Court);
        }

        public override string ToString()
        {
            return Libell�Court;
        }
    }
}