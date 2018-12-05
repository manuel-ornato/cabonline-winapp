using System;
using System.Collections.ObjectModel;
using System.Deployment.Application;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using CabOnline.Extensions.Exception;
using CabOnline.Operators;
using CabOnline.TaskObserver;
using MvvmFoundation.Wpf;

namespace CabOnline.ViewModel
{
    internal class MainViewModel : ViewModel, IAfficheurT�ches
    {
        public MainViewModel(IOp�rateurPrincipal op�rateurPrincipal,
                             OptionsViewModel optionsViewModel, S�lectionViewModel s�lectionViewModel,
                             RapportViewModel rapportViewModel,
                             DateTime trancheDhlMin, DateTime trancheDhlMax)
        {
            _op�rateurPrincipal = op�rateurPrincipal ?? throw new ArgumentNullException(nameof(op�rateurPrincipal));
            _trancheDhlMin = trancheDhlMin;
            _trancheDhlMax = trancheDhlMax;
            Options = optionsViewModel ?? throw new ArgumentNullException(nameof(optionsViewModel));
            S�lection = s�lectionViewModel ?? throw new ArgumentNullException(nameof(s�lectionViewModel));
            Rapport = rapportViewModel ?? throw new ArgumentNullException(nameof(rapportViewModel));
            T�ches = new ObservableCollection<T�cheViewModel>();
            EstPr�t = false;
        }

        public string TitreApplication =>
            $"Gestion des Cabs v. {(ApplicationDeployment.IsNetworkDeployed ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() : Assembly.GetExecutingAssembly().GetName().Version.ToString())}";

        public bool EstPr�t
        {
            get => _estPr�t;
            set
            {
                if (value != _estPr�t)
                {
                    _estPr�t = value;
                    RaisePropertyChangedOnUiThread("EstPr�t");
                    DoOnUiThread(CommandManager.InvalidateRequerySuggested);
                }
            }
        }

        public bool AT�ches => T�ches.Count > 0;
        public OptionsViewModel Options { get; private set; }
        public S�lectionViewModel S�lection { get; private set; }
        public RapportViewModel Rapport { get; private set; }
        public ObservableCollection<T�cheViewModel> T�ches { get; private set; }

        public ICommand ChargerPlanningCommand => new RelayCommand(ChargerPlanningAsync, PeutChargerPlanning);

        public ICommand V�rifierEtT�l�chargerCabsCommand => new RelayCommand(V�rifierEtT�l�chargerCabsAsync, PeutV�rifierEtT�l�chargerCabs);

        public void AjouterT�che(string nom, ITaskObserver observer)
        {
            var t�cheViewModel = new T�cheViewModel(nom, observer);
            DoOnUiThread(() => T�ches.Add(t�cheViewModel));
            RaisePropertyChangedOnUiThread("AT�ches");
        }

        private readonly IOp�rateurPrincipal _op�rateurPrincipal;
        private readonly DateTime _trancheDhlMax;
        private readonly DateTime _trancheDhlMin;
        private bool _estPr�t;

        private bool PeutChargerPlanning()
        {
            return EstPr�t;
        }

        private void ChargerPlanningAsync()
        {
            EstPr�t = false;
            _op�rateurPrincipal.ChargerPlanningAsync(this)
                .ContinueWith(
                    (t) =>
                        {
                            EstPr�t = true;
                            if (t.IsFaulted)
                                MessageBox.Show($"Erreur : {t.Exception.DetailedMessage()}",
                                                "Erreur durant l'op�ration");
                        });
        }

        private bool PeutV�rifierEtT�l�chargerCabs()
        {
            return EstPr�t && Options.T�l�chargerCab;
        }

        private void V�rifierEtT�l�chargerCabsAsync()
        {
            EstPr�t = false;
            _op�rateurPrincipal.V�rifierEtT�l�chargerCabsAsync(_trancheDhlMin, _trancheDhlMax,
                                                               S�lection,
                                                               Options,
                                                               this)
                .ContinueWith(
                    t =>
                        {
                            if (t.IsFaulted)
                                MessageBox.Show($"Erreur : {t.Exception.DetailedMessage()}",
                                                "Erreur durant l'op�ration");
                            else if (t.IsCompleted)
                                Rapport.Assigner(t.Result);
                            EstPr�t = true;
                        });
        }
    }
}