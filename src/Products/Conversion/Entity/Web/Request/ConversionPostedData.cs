using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupDocs.Total.MVC.Products.Conversion.Entity.Web.Request
{
    public class ConversionPostedData
    {
        [JsonProperty]
        private List<string> guids { get; set; }

        [JsonProperty]
        private List<ConversionProperties> conversionProperties { get; set; }

        public List<string> GetGuids()
        {
            return this.guids;
        }

        public void SetGuids(List<string> guids) {
            this.guids = guids;
        }

        public List<ConversionProperties> GetConversionProperties()
        {
            return this.conversionProperties;
        }

        public void SetConversionProperties(List<ConversionProperties> properties) {
            this.conversionProperties = properties;
        }
    }
}