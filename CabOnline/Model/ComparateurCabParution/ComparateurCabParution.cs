using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CabOnline.Model.AnomalieCab;
using CabOnline.Model.Cab;
using CabOnline.Model.Parution;

namespace CabOnline.Model.ComparateurCabParution
{
    internal class ComparateurCabParution : IComparateurCabParution
    {
        public ComparateurCabParution(IAnomalieCabFactory anomalieCabFactory)
        {
            _anomalieCabFactory = anomalieCabFactory ?? throw new ArgumentNullException(nameof(anomalieCabFactory));
        }

        public void CalculerCompatbilité(ICab cab, out bool estCompatible, out IEnumerable<IAnomalieCab> anomalies)
        {
            Tuple<bool, IEnumerable<IAnomalieCab>> cachedValue;
            if (_cache.TryGetValue(cab, out cachedValue))
            {
                estCompatible = cachedValue.Item1;
                anomalies = cachedValue.Item2;
                return;
            }

            estCompatible = true;

            var lAnomalies = new List<IAnomalieCab>();
            anomalies = lAnomalies;
            if (ComparerEditeurs(cab.Editeur, cab.ParutionCible.Editeur) != 0)
            {
                // On note la différence d'éditeur mais ce n'est pas une incompatibilité
                lAnomalies.Add(_anomalieCabFactory.CréerAnomalieCab(TypeAnomalieCab.Editeur,
                                                                    cab.ParutionCible.Editeur.LibelléCourt.ToUpper(),
                                                                    cab.Editeur.LibelléCourt.ToUpper(),
                                                                    nécessiteAction: true,
                                                                    estIncompatible: false));
            }
            if (ComparerCodifs(cab.Codif, cab.ParutionCible.Codif) != 0)
            {
                estCompatible = false;
                lAnomalies.Add(_anomalieCabFactory.CréerAnomalieCab(TypeAnomalieCab.Codif,
                                                                    cab.ParutionCible.Codif,
                                                                    cab.Codif,
                                                                    nécessiteAction: true, 
                                                                    estIncompatible: true));
            }
            if (ComparerNuméros(cab.Numéro, cab.ParutionCible.Numéro) != 0)
            {
                estCompatible = false;
                lAnomalies.Add(_anomalieCabFactory.CréerAnomalieCab(TypeAnomalieCab.Numéro,
                                                                    cab.ParutionCible.Numéro.ToString(),
                                                                    cab.Numéro.ToString(),
                                                                    nécessiteAction: true, 
                                                                    estIncompatible: true));
            }
            if (ComparerPrix(cab.Prix, cab.ParutionCible.Prix) != 0)
            {
                estCompatible = false;
                lAnomalies.Add(_anomalieCabFactory.CréerAnomalieCab(TypeAnomalieCab.Prix,
                                                                    (cab.ParutionCible.Prix ?? 0).ToString("C"),
                                                                    (cab.Prix ?? 0).ToString("C"),
                                                                    nécessiteAction: true, 
                                                                    estIncompatible: true));
            }
            if (ComparerQualifs(cab.Qualif, cab.ParutionCible.Qualif) != 0)
            {
                estCompatible = false;
                lAnomalies.Add(_anomalieCabFactory.CréerAnomalieCab(TypeAnomalieCab.Qualification,
                                                                    cab.ParutionCible.Qualif.LibelléCourt.ToUpper(),
                                                                    cab.Qualif.LibelléCourt.ToUpper(),
                                                                    nécessiteAction: true, 
                                                                    estIncompatible: true));
            }
            if (ComparerDistributeurs(cab.Distributeur, cab.ParutionCible.Distributeur) != 0)
            {
                estCompatible = false;
                lAnomalies.Add(_anomalieCabFactory.CréerAnomalieCab(TypeAnomalieCab.Distributeur,
                                                                    cab.ParutionCible.Distributeur.LibelléCourt.
                                                                        ToUpper(),
                                                                    cab.Distributeur.LibelléCourt.ToUpper(),
                                                                    nécessiteAction: true, 
                                                                    estIncompatible: true));
            }
            int résultatComparaisonPériodicités = ComparerPériodicités(cab.Périodicité,
                                                                       cab.ParutionCible.PériodicitéEffective);
            if (résultatComparaisonPériodicités != 0)
            {
                // On note la différence de périodicité mais ce n'est pas une incompatibilité
                lAnomalies.Add(_anomalieCabFactory.CréerAnomalieCab(TypeAnomalieCab.Périodicité,
                                                                    cab.ParutionCible.Périodicité.LibelléCourt,
                                                                    cab.Périodicité.LibelléCourt,
                                                                    nécessiteAction: résultatComparaisonPériodicités < 0, 
                                                                    estIncompatible: false));
            }

            _cache[cab] = new Tuple<bool, IEnumerable<IAnomalieCab>>(estCompatible, lAnomalies);
        }

        private readonly IAnomalieCabFactory _anomalieCabFactory;

        private readonly ConcurrentDictionary<ICab, Tuple<bool, IEnumerable<IAnomalieCab>>> _cache =
            new ConcurrentDictionary<ICab, Tuple<bool, IEnumerable<IAnomalieCab>>>();

        private static int ComparerNuméros(int? numéro1, int? numéro2)
        {
            if (!numéro1.HasValue || !numéro2.HasValue)
                return 0;

            return numéro1.Value.CompareTo(numéro2.Value);
        }

        private static int ComparerCodifs(string codif1, string codif2)
        {
            if (codif1 == null || codif2 == null)
                return 0;

            return codif1.CompareTo(codif2);
        }

        private static int ComparerPrix(decimal? prix1, decimal? prix2)
        {
            if (!prix1.HasValue || !prix2.HasValue)
                return 0;

            return prix1.Value.CompareTo(prix2.Value);
        }

        private static int ComparerDistributeurs(IDistributeur distributeur1, IDistributeur distributeur2)
        {
            return distributeur1.CompareTo(distributeur2);
        }

        private static int ComparerEditeurs(IEditeur éditeur1, IEditeur éditeur2)
        {
            return éditeur1.CompareTo(éditeur2);
        }

        private static int ComparerQualifs(IQualificationRéseau qualif1, IQualificationRéseau qualif2)
        {
            return qualif1.CompareTo(qualif2);
        }

        private static int ComparerPériodicités(IPériodicité périodicité1, IPériodicité périodicité2)
        {
            return périodicité1.CompareTo(périodicité2);
        }
    }
}