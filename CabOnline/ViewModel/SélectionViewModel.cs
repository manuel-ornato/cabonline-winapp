using System;
using System.Collections.Generic;
using CabOnline.Model.Dépôt;
using CabOnline.Model.Parution;
using CabOnline.Operators;


namespace CabOnline.ViewModel
{
    internal class SélectionViewModel : ViewModel, IOptionsSélectionParutions
    {
        private bool _filtrerParDate;
        private bool _filtrerParPôle;
        private bool _filtrerParCodeInterne;
        private bool _filtrerParNuméro;

        private DateTime? _dateDébut;
        private DateTime? _dateFin;
        private IPôle _pôle;
        private string _codeInterne;
        private int _numéro;

        private readonly IDépôtParutions _dépôtParutions;

        public SélectionViewModel(IDépôtParutions dépôtParutions)
        {
            _dépôtParutions = dépôtParutions ?? throw new ArgumentNullException(nameof(dépôtParutions));
            _dépôtParutions.PropertyChanged += 
                (s, e) =>
                {
                    if (e.PropertyName == "Pôles") 
                        RaisePropertyChangedOnUiThread("Pôles");
                };
        }

        public IEnumerable<IPôle> Pôles => _dépôtParutions.Pôles;

        public int Numéro
        {
            get => _numéro;
            set { _numéro = value; RaisePropertyChanged("Numéro"); }
        }

        public string CodeInterne
        {
            get => _codeInterne;
            set { _codeInterne = value; RaisePropertyChanged("CodeInterne"); }
        }

        public IPôle Pôle
        {
            get => _pôle;
            set { _pôle = value; RaisePropertyChanged("Pôle"); }
        }

        public DateTime? DateDébut
        {
            get => _dateDébut;
            set { _dateDébut = value; RaisePropertyChanged("DateDébut");}
        }

        public DateTime? DateFin
        {
            get => _dateFin;
            set { _dateFin = value; RaisePropertyChanged("DateFin"); }
        }

        public bool FiltrerParPôle
        {
            get => _filtrerParPôle;
            set
            {
                _filtrerParPôle = value;
                RaisePropertyChanged("FiltrerParPôle");
            }
        }

        public bool FiltrerParDate
        {
            get => _filtrerParDate;
            set
            {
                _filtrerParDate = value;
                RaisePropertyChanged("FiltrerParDate");
            }
        }

        public bool FiltrerParCodeInterne
        {
            get => _filtrerParCodeInterne;
            set
            {
                _filtrerParCodeInterne = value;
                RaisePropertyChanged("FiltrerParCodeInterne");
            }
        }

        public bool FiltrerParNuméro
        {
            get => _filtrerParNuméro;
            set
            {
                _filtrerParNuméro = value;
                RaisePropertyChanged("FiltrerParNuméro");
            }
        }
    }
}
