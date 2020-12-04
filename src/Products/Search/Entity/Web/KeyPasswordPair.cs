using Newtonsoft.Json;

namespace GroupDocs.Total.MVC.Products.Search.Entity.Web
{
    public class KeyPasswordPair
    {
        public KeyPasswordPair(string key, string password)
        {
            Key = key;
            Password = password;
        }

        [JsonProperty]
        public string Key { get; private set; }

        [JsonProperty]
        public string Password { get; private set; }
    }
}
