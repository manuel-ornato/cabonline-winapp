namespace CabOnline.Model.Dépôt.BdiEF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Parution;
    using Parution.BdiEF;
    using Simple.Data;
    using TaskObserver;

    internal class DépôtParutionsBdi : IDépôtParutions
    {
        private const int _IdTiersMlp = 20;
        private const string _ActivitéLibelléDistributeur = "Distributeur";
        private const string _ActivitéLibelléEditeur = "Editeur";
        private const string _ActivitéLibelléRégulateur = "Régulateur";
        private const int _IdTypeRevue = 8;

        public DépôtParutionsBdi(string connectionString, string providerName)
        {
            _db = Database.Opener.OpenConnection(connectionString, providerName);
            _pôles = ((List<dynamic>)_db.Pole.All()
                                         .Select(_db.Pole.IdPole, _db.Pole.Libelle))
                .Select(x => new { Id = (int)x.IdPole, Libellé = (string)x.Libelle })
                .ToDictionary(x => x.Id, x => new PôleBdi(x.Libellé));

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void ChargerParutions(ITaskObserver taskObserver)
        {
            taskObserver.NotifyProgress(this, 0);

            _distributeurs = ((List<dynamic>) _db.Tiers.All()
                                                 .Where(_db.Tiers.TiersActivite.Activite.Libelle.Like(_ActivitéLibelléDistributeur))
                                                 .Select(_db.Tiers.IdTiers, _db.Tiers.LibelleCourt, _db.Tiers.RaisonSociale))
                .Select(x => new {Id = (int) x.IdTiers, Libellé = (string) x.LibelleCourt, RaisonSociale = (string) x.RaisonSociale})
                .ToDictionary(x => x.Id, x => new DistributeurBdi(x.Libellé, x.RaisonSociale));

            taskObserver.NotifyProgress(this, 5);

            _éditeurs = ((List<dynamic>) _db.Tiers.All()
                                            .Where(_db.Tiers.TiersActivite.Activite.Libelle.Like(_ActivitéLibelléEditeur))
                                            .Select(_db.Tiers.IdTiers, _db.Tiers.LibelleCourt, _db.Tiers.RaisonSociale))
                .Select(x => new {Id = (int) x.IdTiers, Libellé = (string) x.LibelleCourt, RaisonSociale = (string) x.RaisonSociale})
                .ToDictionary(x => x.Id, x => new EditeurBdi(x.Libellé, x.RaisonSociale));

            taskObserver.NotifyProgress(this, 10);

            _régulateurs = ((List<dynamic>) _db.Tiers.All()
                                               .Where(_db.Tiers.TiersActivite.Activite.Libelle.Like(_ActivitéLibelléRégulateur))
                                               .Select(_db.Tiers.IdTiers, _db.Tiers.LibelleCourt, _db.Tiers.RaisonSociale))
                .Select(x => new {Id = (int) x.IdTiers, Libellé = (string) x.LibelleCourt, RaisonSociale = (string) x.RaisonSociale})
                .ToDictionary(x => x.Id, x => x.RaisonSociale);

            taskObserver.NotifyProgress(this, 15);

            _périodicités = ((List<dynamic>) _db.Periodicite.All()
                                                .Select(_db.Periodicite.IdPeriodicite, _db.Periodicite.Libelle, _db.Periodicite.ADefinir))
                .Select(x => new {Id = (int) x.IdPeriodicite, Libellé = (string) x.Libelle, ADéfinir = (bool) x.ADefinir})
                .ToDictionary(x => x.Id, x => new PériodicitéBdi(x.ADéfinir, x.Libellé));

            taskObserver.NotifyProgress(this, 20);

            _qualificationsRéseau = ((List<dynamic>) _db.QualificationReseau.All()
                                                        .Select(_db.QualificationReseau.IdQualificationReseau, _db.QualificationReseau.Libelle))
                .Select(x => new {Id = (int) x.IdQualificationReseau, Libellé = (string) x.Libelle})
                .ToDictionary(x => x.Id, x => new QualificationRéseauBdi(x.Libellé));

            taskObserver.NotifyProgress(this, 30);

            Parutions = ((List<dynamic>) _db.MiseEnVente.All()
                                            .Where(_db.MiseEnVente.FaitLivraison == false)
                                            .Where(_db.MiseEnVente.IdDistributeur == _IdTiersMlp)
                                            .Where(_db.MiseEnVente.Types == _IdTypeRevue)
                                            .Where(_db.MiseEnVente.ProduitMiseEnVente.Produit.ProduitElement.Principal == true)
                                            .Select(_db.MiseEnVente.IdMiseEnVente.Distinct(), 
                                                    _db.MiseEnVente.IdDistributeur,
                                                    _db.MiseEnVente.RefDistributeur,
                                                    _db.MiseEnVente.IdQualificationReseau,
                                                    _db.MiseEnVente.FaitSortie,
                                                    _db.MiseEnVente.ProduitMiseEnVente.Produit.ProduitElement.Element.IdElement,
                                                    _db.MiseEnVente.ProduitMiseEnVente.Produit.ProduitElement.Element.Titre,
                                                    _db.MiseEnVente.ProduitMiseEnVente.Produit.ProduitElement.Element.IdEditeur,
                                                    _db.MiseEnVente.ProduitMiseEnVente.Produit.ProduitElement.Element.Reference,
                                                    _db.MiseEnVente.ProduitMiseEnVente.Produit.ProduitElement.Element.PrixImprime,
                                                    _db.MiseEnVente.ProduitMiseEnVente.Produit.ProduitElement.Element.Numero,
                                                    _db.MiseEnVente.ProduitMiseEnVente.Produit.ProduitElement.Element.VDateDepartFab,
                                                    _db.MiseEnVente.ProduitMiseEnVente.Produit.ProduitElement.Element.DateDepartFab,
                                                    _db.MiseEnVente.ProduitMiseEnVente.Produit.ProduitElement.Element.SousPole.IdPole,
                                                    _db.MiseEnVente.ProduitMiseEnVente.Produit.ProduitElement.Element.Serie.StatutSerie.Libelle.As(
                                                        "LibelleStatutSerie"),
                                                    _db.MiseEnVente.ProduitMiseEnVente.Produit.ProduitElement.Element.ElementRevue.IdPeriodiciteExterne,
                                                    _db.MiseEnVente.ProduitMiseEnVente.Produit.ProduitElement.Element.ElementRevue.IdRegulateur))
                .Select(x => new
                {
                    Distributeur = _distributeurs[(int) x.IdDistributeur],
                    RefDistributeur = (string) x.RefDistributeur,
                    QualificationRéseau = _qualificationsRéseau[(int) x.IdQualificationReseau],
                    FaitSortie = (bool) x.FaitSortie,
                    Titre = (string) x.Titre,
                    Editeur = _éditeurs[(int) x.IdEditeur],
                    Référence = (string) x.Reference,
                    PrixImprimé = (decimal?) x.PrixImprime,
                    Numéro = (int) x.Numero,
                    VDateDépartFab = (DateTime?) x.VDateDepartFab,
                    DateDépartFab = (DateTime?) x.DateDepartFab,
                    Pôle = _pôles[(int) x.IdPole],
                    StatutSérie = (string) x.LibelleStatutSerie,
                    Périodicité = _périodicités[(int) x.IdPeriodiciteExterne],
                    Régulateur = _régulateurs[(int) x.IdRegulateur],
                    FaitPao = GetFaitPao((int) x.IdElement)
                })
                .Select(
                    x =>
                    new ParutionBdi(x.Distributeur,
                                    x.Editeur,
                                    x.QualificationRéseau,
                                    x.Pôle,
                                    x.Périodicité,
                                    x.RefDistributeur,
                                    x.Titre,
                                    x.Référence,
                                    x.PrixImprimé,
                                    x.Numéro,
                                    x.VDateDépartFab,
                                    x.DateDépartFab,
                                    x.StatutSérie,
                                    x.Régulateur,
                                    x.FaitSortie,
                                    x.FaitPao))
                .ToList();

            taskObserver.NotifyProgress(this, 100);
        }

        public IEnumerable<IParution> Parutions { get; private set; }

        public IDistributeur Distributeur(TypeDistributeur typeDistributeur)
        {
            return (from d in _distributeurs.Values where d.TypeDistributeur == typeDistributeur select d).FirstOrDefault();
        }

        public IEditeur Editeur(TypeEditeur typeEditeur)
        {
            return (from e in _éditeurs.Values where e.TypeEditeur == typeEditeur select e).FirstOrDefault();
        }

        public IPériodicité Périodicité(TypePériodicité typePériodicité)
        {
            return (from p in _périodicités.Values where p.TypePériodicité == typePériodicité select p).FirstOrDefault();
        }

        public IQualificationRéseau QualificationRéseau(TypeQualificationRéseau typeQualificationRéseau)
        {
            return
                (from q in _qualificationsRéseau.Values where q.TypeQualificationRéseau == typeQualificationRéseau select q).
                    FirstOrDefault();
        }

        public IPôle Pôle(string codePôle)
        {
            return (from p in _pôles.Values where p.LibelléCourt == codePôle select p).FirstOrDefault();
        }

        public IEnumerable<IPôle> Pôles => _pôles.Values;
        private Dictionary<int, DistributeurBdi> _distributeurs;
        private Dictionary<int, PériodicitéBdi> _périodicités;
        private Dictionary<int, PôleBdi> _pôles;
        private Dictionary<int, QualificationRéseauBdi> _qualificationsRéseau;
        private Dictionary<int, EditeurBdi> _éditeurs;
        private readonly dynamic _db;
        private Dictionary<int, string> _régulateurs;

        private dynamic GetFaitPao(int idElement)
        {
            return _db.UniteDOeuvreElement.GetCount(
                _db.UniteDOeuvreElement.IdElement == idElement &&
                _db.UniteDOeuvreElement.UniteDOeuvre.TypeUniteDOeuvre.Libelle.Like("SR Maquette") &&
                _db.UniteDOeuvreElement.UniteDOeuvre.Fait == false) == 0;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}