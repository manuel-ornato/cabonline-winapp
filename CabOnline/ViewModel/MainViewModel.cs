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
    internal class MainViewModel : ViewModel, IAfficheurTâches
    {
        public MainViewModel(IOpérateurPrincipal opérateurPrincipal,
                             OptionsViewModel optionsViewModel, SélectionViewModel sélectionViewModel,
                             RapportViewModel rapportViewModel,
                             DateTime trancheDhlMin, DateTime trancheDhlMax)
        {
            _opérateurPrincipal = opérateurPrincipal ?? throw new ArgumentNullException(nameof(opérateurPrincipal));
            _trancheDhlMin = trancheDhlMin;
            _trancheDhlMax = trancheDhlMax;
            Options = optionsViewModel ?? throw new ArgumentNullException(nameof(optionsViewModel));
            Sélection = sélectionViewModel ?? throw new ArgumentNullException(nameof(sélectionViewModel));
            Rapport = rapportViewModel ?? throw new ArgumentNullException(nameof(rapportViewModel));
            Tâches = new ObservableCollection<TâcheViewModel>();
            EstPrêt = false;
        }

        public string TitreApplication =>
            $"Gestion des Cabs v. {(ApplicationDeployment.IsNetworkDeployed ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() : Assembly.GetExecutingAssembly().GetName().Version.ToString())}";

        public bool EstPrêt
        {
            get => _estPrêt;
            set
            {
                if (value != _estPrêt)
                {
                    _estPrêt = value;
                    RaisePropertyChangedOnUiThread("EstPrêt");
                    DoOnUiThread(CommandManager.InvalidateRequerySuggested);
                }
            }
        }

        public bool ATâches => Tâches.Count > 0;
        public OptionsViewModel Options { get; private set; }
        public SélectionViewModel Sélection { get; private set; }
        public RapportViewModel Rapport { get; private set; }
        public ObservableCollection<TâcheViewModel> Tâches { get; private set; }

        public ICommand ChargerPlanningCommand => new RelayCommand(ChargerPlanningAsync, PeutChargerPlanning);

        public ICommand VérifierEtTéléchargerCabsCommand => new RelayCommand(VérifierEtTéléchargerCabsAsync, PeutVérifierEtTéléchargerCabs);

        public void AjouterTâche(string nom, ITaskObserver observer)
        {
            var tâcheViewModel = new TâcheViewModel(nom, observer);
            DoOnUiThread(() => Tâches.Add(tâcheViewModel));
            RaisePropertyChangedOnUiThread("ATâches");
        }

        private readonly IOpérateurPrincipal _opérateurPrincipal;
        private readonly DateTime _trancheDhlMax;
        private readonly DateTime _trancheDhlMin;
        private bool _estPrêt;

        private bool PeutChargerPlanning()
        {
            return EstPrêt;
        }

        private void ChargerPlanningAsync()
        {
            EstPrêt = false;
            _opérateurPrincipal.ChargerPlanningAsync(this)
                .ContinueWith(
                    (t) =>
                        {
                            EstPrêt = true;
                            if (t.IsFaulted)
                                MessageBox.Show($"Erreur : {t.Exception.DetailedMessage()}",
                                                "Erreur durant l'opération");
                        });
        }

        private bool PeutVérifierEtTéléchargerCabs()
        {
            return EstPrêt && Options.TéléchargerCab;
        }

        private void VérifierEtTéléchargerCabsAsync()
        {
            EstPrêt = false;
            _opérateurPrincipal.VérifierEtTéléchargerCabsAsync(_trancheDhlMin, _trancheDhlMax,
                                                               Sélection,
                                                               Options,
                                                               this)
                .ContinueWith(
                    t =>
                        {
                            if (t.IsFaulted)
                                MessageBox.Show($"Erreur : {t.Exception.DetailedMessage()}",
                                                "Erreur durant l'opération");
                            else if (t.IsCompleted)
                                Rapport.Assigner(t.Result);
                            EstPrêt = true;
                        });
        }
    }
}