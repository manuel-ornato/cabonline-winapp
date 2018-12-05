using System;
using System.Collections.Generic;
using System.Linq;
using CabOnline.Model.AnomalieCab;
using CabOnline.Model.Cab;
using CabOnline.Model.Dépôt;
using CabOnline.Model.Parution;
using CabOnline.ViewModel;

namespace CabOnline.Operators
{
    internal class InfosTraitementParution : IInfosTraitementParution
    {
        public InfosTraitementParution(IParution parution, ICab cab, IInfosOpérationDépôt infosOpérationDépôt)
        {
            Parution = parution ?? throw new ArgumentNullException(nameof(parution));
            Cab = cab;
            InfosOpérationDépôt = infosOpérationDépôt;
            if (cab != null)
                _anomalies = cab.Anomalies.ToDictionary(a => a.Type);
            else
                _anomalies = new Dictionary<TypeAnomalieCab, IAnomalieCab>();
        }

        public DateTime? DateDhl => Parution.DateDhl;

        public string CodeProduit => Parution.CodeProduit;

        public string TitreEtNuméro => $"{Parution.Titre} n°{Parution.Numéro}";

        public EtatCab EtatCab
        {
            get
            {
                if (Cab == null)
                    return EtatCab.Absent;
                if (!Cab.EstCompatible)
                    return EtatCab.Incompatible;
                if (Cab.NécessiteCorrection)
                    return EtatCab.ActionNécessaire;
                if (Cab.Anomalies.Count() > 0)
                    return EtatCab.Anormal;

                return EtatCab.Ok;
            }
        }

        public EtatCabOnline EtatCabOnline
        {
            get
            {
                if (InfosOpérationDépôt != null)
                    return InfosOpérationDépôt.EtatCabOnline;
                else
                    return EtatCabOnline.Indisponible;
            }
        }

        public bool CabAbsent => Cab == null;

        public IAnomalieCab AnomalieEditeur
        {
            get
            {
                IAnomalieCab a;
                if (_anomalies.TryGetValue(TypeAnomalieCab.Editeur, out a))
                    return a;
                return null;
            }
        }

        public bool EditeurEstAnormal => AnomalieEditeur != null;

        public IAnomalieCab AnomalieDistributeur
        {
            get
            {
                IAnomalieCab a;
                if (_anomalies.TryGetValue(TypeAnomalieCab.Distributeur, out a))
                    return a;
                return null;
            }
        }

        public bool DistributeurEstAnormal => AnomalieDistributeur != null;

        public IAnomalieCab AnomalieCodif
        {
            get
            {
                IAnomalieCab a;
                if (_anomalies.TryGetValue(TypeAnomalieCab.Codif, out a))
                    return a;
                return null;
            }
        }

        public bool CodifEstAnormale => AnomalieCodif != null;

        public IAnomalieCab AnomaliePrix
        {
            get
            {
                IAnomalieCab a;
                if (_anomalies.TryGetValue(TypeAnomalieCab.Prix, out a))
                    return a;
                return null;
            }
        }

        public bool PrixEstAnormal => AnomaliePrix != null;

        public IAnomalieCab AnomaliePériodicité
        {
            get
            {
                IAnomalieCab a;
                if (_anomalies.TryGetValue(TypeAnomalieCab.Périodicité, out a))
                    return a;
                return null;
            }
        }

        public bool PériodicitéEstAnormale => AnomaliePériodicité != null;

        public IAnomalieCab AnomalieQualification
        {
            get
            {
                IAnomalieCab a;
                if (_anomalies.TryGetValue(TypeAnomalieCab.Qualification, out a))
                    return a;
                return null;
            }
        }

        public bool QualificationEstAnormale => AnomalieQualification != null;

        public IAnomalieCab AnomalieNuméro
        {
            get
            {
                IAnomalieCab a;
                if (_anomalies.TryGetValue(TypeAnomalieCab.Numéro, out a))
                    return a;
                return null;
            }
        }

        public bool NuméroEstAnormal => AnomalieNuméro != null;
        public IParution Parution { get; private set; }

        public ICab Cab { get; private set; }

        public ICab CabOnline
        {
            get
            {
                if (InfosOpérationDépôt != null)
                    return InfosOpérationDépôt.CabSource;
                else
                    return null;
            }
        }

        public bool OnlineEstAnormal => (AnomaliesOnline != null && AnomaliesOnline.Count() > 0);

        public IEnumerable<IAnomalieCab> AnomaliesOnline
        {
            get
            {
                if (CabOnline != null)
                    return CabOnline.Anomalies;
                else
                    return null;
            }
        }

        public IInfosOpérationDépôt InfosOpérationDépôt { get; private set; }

        public override string ToString()
        {
            return
                $"{(Parution.DateDhl.HasValue ? Parution.DateDhl.Value.ToShortDateString() : "00/00/00")} - {Parution.CodeSérie}{(Parution.Numéro.HasValue ? Parution.Numéro.Value.ToString("#00") : "00")} - {Parution.Titre} : [{(Cab != null ? Cab.ToString() : "Pas de cab")}]";
        }

        private readonly Dictionary<TypeAnomalieCab, IAnomalieCab> _anomalies;
    }
}