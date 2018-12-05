using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CabOnline.Model.Parution;
using CabOnline.Operators;

namespace CabOnline.ViewModel
{
    internal class RapportViewModel : ViewModel
    {
        public void Assigner(IEnumerable<InfosTraitementParution> infosParutions)
        {
            _infosParutions = new ObservableCollection<InfosTraitementParution>(infosParutions);
            RaisePropertyChangedOnUiThread("InfosParutions");
        }

        public ObservableCollection<InfosTraitementParution> InfosParutions => _infosParutions;
        private ObservableCollection<InfosTraitementParution> _infosParutions = new ObservableCollection<InfosTraitementParution>();
    }
}