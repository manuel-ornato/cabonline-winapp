namespace CabOnline.Model.Parution.BdiEF
{
    using Generic;

    internal class PériodicitéBdi : PériodicitéBase
    {
        private const string _CodeTrimestrielle = "Trimestriel";
        private const string _CodeBimestrielle = "Bimestriel";
        private const string _CodeBiPuisTri = "Bimestriel puis trimestriel";
        private const string _CodeMensuelle = "Mensuel";
        private const string _CodeInconnue = "Périodicité à définir";
        private const string _CodeIrrégulière = "Irrégulier";
        private readonly bool _aDéfinir;
        private readonly string _libellé;

        public PériodicitéBdi(bool aDéfinir, string libellé)
        {
            _aDéfinir = aDéfinir;
            _libellé = libellé;
        }

        public override TypePériodicité TypePériodicité
        {
            get
            {
                if (_aDéfinir)
                    return TypePériodicité.Inconnue;
                switch (_libellé)
                {
                    case _CodeMensuelle:
                        return TypePériodicité.Mensuelle;
                    case _CodeBiPuisTri:
                        return TypePériodicité.BiPuisTri;
                    case _CodeBimestrielle:
                        return TypePériodicité.Bimestrielle;
                    case _CodeTrimestrielle:
                        return TypePériodicité.Trimestrielle;
                    case _CodeIrrégulière:
                        return TypePériodicité.Irrégulière;
                    case _CodeInconnue:
                    default:
                        return TypePériodicité.Inconnue;
                }
            }
        }

        public override string LibelléCourt => _libellé;

        public override string LibelléLong => LibelléCourt;
    }
}