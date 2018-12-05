using System;

namespace CabOnline.Model.Parution.Generic
{
    internal abstract class QualificationRéseauBase : IQualificationRéseau
    {
        public abstract TypeQualificationRéseau TypeQualificationRéseau { get; }
        public abstract string LibelléCourt { get; }
        public abstract string LibelléLong { get; }

        public int CompareTo(ICaractéristiqueScalaire caractéristiqueScalaire)
        {
            var qualif = caractéristiqueScalaire as IQualificationRéseau;
            if (qualif == null)
                throw new ArgumentException("caractéristiqueScalaire doit être une QualificationRéseau non nulle",
                                            nameof(caractéristiqueScalaire));

            if (TypeQualificationRéseau == TypeQualificationRéseau.Inconnu ||
                qualif.TypeQualificationRéseau == TypeQualificationRéseau.Inconnu)
                return 0;

            return LibelléCourt.CompareTo(qualif.LibelléCourt);
        }

        public override string ToString()
        {
            return LibelléCourt;
        }
    }
}