namespace CabOnline
{
    using System;
    using System.Linq;
    using System.Windows;
    using Autofac;
    using Mo.JsonConfiguration;
    using Mo.SafeEnvironment;
    using Model.AnomalieCab;
    using Model.Cab;
    using Model.ComparateurCabParution;
    using Model.Dépôt;
    using Model.Dépôt.BdiEF;
    using Model.Dépôt.Mlp;
    using Model.Dépôt.Nmpp;
    using Model.Dépôt.Pao;
    using MySql.Data.MySqlClient;
    using Operators;
    using Operators.Mailer;
    using Operators.RapportMail;
    using Operators.Tiff;
    using TaskObserver;
    using ViewModel;
    using Views;

    /// <summary>
    ///     Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var c = ConfigurationApplication();
            var w = c.Resolve<MainWindow>();
            w.Show();
        }

        private static IContainer ConfigurationApplication()
        {
            var cb = new ContainerBuilder();

            var configs = JsonConfigurationService.Create()
                                                  .WithVaultRoot(SafeEnvironment.Get("_MMP_VR"))
                                                  .WithVaultKey(SafeEnvironment.Get("_MMP_VK"))
                                                  .GetConfiguratonInstances(
                                                      new[]
                                                      {
                                                          typeof(AppSettings.Configuration),
                                                          typeof(Model.Dépôt.Mlp.Configuration)
                                                      });
            var mainConfig = configs.OfType<AppSettings.Configuration>().FirstOrDefault() ??
                             throw new Exception("Impossible de charger la configuration de l'application depuis le sous dossier \"Config\"");
            var mlpConfig = configs.OfType<Model.Dépôt.Mlp.Configuration>().FirstOrDefault() ??
                            throw new Exception("Impossible de charger la configuration MLP depuis le sous dossier \"Config\"");

            var bdiCsBuilder = new MySqlConnectionStringBuilder
                               {
                                   Server = "bdidata.multimediapress.local",
                                   UserID = mainConfig.UtilisateurBdi,
                                   Password = mainConfig.MotDePasseBdi,
                                   Database = mainConfig.DatabaseBdi
                               };

            cb.RegisterType<DépôtParutionsBdi>().As<IDépôtParutions>()
              .WithParameter("connectionString", bdiCsBuilder.ConnectionString)
              .WithParameter("providerName", "MySql.Data.MySqlClient")
              .SingleInstance();

            cb.RegisterType<DépôtCabMlp>().As<IDépôtCabMlp>()
              .WithParameter("configuration", mlpConfig)
              .SingleInstance();

            cb.RegisterType<DépôtCabNmpp>().As<IDépôtCabNmpp>()
              .SingleInstance();

            cb.RegisterType<DépôtCabPao>().As<IDépôtCabPao>()
              .WithParameter("racinePao", mainConfig.RacinePao)
              .SingleInstance();

            cb.RegisterType<Cab>().As<ICab>();
            cb.RegisterType<AnomalieCab>().As<IAnomalieCab>();
            cb.RegisterType<AnomalieCabFactory>().As<IAnomalieCabFactory>()
              .SingleInstance();
            cb.RegisterType<CabFactory>().As<ICabFactory>()
              .SingleInstance();
            cb.RegisterType<ComparateurCabParution>().As<IComparateurCabParution>()
              .SingleInstance();

            cb.RegisterType<InfosOpérationDépôt>().As<IInfosOpérationDépôt>();
            cb.RegisterType<InfosOpérationDépôtFactory>().As<IInfosOpérationDépôtFactory>()
              .SingleInstance();

            cb.RegisterType<ConvertisseurEpsVersTiff>().As<IConvertisseurEpsVersTiff>()
              .SingleInstance();

            cb.RegisterType<TaskObserver.TaskObserver>().As<ITaskObserver>();
            cb.RegisterType<TaskObserverFactory>().As<ITaskObserverFactory>()
              .SingleInstance();

            cb.RegisterType<GénérateurRapportMail>().As<IGénérateurRapportMail>()
              .SingleInstance();
            cb.RegisterType<Mailer>().As<IMailer>()
              .WithParameter("from", mainConfig.AdresseMailRapportFrom)
              .WithParameter("fromLabel", "Rapport Cabs Auto")
              .WithParameter("to", mainConfig.AdresseMailRapportTo)
              .WithParameter("toLabel", "Infos Cabs")
              .WithParameter("server", mainConfig.ServeurSmtp)
              .WithParameter("port", mainConfig.PortSmtp)
              .WithParameter("ssl", mainConfig.SslSmtp)
              .WithParameter("authentication", mainConfig.AutentificationSmtp)
              .WithParameter("user", mainConfig.UtilisateurSmtp)
              .WithParameter("password", mainConfig.MotDePasseSmtp)
              .SingleInstance();
            cb.RegisterType<OpérateurPrincipal>().As<IOpérateurPrincipal>()
              .SingleInstance();

            cb.Register(c => new SélectionViewModel(c.Resolve<IDépôtParutions>()) {Numéro = 1});
            cb.RegisterType<OptionsViewModel>();
            cb.RegisterType<RapportViewModel>();
            cb.RegisterType<MainViewModel>()
              .WithParameter("trancheDhlMin", DateTime.Now.AddDays(mainConfig.TrancheDhlInf))
              .WithParameter("trancheDhlMax", DateTime.Now.AddDays(mainConfig.TrancheDhlSup));
            cb.RegisterType<MainWindow>();
            return cb.Build();
        }
    }
}