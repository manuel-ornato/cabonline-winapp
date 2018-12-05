namespace CabOnline.Model.Parution.BdiEF
{
    using Generic;

    internal class EditeurBdi : EditeurBase
    {
        private const string _LibelléCyberPress = "CYBER";
        private const string _LibelléEsi = "ESI";
        private const string _LibelléGo = "GO";
        private readonly string _libellé;
        private readonly string _raisonSociale;

        public EditeurBdi(string libellé, string raisonSociale)
        {
            _libellé = libellé;
            _raisonSociale = raisonSociale;
        }

        public override TypeEditeur TypeEditeur
        {
            get
            {
                switch (_libellé)
                {
                    case _LibelléEsi:
                        return TypeEditeur.Esi;
                    case _LibelléGo:
                        return TypeEditeur.Go;
                    case _LibelléCyberPress:
                        return TypeEditeur.Cyber;
                    default:
                        return TypeEditeur.Inconnu;
                }
            }
        }

        public override string LibelléCourt => _libellé;

        public override string LibelléLong => _raisonSociale;
    }
}