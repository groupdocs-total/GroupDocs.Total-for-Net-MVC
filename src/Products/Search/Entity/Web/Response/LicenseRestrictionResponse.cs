using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web.Response
{
    public class LicenseRestrictionResponse
    {
        [JsonProperty]
        public bool IsRestricted { get; set; }

        [JsonProperty]
        public string Message { get; set; }

        public static LicenseRestrictionResponse CreateNonRestricted()
        {
            var response = new LicenseRestrictionResponse()
            {
                IsRestricted = false,
            };
            return response;
        }

        public static LicenseRestrictionResponse CreateRestricted(string message)
        {
            var response = new LicenseRestrictionResponse()
            {
                IsRestricted = true,
                Message = message,
            };
            return response;
        }
    }
}
