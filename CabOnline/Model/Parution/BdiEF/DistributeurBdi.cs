namespace CabOnline.Model.Parution.BdiEF
{
    using Generic;

    internal class DistributeurBdi : DistributeurBase
    {
        private const string _LibelléInconnu = "Dist A Def";
        private const string _LibelléCoedis = "COEDIS";
        private const string _LibelléSofedis = "SOFEDIS";
        private const string _LibelléEyrolles = "EYR";
        private const string _LibelléMepe = "MEPE";
        private const string _LibelléMarketForce = "MF";
        private const string _LibelléMlp = "MLP";
        private const string _LibelléNmpp = "NMPP";
        private const string _LibelléTp = "TP";
        private const string _LibelléVerlagsUnion = "VU";
        private const string _LibelléWeb = "WEB";
        private readonly string _libellé;
        private readonly string _raisonSociale;

        public DistributeurBdi(string libellé, string raisonSociale)
        {
            _libellé = libellé;
            _raisonSociale = raisonSociale;
        }

        public override TypeDistributeur TypeDistributeur
        {
            get
            {
                switch (_libellé)
                {
                    case _LibelléCoedis:
                        return TypeDistributeur.Coedis;
                    case _LibelléSofedis:
                        return TypeDistributeur.Sofedis;
                    case _LibelléEyrolles:
                        return TypeDistributeur.Eyrolles;
                    case _LibelléMepe:
                        return TypeDistributeur.Mepe;
                    case _LibelléMarketForce:
                        return TypeDistributeur.MarketForce;
                    case _LibelléMlp:
                        return TypeDistributeur.Mlp;
                    case _LibelléNmpp:
                        return TypeDistributeur.Nmpp;
                    case _LibelléTp:
                        return TypeDistributeur.Tp;
                    case _LibelléVerlagsUnion:
                        return TypeDistributeur.VerlagsUnion;
                    case _LibelléWeb:
                        return TypeDistributeur.Web;
                    case _LibelléInconnu:
                    default:
                        return TypeDistributeur.Inconnu;
                }
            }
        }

        public override string ToString()
        {
            return LibelléCourt;
        }

        public override string LibelléCourt => _libellé;

        public override string LibelléLong => _raisonSociale;
    }
}