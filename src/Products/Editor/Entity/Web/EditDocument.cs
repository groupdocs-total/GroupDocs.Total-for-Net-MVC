using System.Collections.Generic;
using System.Web.Mvc;

namespace GroupDocs.Total.MVC.Products.Editor.Entity.Web
{
    public class EditDocument
    {
        [AllowHtml]
        public string HtmlContent { get; set; }

        public string EditableDocumentName { get; set; }

        public string CssRelativePath { get; set; }       

        public List<string> OutputFormats { get; set; }

        public bool IsNewDocument { get; set; }

        public EditableDocumentType DocumentType { get; set; }
    }
}