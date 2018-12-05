using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Antlr3.ST;
using Antlr3.ST.Language;
using CabOnline.Model.Dépôt;
using CabOnline.Operators.RapportMail.StRenderers;
using CabOnline.ViewModel;

namespace CabOnline.Operators.RapportMail
{
    internal class GénérateurRapportMail : IGénérateurRapportMail
    {
        private readonly StringTemplateGroup _stg;

        public GénérateurRapportMail()
        {
            _stg = new StringTemplateGroup(
                new StreamReader(
                    Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream("CabOnline.Operators.RapportMail.StViews.RapportMailView.stg"),Encoding.UTF8),
                typeof(TemplateLexer)) {FileCharEncoding = Encoding.UTF8};
            _stg.RegisterRenderer(typeof(DateTime), new DateRenderer());
        }

        public string RapportMail(IEnumerable<IInfosTraitementParution> infosParutions)
        {
            var cabsIndisponibles =
                from i in infosParutions
                where i.CabAbsent && i.EtatCabOnline == EtatCabOnline.Indisponible
                orderby i.DateDhl
                select new InfosParutionStAdapter(i);
            var cabsBloqués =
                from i in infosParutions
                where i.CabAbsent && i.EtatCabOnline == EtatCabOnline.Incompatible
                orderby i.DateDhl
                select new InfosParutionStAdapter(i);
            var cabsIncompatibles =
                from i in infosParutions
                where i.EtatCab == EtatCab.Incompatible
                orderby i.DateDhl
                select new InfosParutionStAdapter(i);
            var cabsAnormauxAvecAction =
                from i in infosParutions
                where i.EtatCab == EtatCab.ActionNécessaire
                orderby i.DateDhl
                select new InfosParutionStAdapter(i);
            var cabsAnormaux =
                from i in infosParutions
                where i.EtatCab == EtatCab.Anormal
                orderby i.DateDhl
                select new InfosParutionStAdapter(i);
            var cabsTéléchargés =
                from i in infosParutions
                where i.EtatCabOnline == EtatCabOnline.Téléchargé
                orderby i.DateDhl
                select new InfosParutionStAdapter(i);

            var bodyTemplate = _stg.GetInstanceOf("body");
            bodyTemplate.SetAttribute("date", DateTime.Now);
            bodyTemplate.SetAttribute("cabsIndisponibles", cabsIndisponibles);
            bodyTemplate.SetAttribute("cabsBloques", cabsBloqués);
            bodyTemplate.SetAttribute("cabsIncompatibles", cabsIncompatibles);
            bodyTemplate.SetAttribute("cabsAnormauxAvecAction", cabsAnormauxAvecAction);
            bodyTemplate.SetAttribute("cabsAnormaux", cabsAnormaux);
            bodyTemplate.SetAttribute("cabsTelecharges", cabsTéléchargés);
            return bodyTemplate.ToString();
        }

    }
}