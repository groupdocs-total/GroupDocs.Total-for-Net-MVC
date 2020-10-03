using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupDocs.Total.MVC.Products.Common.Entity.Web;

namespace GroupDocs.Total.MVC.Products.Parser.Entity.Web
{
    public class ParserDocumentEntity : PageDescriptionEntity
    {
        public string guid;
        public List<PageDescriptionEntity> pages = new List<PageDescriptionEntity>();
    }
}
