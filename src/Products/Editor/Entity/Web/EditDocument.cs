using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using System.Collections.Generic;
using System.Web.Mvc;

namespace GroupDocs.Total.MVC.Products.Editor.Entity.Web
{
    public class EditDocument : LoadDocumentEntity
    {
        [AllowHtml]
        public string editableDocumentName { get; set; }      

        public List<string> outputFormats { get; set; }

        public bool isNewDocument { get; set; }

        public EditableDocumentType documentType { get; set; }
    }
}