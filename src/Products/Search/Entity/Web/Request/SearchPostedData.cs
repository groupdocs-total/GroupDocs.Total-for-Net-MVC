using GroupDocs.Total.MVC.Products.Common.Entity.Web;
using Newtonsoft.Json;
using System;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Request
{
    public class SearchPostedData : PostedDataEntity
    {
        [JsonProperty]
        private string query { get; set; }

        [JsonProperty]
        private string[] guids { get; set; }

        internal string GetQuery()
        {
            return this.query;
        }

        internal void SetQuery(string query)
        {
            this.query = query;
        }

        internal string[] GetGuids()
        {
            return this.guids;
        }

        internal void SetGuids(string[] guids)
        {
            this.guids = guids;
        }
    }
}