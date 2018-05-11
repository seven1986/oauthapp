using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.MicroService.AzureJobs.Services
{
    internal class GithubService
    {
        #region RandomColor
        /// <summary>
        /// colors from http://www.color-hex.com/
        /// </summary>
        static List<string> colorHex = new List<string>() { "00e6dc", "2ccbdc", "22959b", "328484", "ec4758", "e62db9", "c55a00", "e37a22", "ffd72f", "26ee99", "85ffcb", "ff6584", "d487ff", "967ff9", "d69aff", "7b90f9", "7bdcf9", "78f5ff", "7eb233", "dba15a", "74412f", "ff966f", "926757", "ffe1d6", "ff66cd", "ff98e6", "ffff75", "4267b2", "f3904f", "1ab394", "ec4758", "c0c0c0", "ffd700", "000080", "990000", "cc0000", "80ddff", "98b7d6", "001933", "7eb546", "19a35e", "8080ff" };
        static Random rd = new Random(DateTime.Now.Second);
        public string RandomColor
        {
            get
            {
                return colorHex[rd.Next(0, colorHex.Count - 1)];
            }
        } 
        #endregion

        public string userAgent { get; set; }

        /// <summary>
        /// owner
        /// </summary>
        public string owner { get; set; }

        /// <summary>
        /// repo
        /// </summary>
        public string repo { get; set; }

        /// <summary>
        /// token
        /// </summary>
        public string token { get; set; }

        public static string apiServer = "https://api.github.com";

        #region construct
        public GithubService(string _userAgent,  string _owner, string _repo, string _token)
        {
            userAgent = _userAgent;
            owner = _owner;
            repo = _repo;
            token = _token;
        } 
        #endregion

        public async Task<List<GithubLabel>> LabelsByPageAsync(int pageIndex)
        {
            List<GithubLabel> labels = null;
            try
            {
                using (var wc = new HttpClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                    wc.DefaultRequestHeaders.Add("Accept", "application/json");

                    wc.DefaultRequestHeaders.Add("User-Agent", userAgent);

                    var url = $"{apiServer}/repos/{owner}/{repo}/labels?access_token={token}&page={pageIndex}";

                    var result = await wc.GetAsync(url);

                    if (result.IsSuccessStatusCode)
                    {
                        var jsonString = result.Content.ReadAsStringAsync().Result;

                        labels = JsonConvert.DeserializeObject<List<GithubLabel>>(jsonString);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return labels;
        }

        public async Task<List<GithubLabel>> LabelsAsync()
        {
            var result = new List<GithubLabel>();

            List<GithubLabel> _labels;

            int pageIndex = 1;

            do
            {
                _labels = await LabelsByPageAsync(pageIndex);

                pageIndex++;

                if (_labels != null && _labels.Count > 0)
                {
                    result.AddRange(_labels);
                }
            }
            while (_labels != null && _labels.Count > 0);

            return result;
        }

        public async Task<HttpResponseMessage> LabelPostAsync(string name, string description, string color = "")
        {
            if (string.IsNullOrWhiteSpace(color))
            {
                color = RandomColor;
            }

            using (var wc = new HttpClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                wc.DefaultRequestHeaders.Add("Accept", "application/json");
                wc.DefaultRequestHeaders.Add("User-Agent", userAgent);

                var url = $"{apiServer}/repos/{owner}/{repo}/labels?access_token={token}";

                var content = new StringContent(JsonConvert.SerializeObject(new
                {
                    name,
                    description,
                    color
                }), Encoding.UTF8, "application/json");

                var result = await wc.PostAsync(url, content);

                return result;
            }
        }

        public async Task<Dictionary<string, string>> OperationsAsync(string swaggerUrl)
        {
            var result = new Dictionary<string, string>();

            var swaggerDoc = string.Empty;

            using (var wc = new HttpClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                swaggerDoc = await wc.GetStringAsync(swaggerUrl);
            }

            if (!string.IsNullOrWhiteSpace(swaggerDoc))
            {
                var swaggerObj = JsonConvert.DeserializeObject<JObject>(swaggerDoc);

                var swaggerTitle = swaggerObj["info"]["title"].Value<string>();

                foreach (JProperty v in swaggerObj["paths"])
                {
                    foreach (JProperty method in v.Value)
                    {
                        result.Add(
                            swaggerTitle + "." + method.Value["operationId"].Value<string>(),
                            method.Value["description"].Value<string>());
                    }
                }
            }

            return result;
        }
    }

    internal class GithubLabel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string color { get; set; }
    }

    public class GithubQueueModel
    {
        /// <summary>
        /// userAgent
        /// </summary>
        public string userAgent { get; set; } = "Awesome-Game5.0-App";

        /// <summary>
        /// owner
        /// </summary>
        public string owner { get; set; }

        /// <summary>
        /// repo
        /// </summary>
        public string repo { get; set; }

        /// <summary>
        /// token
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// swaggerUrl
        /// </summary>
        public string swaggerUrl { get; set; }
    }
}
