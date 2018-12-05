namespace CabOnline.Operators
{
    internal interface IOptionsTéléchargementCabs
    {
        bool ExaminerDossierCab { get; set; }
        bool ExaminerSiteMlp { get; set; }
        bool ExaminerSiteNmpp { get; set; }
        bool ConvertirCabVersTiff { get; set; }
        bool TéléchargerCab { get; set; }
    }
}