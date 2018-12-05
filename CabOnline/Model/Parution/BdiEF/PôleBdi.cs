using CabOnline.Model.Parution.Generic;

namespace CabOnline.Model.Parution.BdiEF
{
    internal class PôleBdi : PôleBase
    {
        private string _libellé;

        public PôleBdi(string libellé)
        {
            _libellé = libellé;
        }

        public override string LibelléCourt => _libellé??"";

        public override string LibelléLong => LibelléCourt;
    }
}