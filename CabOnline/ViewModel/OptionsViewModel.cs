using CabOnline.Operators;

namespace CabOnline.ViewModel
{
    internal class OptionsViewModel : ViewModel, IOptionsTéléchargementCabs
    {
        private bool _convertirCabVersTiff = true;
        private bool _examinerDossierCab = false;
        private bool _examinerSiteMlp = true;
        private bool _examinerSiteNmpp = false;
        private bool _téléchargerCab = true;

        public bool ExaminerDossierCab
        {
            get => _examinerDossierCab;
            set
            {
                _examinerDossierCab = value;
                RaisePropertyChangedOnUiThread("ExaminerDossierCab");
            }
        }

        public bool ExaminerSiteMlp
        {
            get => _examinerSiteMlp;
            set
            {
                _examinerSiteMlp = value;
                RaisePropertyChangedOnUiThread("ExaminerSiteMlp");
                RaisePropertyChangedOnUiThread("TéléchargerCabPossible");
                RaisePropertyChangedOnUiThread("TéléchargerCab");
                RaisePropertyChangedOnUiThread("ConvertirCabVersTiffPossible");
                RaisePropertyChangedOnUiThread("ConvertirCabVersTiff");
            }
        }

        public bool ExaminerSiteNmpp
        {
            get => _examinerSiteNmpp;
            set
            {
                _examinerSiteNmpp = value;
                RaisePropertyChangedOnUiThread("ExaminerSiteNmpp");
                RaisePropertyChangedOnUiThread("TéléchargerCabPossible");
                RaisePropertyChangedOnUiThread("TéléchargerCab");
                RaisePropertyChangedOnUiThread("ConvertirCabVersTiffPossible");
                RaisePropertyChangedOnUiThread("ConvertirCabVersTiff");
            }
        }

        public bool ConvertirCabVersTiff
        {
            get => ConvertirCabVersTiffPossible && _convertirCabVersTiff;
            set
            {
                _convertirCabVersTiff = value;
                RaisePropertyChangedOnUiThread("ConvertirCabVersTiff");
            }
        }

        public bool TéléchargerCab
        {
            get => TéléchargerCabPossible && _téléchargerCab;
            set
            {
                _téléchargerCab = value;
                RaisePropertyChangedOnUiThread("TéléchargerCab");
                RaisePropertyChangedOnUiThread("ConvertirCabVersTiffPossible");
                RaisePropertyChangedOnUiThread("ConvertirCabVersTiff");
            }
        }

        public bool TéléchargerCabPossible => ExaminerSiteMlp || ExaminerSiteNmpp;

        public bool ConvertirCabVersTiffPossible => TéléchargerCab;
    }
}