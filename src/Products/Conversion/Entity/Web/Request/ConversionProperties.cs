using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupDocs.Total.MVC.Products.Conversion.Entity.Web.Request
{
    public class ConversionProperties
    {
        [JsonProperty]
        private string guid { get; set; }

        [JsonProperty]
        private string destinationType { get; set; }

        public string GetGuid()
        {
            return this.guid;
        }

        public void SetGuid(string guid)
        {
            this.guid = guid;
        }

        public string getDestinationType()
        {
            return this.destinationType;
        }

        public void SetDestinationType(string type)
        {
            this.destinationType = type;
        }
    }
}