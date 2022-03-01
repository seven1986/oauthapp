using System.Collections.Generic;

namespace OAuthApp.ApiModels.AppsController
{
    public class AppFileItem
    {
        public AppFileItem(int _id, string _text, string _fullName, string _type)
        {
            id = _id;
            text = _text;
            fullName = _fullName;
            type = _type;
        }

        public int id { get; set; }

        public string text { get; set; }

        public string fullName { get; set; }

        public string type { get; set; }

        public List<AppFileItem> children { get; set; }
    }
}
