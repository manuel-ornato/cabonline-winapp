namespace CabOnline.Model.Parution.BdiEF
{
    using Generic;

    internal class DistributeurBdi : DistributeurBase
    {
        private const string _Libell�Inconnu = "Dist A Def";
        private const string _Libell�Coedis = "COEDIS";
        private const string _Libell�Sofedis = "SOFEDIS";
        private const string _Libell�Eyrolles = "EYR";
        private const string _Libell�Mepe = "MEPE";
        private const string _Libell�MarketForce = "MF";
        private const string _Libell�Mlp = "MLP";
        private const string _Libell�Nmpp = "NMPP";
        private const string _Libell�Tp = "TP";
        private const string _Libell�VerlagsUnion = "VU";
        private const string _Libell�Web = "WEB";
        private readonly string _libell�;
        private readonly string _raisonSociale;

        public DistributeurBdi(string libell�, string raisonSociale)
        {
            _libell� = libell�;
            _raisonSociale = raisonSociale;
        }

        public override TypeDistributeur TypeDistributeur
        {
            get
            {
                switch (_libell�)
                {
                    case _Libell�Coedis:
                        return TypeDistributeur.Coedis;
                    case _Libell�Sofedis:
                        return TypeDistributeur.Sofedis;
                    case _Libell�Eyrolles:
                        return TypeDistributeur.Eyrolles;
                    case _Libell�Mepe:
                        return TypeDistributeur.Mepe;
                    case _Libell�MarketForce:
                        return TypeDistributeur.MarketForce;
                    case _Libell�Mlp:
                        return TypeDistributeur.Mlp;
                    case _Libell�Nmpp:
                        return TypeDistributeur.Nmpp;
                    case _Libell�Tp:
                        return TypeDistributeur.Tp;
                    case _Libell�VerlagsUnion:
                        return TypeDistributeur.VerlagsUnion;
                    case _Libell�Web:
                        return TypeDistributeur.Web;
                    case _Libell�Inconnu:
                    default:
                        return TypeDistributeur.Inconnu;
                }
            }
        }

        public override string ToString()
        {
            return Libell�Court;
        }

        public override string Libell�Court => _libell�;

        public override string Libell�Long => _raisonSociale;
    }
}