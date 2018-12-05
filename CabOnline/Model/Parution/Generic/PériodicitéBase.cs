using System;

namespace CabOnline.Model.Parution.Generic
{
    internal abstract class PériodicitéBase : IPériodicité
    {
        public override string ToString()
        {
            return LibelléCourt;
        }

        public abstract string LibelléCourt { get; }
        public abstract string LibelléLong { get; }
        public abstract TypePériodicité TypePériodicité { get; }

        public int CompareTo(ICaractéristiqueScalaire caractéristiqueScalaire)
        {
            var périodicitéLgp = caractéristiqueScalaire as IPériodicité;
            if (périodicitéLgp == null)
                throw new ArgumentException("caractéristiqueScalaire doit être une Périodicité non null",
                                            nameof(caractéristiqueScalaire));

            switch (TypePériodicité)
            {
                case TypePériodicité.Inconnue:
                    return 0;
                case TypePériodicité.Mensuelle:
                    switch (périodicitéLgp.TypePériodicité)
                    {
                        case TypePériodicité.Inconnue:
                            return 0;
                        case TypePériodicité.Mensuelle:
                            return 0;
                        case TypePériodicité.Bimestrielle:
                        case TypePériodicité.BiPuisTri:
                        case TypePériodicité.Trimestrielle:
                        case TypePériodicité.Irrégulière:
                            return -1;
                        default:
                            throw new ArgumentException("Périodicité inconnue", nameof(caractéristiqueScalaire));
                    }
                case TypePériodicité.Bimestrielle:
                    switch (périodicitéLgp.TypePériodicité)
                    {
                        case TypePériodicité.Inconnue:
                            return 0;
                        case TypePériodicité.Mensuelle:
                            return 1;
                        case TypePériodicité.Bimestrielle:
                            return 0;
                        case TypePériodicité.BiPuisTri:
                        case TypePériodicité.Trimestrielle:
                        case TypePériodicité.Irrégulière:
                            return -1;
                        default:
                            throw new ArgumentException("Périodicité inconnue", nameof(caractéristiqueScalaire));
                    }
                case TypePériodicité.BiPuisTri:
                case TypePériodicité.Trimestrielle:
                    switch (périodicitéLgp.TypePériodicité)
                    {
                        case TypePériodicité.Inconnue:
                            return 0;
                        case TypePériodicité.Mensuelle:
                        case TypePériodicité.Bimestrielle:
                            return 1;
                        case TypePériodicité.BiPuisTri:
                        case TypePériodicité.Trimestrielle:
                            return 0;
                        case TypePériodicité.Irrégulière:
                            return -1;
                        default:
                            throw new ArgumentException("Périodicité inconnue", nameof(caractéristiqueScalaire));
                    }
                case TypePériodicité.Irrégulière:
                    switch (périodicitéLgp.TypePériodicité)
                    {
                        case TypePériodicité.Inconnue:
                            return 0;
                        case TypePériodicité.Mensuelle:
                        case TypePériodicité.Bimestrielle:
                        case TypePériodicité.BiPuisTri:
                        case TypePériodicité.Trimestrielle:
                            return 1;
                        case TypePériodicité.Irrégulière:
                            return 0;
                        default:
                            throw new ArgumentException("Périodicité inconnue", nameof(caractéristiqueScalaire));
                    }
                default:
                    throw new ArgumentException("Périodicité inconnue", nameof(caractéristiqueScalaire));
            }
        }
    }
}