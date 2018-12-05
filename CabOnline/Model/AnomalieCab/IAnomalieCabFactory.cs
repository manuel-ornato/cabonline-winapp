namespace CabOnline.Model.AnomalieCab
{
    internal interface IAnomalieCabFactory
    {
        IAnomalieCab CréerAnomalieCab(TypeAnomalieCab type, string valeurParution, string valeurCab, bool nécessiteAction, bool estIncompatible);
    }
}