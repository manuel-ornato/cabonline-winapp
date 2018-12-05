using System.Collections.Generic;
using System.ComponentModel;
using CabOnline.Model.Parution;
using CabOnline.TaskObserver;

namespace CabOnline.Model.Dépôt
{
    internal interface IDépôtParutions : INotifyPropertyChanged
    {
        void ChargerParutions(ITaskObserver taskObserver);
        IEnumerable<IParution> Parutions { get; }
        IEnumerable<IPôle> Pôles { get; }
        IDistributeur Distributeur(TypeDistributeur typeDistributeur);
        IEditeur Editeur(TypeEditeur typeEditeur);
        IPériodicité Périodicité(TypePériodicité typePériodicité);
        IQualificationRéseau QualificationRéseau(TypeQualificationRéseau typeQualificationRéseau);
        IPôle Pôle(string codePôle);
    }
}