using System;
using System.Collections.Generic;
using System.Linq;
using CabOnline.Model.Dépôt;
using CabOnline.ViewModel;

namespace CabOnline.Operators.RapportMail
{
    internal class InfosParutionStAdapter
    {
        private readonly IInfosTraitementParution _ipvm;

        public InfosParutionStAdapter(IInfosTraitementParution ipvm)
        {
            _ipvm = ipvm ?? throw new ArgumentNullException(nameof(ipvm));
        }

        public DateTime? DateDhl => _ipvm.DateDhl;

        public string CodeProduit => _ipvm.CodeProduit;

        public string TitreEtNumero => _ipvm.TitreEtNuméro;

        public EtatCab EtatCab => _ipvm.EtatCab;

        public EtatCabOnline EtatCabOnline => _ipvm.EtatCabOnline;

        public bool CabAbsent => _ipvm.CabAbsent;

        public bool OnlineEstAnormal => _ipvm.OnlineEstAnormal;
        public string Codif => _ipvm.Parution.Codif;
        public string Editeur => _ipvm.Parution.Editeur.ToString();
        public string Distributeur => _ipvm.Parution.Distributeur.ToString();

        public IEnumerable<AnomalieCabStAdapter> AnomaliesOnline
        {
            get
            {
                if (_ipvm.AnomaliesOnline != null)
                    return _ipvm.AnomaliesOnline.Select(a => new AnomalieCabStAdapter(a));
                else
                    return new List<AnomalieCabStAdapter>();
            }
        }

        public IEnumerable<AnomalieCabStAdapter> Anomalies
        {
            get
            {
                if (_ipvm.Cab != null)
                    return _ipvm.Cab.Anomalies.Select(a => new AnomalieCabStAdapter(a));
                else
                    return new List<AnomalieCabStAdapter>();
            }
        }

        public string Url => _ipvm.Cab != null ? _ipvm.Cab.Url : null;
        public string UrlOnline => _ipvm.CabOnline != null ? _ipvm.CabOnline.Url : null;

        public override string ToString()
        {
            return _ipvm.ToString();
        }
    }
}