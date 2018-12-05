using System;
using CabOnline.Model.AnomalieCab;

namespace CabOnline.Operators.RapportMail
{
    internal class AnomalieCabStAdapter
    {
        private readonly IAnomalieCab _anomalieCab;

        public AnomalieCabStAdapter(IAnomalieCab anomalieCab)
        {
            _anomalieCab = anomalieCab ?? throw new ArgumentNullException(nameof(anomalieCab));
        }

        public string LibelleCourt => _anomalieCab.LibelléCourt;

        public string LibelleLong => _anomalieCab.LibelléLong;

        public TypeAnomalieCab Type => _anomalieCab.Type;

        public bool NecessiteAction => _anomalieCab.NécessiteAction;

        public bool EstIncompatible => _anomalieCab.EstIncompatible;

        public string ValeurParution => _anomalieCab.ValeurParution;
        public string ValeurCab => _anomalieCab.ValeurCab;

        public override string ToString()
        {
            return LibelleLong;
        }
    }
}