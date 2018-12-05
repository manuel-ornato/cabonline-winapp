namespace CabOnline.Model.Parution
{
    internal interface ICalculateurPériodicitéEffective
    {
        IPériodicité PériodicitéEffective(IPériodicité périodicité, int? numéro);
    }
}