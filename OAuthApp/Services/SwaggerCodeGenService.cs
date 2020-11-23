using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OAuthApp.Services
{
    public class SwaggerCodeGenResult
    {
        public string code { get; set; }
        public string link { get; set; }
    }

    public class SwaggerCodeGenItem
    {
        public string language { get; set; }

        public JObject options { get; set; }
    }

    public class SwaggerCodeGenService
    {
        const string ServerUrl = "https://generator.swagger.io/api/gen";

        #region ClientItems
        List<SwaggerCodeGenItem> clientItems;
        public List<SwaggerCodeGenItem> ClientItems
        {
            get
            {
                if (clientItems == null)
                {
                    clientItems = new List<SwaggerCodeGenItem>();

                    var languages = ClientLanguages().Result;

                    if (languages.Count > 0)
                    {
                        foreach (var language in languages)
                        {
                            var options = ClientLanguagesOptions(language).Result;

                            if (options != null)
                            {
                                clientItems.Add(new SwaggerCodeGenItem() { language = language, options = options });
                            }
                        }
                    }
                }

                return clientItems;
            }
        } 
        #endregion

        #region ClientItemsCache
        List<SwaggerCodeGenItem> clientItemsCache;
        const string clientItemsCacheFile = "https://jixiucampaignstorage.blob.core.chinacloudapi.cn/campaign-core-identity/swaggerCodeGen.clientItems.txt";
        public List<SwaggerCodeGenItem> ClientItemsCache
        {
            get
            {
                if (clientItemsCache == null)
                {
                    using (var hc = new HttpClient())
                    {
                        var result = hc.GetStringAsync(clientItemsCacheFile).Result;

                        clientItemsCache = JsonConvert.DeserializeObject<List<SwaggerCodeGenItem>>(result);
                    }
                }

                return clientItemsCache;
            }
        }
        #endregion

        #region ServerItems
        List<SwaggerCodeGenItem> serverItems;
        public List<SwaggerCodeGenItem> ServerItems
        {
            get
            {
                if (serverItems == null)
                {
                    serverItems = new List<SwaggerCodeGenItem>();

                    var languages = ServerLanguages().Result;

                    if (languages.Count > 0)
                    {
                        foreach (var language in languages)
                        {
                            var options = ServerLanguagesOptions(language).Result;

                            if (options != null)
                            {
                                serverItems.Add(new SwaggerCodeGenItem() { language = language, options = options });
                            }
                        }
                    }
                }

                return serverItems;
            }
        } 
        #endregion

        #region ServerItemsCache
        List<SwaggerCodeGenItem> serverItemsCache;
        const string serverItemsCacheFile = "https://jixiucampaignstorage.blob.core.chinacloudapi.cn/campaign-core-identity/swaggerCodeGen.serverItems..txt";
        public List<SwaggerCodeGenItem> ServerItemsCache
        {
            get
            {
                if (serverItemsCache == null)
                {
                    using (var hc = new HttpClient())
                    {
                        var result = hc.GetStringAsync(serverItemsCacheFile).Result;

                        serverItemsCache = JsonConvert.DeserializeObject<List<SwaggerCodeGenItem>>(result);
                    }
                }

                return serverItemsCache;
            }
        } 
        #endregion

        public async Task<SwaggerCodeGenResult> ClientGenerate(string language, string options) => await Generate("clients", language, options);
        public async Task<SwaggerCodeGenResult> ServerGenerate(string language, string options) => await Generate("servers", language, options);
        async Task<SwaggerCodeGenResult> Generate(string side, string language, string options)
        {
            using (var hc = new HttpClient())
            {
                var url = $"{ServerUrl}/{side}/{language}";

                var content = new StringContent(options, Encoding.UTF8, "application/json");

                var response = await hc.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<SwaggerCodeGenResult>(responseContent);

                    return result;
                }
            }

            return null;
        }

        async Task<List<string>> ClientLanguages() => await Languages("clients");
        async Task<List<string>> ServerLanguages() => await Languages("servers");
        async Task<List<string>> Languages(string side)
        {
            using (var hc = new HttpClient())
            {
                var url = $"{ServerUrl}/{side}";

                var response = await hc.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<List<string>>(responseContent);

                    return result;
                }
            }

            return null;
        }

        async Task<JObject> ClientLanguagesOptions(string language) => await LanguagesOptions("clients", language);
        async Task<JObject> ServerLanguagesOptions(string language) => await LanguagesOptions("servers", language);
        async Task<JObject> LanguagesOptions(string side, string language)
        {
            using (var hc = new HttpClient())
            {
                var url = $"{ServerUrl}/{side}/{language}";

                var response = await hc.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var result = JObject.Parse(responseContent);

                    return result;
                }
            }

            return null;
        }
    }
}
