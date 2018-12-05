using System;

namespace CabOnline.Model.Parution.Generic
{
    internal abstract class P�riodicit�Base : IP�riodicit�
    {
        public override string ToString()
        {
            return Libell�Court;
        }

        public abstract string Libell�Court { get; }
        public abstract string Libell�Long { get; }
        public abstract TypeP�riodicit� TypeP�riodicit� { get; }

        public int CompareTo(ICaract�ristiqueScalaire caract�ristiqueScalaire)
        {
            var p�riodicit�Lgp = caract�ristiqueScalaire as IP�riodicit�;
            if (p�riodicit�Lgp == null)
                throw new ArgumentException("caract�ristiqueScalaire doit �tre une P�riodicit� non null",
                                            nameof(caract�ristiqueScalaire));

            switch (TypeP�riodicit�)
            {
                case TypeP�riodicit�.Inconnue:
                    return 0;
                case TypeP�riodicit�.Mensuelle:
                    switch (p�riodicit�Lgp.TypeP�riodicit�)
                    {
                        case TypeP�riodicit�.Inconnue:
                            return 0;
                        case TypeP�riodicit�.Mensuelle:
                            return 0;
                        case TypeP�riodicit�.Bimestrielle:
                        case TypeP�riodicit�.BiPuisTri:
                        case TypeP�riodicit�.Trimestrielle:
                        case TypeP�riodicit�.Irr�guli�re:
                            return -1;
                        default:
                            throw new ArgumentException("P�riodicit� inconnue", nameof(caract�ristiqueScalaire));
                    }
                case TypeP�riodicit�.Bimestrielle:
                    switch (p�riodicit�Lgp.TypeP�riodicit�)
                    {
                        case TypeP�riodicit�.Inconnue:
                            return 0;
                        case TypeP�riodicit�.Mensuelle:
                            return 1;
                        case TypeP�riodicit�.Bimestrielle:
                            return 0;
                        case TypeP�riodicit�.BiPuisTri:
                        case TypeP�riodicit�.Trimestrielle:
                        case TypeP�riodicit�.Irr�guli�re:
                            return -1;
                        default:
                            throw new ArgumentException("P�riodicit� inconnue", nameof(caract�ristiqueScalaire));
                    }
                case TypeP�riodicit�.BiPuisTri:
                case TypeP�riodicit�.Trimestrielle:
                    switch (p�riodicit�Lgp.TypeP�riodicit�)
                    {
                        case TypeP�riodicit�.Inconnue:
                            return 0;
                        case TypeP�riodicit�.Mensuelle:
                        case TypeP�riodicit�.Bimestrielle:
                            return 1;
                        case TypeP�riodicit�.BiPuisTri:
                        case TypeP�riodicit�.Trimestrielle:
                            return 0;
                        case TypeP�riodicit�.Irr�guli�re:
                            return -1;
                        default:
                            throw new ArgumentException("P�riodicit� inconnue", nameof(caract�ristiqueScalaire));
                    }
                case TypeP�riodicit�.Irr�guli�re:
                    switch (p�riodicit�Lgp.TypeP�riodicit�)
                    {
                        case TypeP�riodicit�.Inconnue:
                            return 0;
                        case TypeP�riodicit�.Mensuelle:
                        case TypeP�riodicit�.Bimestrielle:
                        case TypeP�riodicit�.BiPuisTri:
                        case TypeP�riodicit�.Trimestrielle:
                            return 1;
                        case TypeP�riodicit�.Irr�guli�re:
                            return 0;
                        default:
                            throw new ArgumentException("P�riodicit� inconnue", nameof(caract�ristiqueScalaire));
                    }
                default:
                    throw new ArgumentException("P�riodicit� inconnue", nameof(caract�ristiqueScalaire));
            }
        }
    }
}