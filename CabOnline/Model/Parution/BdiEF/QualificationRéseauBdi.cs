
namespace CabOnline.Model.Parution.BdiEF
{
    using System;
    using Generic;

    internal class QualificationRéseauBdi : QualificationRéseauBase
    {
        private const string _CodeInconnu = "Code traitement à définir";
        private const string _CodeAucun = "Pas de code traitement";
        private const string _CodeRd = "RD";
        private const string _CodePm = "PM";
        private const string _CodePz = "PZ";
        private const string _CodeEy = "EY";
        private const string _CodeAl = "AL";
        private const string _CodePp = "PP";
        private const string _CodePlv = "PLV (Usage Interne)";
        private readonly string _libellé;

        public QualificationRéseauBdi(string libellé)
        {
            _libellé = libellé;
        }

        public override TypeQualificationRéseau TypeQualificationRéseau
        {
            get
            {
                switch (_libellé)
                {
                    case _CodeInconnu:
                        return TypeQualificationRéseau.Inconnu;
                    case _CodeAucun:
                        return TypeQualificationRéseau.Aucun;
                    case _CodeRd:
                        return TypeQualificationRéseau.Rd;
                    case _CodePm:
                        return TypeQualificationRéseau.Pm;
                    case _CodePz:
                        return TypeQualificationRéseau.Pz;
                    case _CodeEy:
                        return TypeQualificationRéseau.Ey;
                    case _CodeAl:
                        return TypeQualificationRéseau.Al;
                    case _CodePp:
                        return TypeQualificationRéseau.Pp;
                    case _CodePlv:
                        return TypeQualificationRéseau.Plv;
                    default:
                        throw new ArgumentException("QualificationRéseau inconnue");
                }
            }
        }

        public override string LibelléCourt => _libellé;

        public override string LibelléLong => LibelléCourt;
    }
}