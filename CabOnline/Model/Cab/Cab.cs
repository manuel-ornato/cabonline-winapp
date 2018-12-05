using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CabOnline.Model.AnomalieCab;
using CabOnline.Model.ComparateurCabParution;
using CabOnline.Model.Dépôt;
using CabOnline.Model.Parution;

namespace CabOnline.Model.Cab
{
    internal sealed class Cab : ICab
    {
        public Cab(IComparateurCabParution comparateurCabParution, IDépôtCab dépôtCab, string url, IEditeur éditeur, IDistributeur distributeur, string codif, int? numéro, decimal? prix, IPériodicité périodicité, IQualificationRéseau qualif, DateTime dateCréation, IParution parutionCible)
        {
            _comparateurCabParution = comparateurCabParution ?? throw new ArgumentNullException(nameof(comparateurCabParution));
            _dépôtCab = dépôtCab ?? throw new ArgumentNullException(nameof(dépôtCab));
            Url = url;
            Editeur = éditeur ?? throw new ArgumentNullException(nameof(éditeur));
            Distributeur = distributeur ?? throw new ArgumentNullException(nameof(distributeur));
            Codif = codif;
            Numéro = numéro;
            Prix = prix;
            Périodicité = périodicité ?? throw new ArgumentNullException(nameof(périodicité));
            Qualif = qualif ?? throw new ArgumentNullException(nameof(qualif));
            DateCréation = dateCréation;
            ParutionCible = parutionCible ?? throw new ArgumentNullException(nameof(parutionCible));
        }

        public override string ToString()
        {
            return LibelléCourt;
        }

        public Stream ObtenirContenu()
        {
            return _dépôtCab.ObtenirContenu(this);
        }

        public IDépôtCab Dépôt => _dépôtCab;
        public IParution ParutionCible { get; private set; }

        public IEditeur Editeur { get; private set; }

        public IDistributeur Distributeur { get; private set; }

        public string Codif { get; private set; }

        public int? Numéro { get; private set; }

        public decimal? Prix { get; private set; }

        public IPériodicité Périodicité { get; private set; }

        public IQualificationRéseau Qualif { get; private set; }

        public string Url { get; private set; }

        public DateTime DateCréation { get; private set; }

        public bool EstCompatible
        {
            get 
            { 
                bool estCompatible;
                IEnumerable<IAnomalieCab> anomalies;
                _comparateurCabParution.CalculerCompatbilité(this, out estCompatible, out anomalies);
                return estCompatible;
            }
        }

        public bool NécessiteCorrection
        {
            get { return Anomalies.Any(a => a.NécessiteAction); }
        }

        public IEnumerable<IAnomalieCab> Anomalies
        {
            get
            {
                bool estCompatible;
                IEnumerable<IAnomalieCab> anomalies;
                _comparateurCabParution.CalculerCompatbilité(this, out estCompatible, out anomalies);
                return anomalies;
            }
        }
        
        public string LibelléCourt => $"{Dépôt}|{Codif}|{Qualif}";

        public string LibelléLong => $"{Dépôt}|{Codif}|{Qualif}|{Url}|{DateCréation.ToShortDateString()}";
        private readonly IComparateurCabParution _comparateurCabParution;
        private readonly IDépôtCab _dépôtCab;
    }
}