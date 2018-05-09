using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4.MicroService.AzureJobs.Services
{
   internal class GithubService
    {
        //http://www.color-hex.com/
        static List<string> colorHex = new List<string>() {
"00e6dc",
"2ccbdc",
"22959b",
"328484",
"ec4758",
"e62db9",
"c55a00",
"e37a22",
"ffd72f",
"26ee99",
"85ffcb",
"ff6584",
"d487ff",
"967ff9",
"d69aff",
"7b90f9",
"7bdcf9",
"78f5ff",
"7eb233",
"dba15a",
"74412f",
"ff966f",
"926757",
"ffe1d6",
"ff66cd",
"ff98e6",
"ffff75",
"4267b2",
"f3904f",
"1ab394",
"ec4758",
"c0c0c0",
"ffd700",
"000080",
"990000",
"cc0000",
"80ddff",
"98b7d6",
"001933",
"7eb546",
"19a35e",
"8080ff"
        };

        static Random rd = new Random(DateTime.Now.Second);

        static string owner = "seven1986";
        static string repo = "game5.0";
        static string userToken = "6d94e8bb3f8702466b45bd58eff1d257d69918d8";
        static string userAgent = "Awesome-Game5.0-App";
        static string client_id = "Iv1.a5ea8db038e34dc1";
        static string client_secret = "4fcf853f5021e8a00b32825eddc36f1895c71f75";
    }

    internal class GithubLabel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string color { get; set; }
    }
}
