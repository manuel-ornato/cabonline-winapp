namespace CabOnline.Model.Parution
{
    internal interface IPériodicité : ICaractéristiqueScalaire
    {
        TypePériodicité TypePériodicité { get; }
    }
}