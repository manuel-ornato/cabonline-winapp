using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CabOnline.Extensions.Exception;
using CabOnline.Model.Cab;
using CabOnline.Model.Dépôt;
using CabOnline.Model.Dépôt.Mlp;
using CabOnline.Model.Dépôt.Nmpp;
using CabOnline.Model.Dépôt.Pao;
using CabOnline.Model.Parution;
using CabOnline.Operators.Mailer;
using CabOnline.Operators.RapportMail;
using CabOnline.TaskObserver;
using CabOnline.ViewModel;

namespace CabOnline.Operators
{
    internal class OpérateurPrincipal : IOpérateurPrincipal
    {
        public OpérateurPrincipal(IDépôtParutions dépôtParutions, IDépôtCabPao dépôtCabPao, IDépôtCabMlp dépôtCabMlp,
                                  IDépôtCabNmpp dépôtCabNmpp, ITaskObserverFactory taskObserverFactory,
                                  IInfosOpérationDépôtFactory infosOpérationDépôtFactory,
                                  IGénérateurRapportMail générateurRapportMail, IMailer mailer)
        {
            _dépôtParutions = dépôtParutions ?? throw new ArgumentNullException(nameof(dépôtParutions));
            _dépôtCabPao = dépôtCabPao ?? throw new ArgumentNullException(nameof(dépôtCabPao));
            _dépôtCabMlp = dépôtCabMlp ?? throw new ArgumentNullException(nameof(dépôtCabMlp));
            _dépôtCabNmpp = dépôtCabNmpp ?? throw new ArgumentNullException(nameof(dépôtCabNmpp));
            _taskObserverFactory = taskObserverFactory ?? throw new ArgumentNullException(nameof(taskObserverFactory));
            _infosOpérationDépôtFactory = infosOpérationDépôtFactory ?? throw new ArgumentNullException(nameof(infosOpérationDépôtFactory));
            _générateurRapportMail = générateurRapportMail ?? throw new ArgumentNullException(nameof(générateurRapportMail));
            _mailer = mailer;
        }

        public Task ChargerPlanningAsync(IAfficheurTâches afficheurTâches)
        {
            return Task.Factory.StartNew(() => ChargerPlanning(afficheurTâches),
                                         TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
        }

        public Task<IEnumerable<InfosTraitementParution>> VérifierEtTéléchargerCabsAsync(
            DateTime dhlMin, DateTime dhlMax, 
            IOptionsSélectionParutions optionsSélection, IOptionsTéléchargementCabs optionsTéléchargement,
            IAfficheurTâches afficheurTâches)
        {
            return
                Task.Factory.StartNew(
                    () =>
                    VérifierEtTéléchargerCabs(dhlMin, dhlMax, optionsSélection, optionsTéléchargement, afficheurTâches),
                    TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
        }

        private readonly IDépôtCabMlp _dépôtCabMlp;
        private readonly IDépôtCabNmpp _dépôtCabNmpp;
        private readonly IDépôtCabPao _dépôtCabPao;
        private readonly IDépôtParutions _dépôtParutions;
        private readonly IGénérateurRapportMail _générateurRapportMail;
        private readonly IInfosOpérationDépôtFactory _infosOpérationDépôtFactory;
        private readonly IMailer _mailer;
        private readonly ITaskObserverFactory _taskObserverFactory;

        private void ChargerPlanning(IAfficheurTâches afficheurTâches)
        {
            var observer = _taskObserverFactory.CreateTaskObserver();
            afficheurTâches.AjouterTâche("Chargement des données du planning", observer);
            try
            {
                observer.NotifyStarted(this);
                observer.NotifyProgress(this, 0);
                _dépôtParutions.ChargerParutions(observer);
                observer.NotifyProgress(this, 100);
                observer.NotifyCompleted(this);
            }
            catch (Exception exception)
            {
                observer.NotifyError(this, exception.DetailedMessage());
                observer.NotifyAborted(this, exception.DetailedMessage());
            }
        }

        private Task<IEnumerable<IParution>> ParutionsCibléesAsync(DateTime dhlMin, DateTime dhlMax,
                                                                   IOptionsSélectionParutions optionsSélection)
        {
            return Task.Factory.StartNew<IEnumerable<IParution>>(
                () => ParutionsCiblées(dhlMin, dhlMax, optionsSélection),
                TaskCreationOptions.AttachedToParent);
        }

        private IEnumerable<IParution> ParutionsCiblées(DateTime dhlMin, DateTime dhlMax,
                                                        IOptionsSélectionParutions optionsSélection)
        {
            return
                RequêteParutionsCiblées(_dépôtParutions.Parutions.AsParallel(), dhlMin, dhlMax, optionsSélection)
                    .ToList();
        }

        private static IEnumerable<IParution> RequêteParutionsCiblées(IEnumerable<IParution> parutions, DateTime dhlMin,
                                                                      DateTime dhlMax,
                                                                      IOptionsSélectionParutions optionsSélection)
        {
            var req = from p in parutions
                      where (p.DateDhl >= dhlMin) && (p.DateDhl <= dhlMax) && !p.FaitSortie &&
                            (p.Distributeur.TypeDistributeur == TypeDistributeur.Mlp || p.Distributeur.TypeDistributeur == TypeDistributeur.Nmpp) &&
                            !p.SupplémentAutreProduit
                      select p;

            if (optionsSélection.FiltrerParCodeInterne && !String.IsNullOrWhiteSpace(optionsSélection.CodeInterne))
                req = from p in req where p.CodeSérie == optionsSélection.CodeInterne select p;

            if (optionsSélection.FiltrerParDate && optionsSélection.DateDébut.HasValue)
                req = from p in req where p.DateDhl >= optionsSélection.DateDébut select p;

            if (optionsSélection.FiltrerParDate && optionsSélection.DateFin.HasValue)
                req = from p in req where p.DateDhl <= optionsSélection.DateFin select p;

            if (optionsSélection.FiltrerParNuméro)
                req = from p in req where p.Numéro == optionsSélection.Numéro select p;

            if (optionsSélection.FiltrerParPôle && optionsSélection.Pôle != null)
                req = from p in req where p.Pôle.LibelléCourt == optionsSélection.Pôle.LibelléCourt select p;

            return req;
        }

        private IEnumerable<InfosTraitementParution> InfosParutions(IEnumerable<IParution> parutions,
                                                                    IEnumerable<IInfosOpérationDépôt>
                                                                        infosOpérationDépôt)
        {
            //var hs = new HashSet<IParution>();
            //foreach(var info in infosOpérationDépôt)
            //{    
            //    if(info.CabSource!=null)
            //    {
            //        if(hs.Contains(info.CabSource.ParutionCible))
            //            Debugger.Break();
            //        hs.Add(info.CabSource.ParutionCible);
            //    }
            //}


            var dicInfos =
                infosOpérationDépôt.Where(i => i.CabSource != null).ToDictionary(i => i.ParutionCible);
            return
                from p in parutions
                orderby p.DateDhl
                let infoOnline = dicInfos.ContainsKey(p) ? dicInfos[p] : null
                select new InfosTraitementParution(p, _dépôtCabPao.Cab(p), infoOnline);
        }

        private IEnumerable<InfosTraitementParution> VérifierEtTéléchargerCabs(DateTime dhlMin, DateTime dhlMax,
                                                                               IOptionsSélectionParutions
                                                                                   optionsSélection,
                                                                               IOptionsTéléchargementCabs
                                                                                   optionsTéléchargement,
                                                                               IAfficheurTâches afficheurTâches)
        {
            Task<IEnumerable<IInfosOpérationDépôt>> téléchargerCabsMlpTask = null;
            Task<IEnumerable<IInfosOpérationDépôt>> téléchargerCabsNmppTask = null;
            var cabsPaoTask = ChargerDépôtCabAsync(_dépôtCabPao, afficheurTâches);
            var parutionsCibléesTask = ParutionsCibléesAsync(dhlMin, dhlMax, optionsSélection);
            if (optionsTéléchargement.ExaminerSiteMlp)
            {
                var cabsMlpTask = ChargerDépôtCabAsync(_dépôtCabMlp, afficheurTâches);
                téléchargerCabsMlpTask = TéléchargerCabsAsync(parutionsCibléesTask, cabsPaoTask, cabsMlpTask,
                                                              optionsTéléchargement, afficheurTâches);
            }
            else
            {
                téléchargerCabsMlpTask =
                    Task.Factory.StartNew(() => (IEnumerable<IInfosOpérationDépôt>) new List<IInfosOpérationDépôt>());
            }
            if (optionsTéléchargement.ExaminerSiteNmpp)
            {
                var cabsNmppTask = ChargerDépôtCabAsync(_dépôtCabNmpp, afficheurTâches);
                téléchargerCabsNmppTask = TéléchargerCabsAsync(parutionsCibléesTask, cabsPaoTask, cabsNmppTask,
                                                               optionsTéléchargement, afficheurTâches);
            }
            else
            {
                téléchargerCabsNmppTask =
                    Task.Factory.StartNew(() => (IEnumerable<IInfosOpérationDépôt>) new List<IInfosOpérationDépôt>());
            }

            var infosParutions = InfosParutions(parutionsCibléesTask.Result,
                                                                                 téléchargerCabsMlpTask.Result.Union(
                                                                                     téléchargerCabsNmppTask.Result));

            EnvoyerRapportMail(infosParutions);

            return infosParutions;
        }

        private void EnvoyerRapportMail(IEnumerable<InfosTraitementParution> infosParutions)
        {
            string mailBody = _générateurRapportMail.RapportMail(infosParutions);
            _mailer.Send("Rapport Cabs auto", mailBody);
        }

        private Task<IDépôtCab> ChargerDépôtCabAsync(IDépôtCab dépôtCab, IAfficheurTâches afficheurTâches)
        {
            return Task.Factory.StartNew(() => ChargerDépôtCab(dépôtCab, afficheurTâches),
                                         TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
        }

        private IDépôtCab ChargerDépôtCab(IDépôtCab dépôtCab, IAfficheurTâches afficheurTâches)
        {
            var observer = _taskObserverFactory.CreateTaskObserver();
            afficheurTâches.AjouterTâche($"Examen des Cabs disponibles sur [{dépôtCab}]", observer);
            try
            {
                observer.NotifyStarted(this);
                observer.NotifyProgress(this, 0);

                dépôtCab.ChargerCabs(observer);

                observer.NotifyProgress(this, 100);
                observer.NotifyCompleted(this);

                return dépôtCab;
            }
            catch (Exception exception)
            {
                observer.NotifyError(this, exception.DetailedMessage());
                observer.NotifyAborted(this, "Interruption causée par une erreur");
                throw;
            }
        }

        private Task<IEnumerable<IInfosOpérationDépôt>> TéléchargerCabsAsync(
            Task<IEnumerable<IParution>> parutionsCibléesTask, Task<IDépôtCab> cabsPaoTask,
            Task<IDépôtCab> cabsExternesTask, IOptionsTéléchargementCabs optionsTéléchargement,
            IAfficheurTâches afficheurTâches)
        {
            return Task.Factory.StartNew(
                () =>
                TéléchargerCabs(parutionsCibléesTask.Result, cabsPaoTask.Result as IDépôtCabPao, cabsExternesTask.Result,
                                optionsTéléchargement, afficheurTâches),
                TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
        }

        private IEnumerable<IInfosOpérationDépôt> TéléchargerCabs(IEnumerable<IParution> parutionsCiblées,
                                                                  IDépôtCabPao dépôtCabPao,
                                                                  IDépôtCab dépôtCabExterne,
                                                                  IOptionsTéléchargementCabs optionsTéléchargement,
                                                                  IAfficheurTâches afficheurTâches)
        {
            var observer = _taskObserverFactory.CreateTaskObserver();
            afficheurTâches.AjouterTâche($"Téléchargement des Cabs depuis [{dépôtCabExterne}]",
                                         observer);
            try
            {
                observer.NotifyStarted(this);
                observer.NotifyProgress(this, 0);

                var résultat =
                    TéléchargerCabsCore(parutionsCiblées, dépôtCabPao, dépôtCabExterne,
                                        optionsTéléchargement.ConvertirCabVersTiff, observer);

                observer.NotifyProgress(this, 100);
                observer.NotifyCompleted(this);

                return résultat;
            }
            catch (Exception exception)
            {
                observer.NotifyError(this, exception.DetailedMessage());
                observer.NotifyAborted(this, "Interruption causée par une erreur");
                throw;
            }
        }

        private IEnumerable<IInfosOpérationDépôt> TéléchargerCabsCore(IEnumerable<IParution> parutionsCiblées,
                                                                      IDépôtCabPao dépôtCabPao,
                                                                      IDépôtCab dépôtCabExterne,
                                                                      bool conversionVersTiff,
                                                                      ITaskObserver observer)
        {
            observer.NotifyProgress(this, 0);

            var i = 0;
            int total = parutionsCiblées.Count();
            var téléchargementTasks =
                (from p in parutionsCiblées
                 select Task.Factory.StartNew(
                     () => TéléchargerCabSiNécessaireCore(p, dépôtCabExterne, dépôtCabPao, conversionVersTiff, observer),
                     TaskCreationOptions.AttachedToParent)
                     .ContinueWith(
                         t =>
                             {
                                 int step = Interlocked.Increment(ref i);
                                 observer.NotifyProgress(this, step*100/total);
                                 return t.Result;
                             },
                         TaskContinuationOptions.AttachedToParent
                     )).ToList();


            var résultat = (from task in téléchargementTasks where task.Result != null select task.Result).ToList();

            observer.NotifyProgress(this, 100);

            return résultat;
        }

        private IInfosOpérationDépôt TéléchargerCabSiNécessaireCore(IParution p, IDépôtCab dépôtCabExterne,
                                                                    IDépôtCabPao dépôtCabPao,
                                                                    bool conversionVerstiff, ITaskObserver observer)
        {
            try
            {
                var cabExterne = dépôtCabExterne.Cab(p);
                var cabPao = dépôtCabPao.Cab(p);
                if (cabExterne != null)
                {
                    if (cabPao == null || cabExterne.DateCréation > cabPao.DateCréation)
                    {
                        if (cabExterne.EstCompatible)
                        {
                            observer.NotifyInfo(this, $"Téléchargement nécessaire du cab de [{p}] ([{cabExterne.LibelléLong}]).");
                            return _infosOpérationDépôtFactory.CréerInfosOpérationDépôt(cabExterne,
                                                                                        dépôtCabPao.Déposer(cabExterne,
                                                                                                            conversionVerstiff,
                                                                                                            observer),
                                                                                        cabPao, EtatCabOnline.Téléchargé, p);
                        }
                        return _infosOpérationDépôtFactory.CréerInfosOpérationDépôt(cabExterne, null, cabPao,
                                                                                    EtatCabOnline.Incompatible, p);
                    }
                    else
                        return _infosOpérationDépôtFactory.CréerInfosOpérationDépôt(cabExterne, null, cabPao,
                                                                                    EtatCabOnline.PasPlusRécent, p);
                }
                else
                {
                    return _infosOpérationDépôtFactory.CréerInfosOpérationDépôt(null, null, cabPao,
                                                                                EtatCabOnline.Indisponible, p);
                }
            }
            catch (Exception exception)
            {
                observer.NotifyWarning(this,
                                       $"Echec dans la recherche de Cab pour [{p}] ({exception.DetailedMessage()})");
                return null;
            }
        }
    }
}