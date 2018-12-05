using System;
using System.Collections.ObjectModel;
using System.Linq;
using CabOnline.TaskObserver;
using CabOnline.TaskObserver.Messages;

namespace CabOnline.ViewModel
{
    internal class TâcheViewModel : ViewModel
    {
        public TâcheViewModel(string nom, ITaskObserver taskObserver)
        {
            _nom = nom ?? throw new ArgumentNullException(nameof(nom));
            _taskObserver = taskObserver ?? throw new ArgumentNullException(nameof(taskObserver));
            _messages = new ObservableCollection<MessageTâcheViewModel>();
            _duréeIndéterminée = false;
            _progression = 0;
            _état = EtatTâche.Inactive;
            _filtrerErreurs = true;
            _filtrerAvertissements = true;
            _filtrerInfos = true;
            _taskObserver.OnStarted += (sender, e) => ModifierEtat(EtatTâche.Active);
            _taskObserver.OnProgress += (sender, e) => ChangerProgression(e.Progress);
            _taskObserver.OnIsIndeterminate += (sender, e) => DevientIndéterminée();
            _taskObserver.OnCompleted += (sender, e) => ModifierEtat(EtatTâche.Terminée);
            _taskObserver.OnAborted += (sender, e) => DevientAbandonnée(e.Message);
            _taskObserver.OnStatus += (sender, e) => ModifierStatut(e.Message);
            _taskObserver.OnInfo += (sender, e) => AjouterMessage(e.Message);
            _taskObserver.OnWarning += (sender, e) => AjouterMessage(e.Message);
            _taskObserver.OnError += (sender, e) => AjouterMessage(e.Message);
        }

        public override string ToString()
        {
            return LibelléCourt;
        }

        public string Nom => _nom;

        public string Statut
        {
            get => _statut;
            set
            {
                _statut = value;
                RaisePropertyChangedOnUiThread("Statut");
                RaisePropertyChangedOnUiThread("LibelléLong");
            }
        }

        public string LibelléCourt => Nom;

        public string LibelléLong => Nom + (!String.IsNullOrWhiteSpace(Statut) ? $" ({Statut})" : "");

        public ObservableCollection<MessageTâcheViewModel> Messages => _messages;

        public Func<MessageTâcheViewModel, bool> FiltreMessages
        {
            get
            {
                return
                    m => ((_filtrerInfos && m.Niveau == MessageLevel.Info) ||
                            (_filtrerAvertissements && m.Niveau == MessageLevel.Warning) ||
                            (_filtrerErreurs && m.Niveau == MessageLevel.Error));
            }
        }

        public bool FiltrerInfos
        {
            get => _filtrerInfos;
            set
            {
                _filtrerInfos = value;
                RaisePropertyChangedOnUiThread("FiltrerInfos");
                RaisePropertyChangedOnUiThread("FiltreMessages");
            }
        }

        public bool FiltrerAvertissements
        {
            get => _filtrerAvertissements;
            set
            {
                _filtrerAvertissements = value;
                RaisePropertyChangedOnUiThread("FiltrerAvertissements");
                RaisePropertyChangedOnUiThread("FiltreMessages");
            }
        }

        public bool FiltrerErreurs
        {
            get => _filtrerErreurs;
            set
            {
                _filtrerErreurs = value;
                RaisePropertyChangedOnUiThread("FiltrerErreurs");
                RaisePropertyChangedOnUiThread("FiltreMessages");
            }
        }

        public bool ContientMessage => Messages.Count > 0;

        public bool ContientInfo
        {
            get { return _messages.Any(m => m.Niveau == MessageLevel.Info); }
        }

        public bool ContientAvertissement
        {
            get { return _messages.Any(m => m.Niveau == MessageLevel.Warning); }
        }

        public bool ContientErreur
        {
            get { return _messages.Any(m => m.Niveau == MessageLevel.Error); }
        }

        public bool DuréeIndéterminée
        {
            get => _duréeIndéterminée;
            private set
            {
                _duréeIndéterminée = value;
                RaisePropertyChangedOnUiThread("DuréeIndéterminée");
            }
        }

        public int Progression
        {
            get
            {
                if (Etat == EtatTâche.Active)
                    return _progression;
                else
                    return 0;
            }
            set
            {
                _progression = value;
                RaisePropertyChangedOnUiThread("Progression");
            }
        }

        public EtatTâche Etat
        {
            get => _état;
            set
            {
                _état = value;
                RaisePropertyChangedOnUiThread("Etat");
            }
        }

        private readonly ObservableCollection<MessageTâcheViewModel> _messages;
        private readonly string _nom;
        private readonly ITaskObserver _taskObserver;

        private bool _duréeIndéterminée;
        private int _progression;
        private string _statut;
        private EtatTâche _état;
        private bool _filtrerAvertissements;
        private bool _filtrerErreurs;
        private bool _filtrerInfos;

        private void AjouterMessage(Message message)
        {
            var messageTâcheViewModel = new MessageTâcheViewModel(message);
            DoOnUiThread(() => _messages.Add(messageTâcheViewModel));
            switch (message.Level)
            {
                case MessageLevel.Info:
                    RaisePropertyChangedOnUiThread("ContientInfo");
                    break;
                case MessageLevel.Warning:
                    RaisePropertyChangedOnUiThread("ContientAvertissement");
                    break;
                case MessageLevel.Error:
                    RaisePropertyChangedOnUiThread("ContientErreur");
                    break;
            }
            RaisePropertyChangedOnUiThread("ContientMessage");
        }

        private void DevientIndéterminée()
        {
            DuréeIndéterminée = true;
        }

        private void ChangerProgression(int valeur)
        {
            DuréeIndéterminée = false;
            Progression = valeur;
        }

        private void DevientAbandonnée(Message message)
        {
            ModifierEtat(EtatTâche.Abandonnée);
            AjouterMessage(message);
        }

        private void ModifierStatut(Message message)
        {
            Statut = message.Body;
        }

        private void ModifierEtat(EtatTâche étatTâche)
        {
            Etat = étatTâche;
        }
    }
}