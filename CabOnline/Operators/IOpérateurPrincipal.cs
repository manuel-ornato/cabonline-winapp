using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CabOnline.ViewModel;

namespace CabOnline.Operators
{
    internal interface IOp�rateurPrincipal
    {
        Task ChargerPlanningAsync(IAfficheurT�ches afficheurT�ches);

        Task<IEnumerable<InfosTraitementParution>> V�rifierEtT�l�chargerCabsAsync(DateTime dhlMin,
                                                                                  DateTime dhlMax,
                                                                                  IOptionsS�lectionParutions
                                                                                      optionsS�lection,
                                                                                  IOptionsT�l�chargementCabs
                                                                                      optionsT�l�chargement,
                                                                                  IAfficheurT�ches
                                                                                      afficheurT�ches);
    }
}