using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CabOnline.Model.Cab;
using CabOnline.Model.Parution;
using CabOnline.TaskObserver;

namespace CabOnline.Model.Dépôt
{
    internal abstract class DépôtCab : IDépôtCab
    {
        protected readonly ICabFactory CabFactory;
        protected readonly IDépôtParutions DépôtParutions;

        protected DépôtCab(ICabFactory cabFactory, IDépôtParutions dépôtParutions)
        {
            CabFactory = cabFactory ?? throw new ArgumentNullException(nameof(cabFactory));
            DépôtParutions = dépôtParutions ?? throw new ArgumentNullException(nameof(dépôtParutions));
        }

        public override string ToString()
        {
            return LibelléCourt;
        }

        public abstract string LibelléCourt { get; }
        public virtual string LibelléLong => LibelléCourt;

        public abstract void ChargerCabs(ITaskObserver observer);

        public virtual ICab Cab(IParution parution)
        {
            var donnéesCabParution = from dc in DonnéesCab()
                                     where (dc.Codif == parution.Codif) && (dc.Numéro == parution.Numéro)
                                     orderby dc.DateCréation descending
                                     select dc;
            return donnéesCabParution.FirstOrDefault();
        }

        public abstract Stream ObtenirContenu(ICab cab);

        protected abstract IEnumerable<ICab> DonnéesCab();
    }
}