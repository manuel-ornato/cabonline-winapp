using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CabOnline.Model.Cab;
using CabOnline.Model.Parution;
using CabOnline.TaskObserver;

namespace CabOnline.Model.D�p�t
{
    internal abstract class D�p�tCab : ID�p�tCab
    {
        protected readonly ICabFactory CabFactory;
        protected readonly ID�p�tParutions D�p�tParutions;

        protected D�p�tCab(ICabFactory cabFactory, ID�p�tParutions d�p�tParutions)
        {
            CabFactory = cabFactory ?? throw new ArgumentNullException(nameof(cabFactory));
            D�p�tParutions = d�p�tParutions ?? throw new ArgumentNullException(nameof(d�p�tParutions));
        }

        public override string ToString()
        {
            return Libell�Court;
        }

        public abstract string Libell�Court { get; }
        public virtual string Libell�Long => Libell�Court;

        public abstract void ChargerCabs(ITaskObserver observer);

        public virtual ICab Cab(IParution parution)
        {
            var donn�esCabParution = from dc in Donn�esCab()
                                     where (dc.Codif == parution.Codif) && (dc.Num�ro == parution.Num�ro)
                                     orderby dc.DateCr�ation descending
                                     select dc;
            return donn�esCabParution.FirstOrDefault();
        }

        public abstract Stream ObtenirContenu(ICab cab);

        protected abstract IEnumerable<ICab> Donn�esCab();
    }
}