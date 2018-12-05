using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CabOnline.ViewModel;

namespace CabOnline.Operators
{
    internal interface IOpérateurPrincipal
    {
        Task ChargerPlanningAsync(IAfficheurTâches afficheurTâches);

        Task<IEnumerable<InfosTraitementParution>> VérifierEtTéléchargerCabsAsync(DateTime dhlMin,
                                                                                  DateTime dhlMax,
                                                                                  IOptionsSélectionParutions
                                                                                      optionsSélection,
                                                                                  IOptionsTéléchargementCabs
                                                                                      optionsTéléchargement,
                                                                                  IAfficheurTâches
                                                                                      afficheurTâches);
    }
}