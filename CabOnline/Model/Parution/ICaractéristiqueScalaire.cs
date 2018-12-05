namespace CabOnline.Model.Parution
{
    internal interface ICaractéristiqueScalaire
    {
        string LibelléCourt { get; }
        string LibelléLong { get; }

        int CompareTo(ICaractéristiqueScalaire caractéristiqueScalaire);
    }
}