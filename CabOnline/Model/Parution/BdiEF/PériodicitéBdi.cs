namespace CabOnline.Model.Parution.BdiEF
{
    using Generic;

    internal class P�riodicit�Bdi : P�riodicit�Base
    {
        private const string _CodeTrimestrielle = "Trimestriel";
        private const string _CodeBimestrielle = "Bimestriel";
        private const string _CodeBiPuisTri = "Bimestriel puis trimestriel";
        private const string _CodeMensuelle = "Mensuel";
        private const string _CodeInconnue = "P�riodicit� � d�finir";
        private const string _CodeIrr�guli�re = "Irr�gulier";
        private readonly bool _aD�finir;
        private readonly string _libell�;

        public P�riodicit�Bdi(bool aD�finir, string libell�)
        {
            _aD�finir = aD�finir;
            _libell� = libell�;
        }

        public override TypeP�riodicit� TypeP�riodicit�
        {
            get
            {
                if (_aD�finir)
                    return TypeP�riodicit�.Inconnue;
                switch (_libell�)
                {
                    case _CodeMensuelle:
                        return TypeP�riodicit�.Mensuelle;
                    case _CodeBiPuisTri:
                        return TypeP�riodicit�.BiPuisTri;
                    case _CodeBimestrielle:
                        return TypeP�riodicit�.Bimestrielle;
                    case _CodeTrimestrielle:
                        return TypeP�riodicit�.Trimestrielle;
                    case _CodeIrr�guli�re:
                        return TypeP�riodicit�.Irr�guli�re;
                    case _CodeInconnue:
                    default:
                        return TypeP�riodicit�.Inconnue;
                }
            }
        }

        public override string Libell�Court => _libell�;

        public override string Libell�Long => Libell�Court;
    }
}