using System;

namespace CabOnline.Model.Parution.Generic
{
    internal abstract class PôleBase : IPôle
    {
        public abstract string LibelléCourt { get; }
        public abstract string LibelléLong { get; }

        public int CompareTo(ICaractéristiqueScalaire caractéristiqueScalaire)
        {
            var pôle = caractéristiqueScalaire as IPôle;
            if (pôle == null)
                throw new ArgumentException("caractéristiqueScalaire doit être un Editeur non null",
                                            nameof(caractéristiqueScalaire));

            return LibelléCourt.CompareTo(pôle.LibelléCourt);
        }

        public override string ToString()
        {
            return LibelléCourt;
        }
    }
}