namespace CabOnline.Model.AnomalieCab
{
    internal sealed class AnomalieCab : IAnomalieCab
    {
        public AnomalieCab(TypeAnomalieCab type, string valeurParution, string valeurCab, bool nécessiteAction,
                           bool estIncompatible)
        {
            Type = type;
            ValeurParution = valeurParution;
            ValeurCab = valeurCab;
            NécessiteAction = nécessiteAction;
            EstIncompatible = estIncompatible;
        }

        public string LibelléCourt => $"C:[{ValeurCab}]/P:[{ValeurParution}]{(NécessiteAction || EstIncompatible ? " (!)" : "")}";

        public string LibelléLong =>
            $"{Type} : Cab:[{ValeurCab}] vs. Planning:[{ValeurParution}]{(EstIncompatible ? " (anomalie blocante)" : (NécessiteAction ? " (action nécessaire)" : ""))}";
        public TypeAnomalieCab Type { get; private set; }

        public bool NécessiteAction { get; private set; }

        public bool EstIncompatible { get; private set; }

        public string ValeurParution { get; private set; }

        public string ValeurCab { get; private set; }

        public override string ToString()
        {
            return LibelléLong;
        }
    }
}