using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Windows.Data.Json;
using LuckyBallsData.Statistics;

namespace LuckyBallsSpirit.DataModel
{
    public class LotteryWebSite
    {
        public LotteryWebSite()
        {
            Name = "";
            Uri = null;
            OutputFormat = " :";
            Customized = true;
        }

        public bool Customized
        {
            get;
            set;
        }

        public string Icon
        {
            get
            {
                return "http://" + Uri.Host + "/favicon.ico";
            }
        }

        public string Name
        {
            get;
            set;
        }

        public Uri Uri
        {
            get;
            set;
        }

        public string OutputFormat
        {
            get;
            set;
        }

        public string Sample
        {
            get
            {
                Scheme temp = new Scheme(1,2,3,4,5,6,7);
                return temp.ToString(OutputFormat);
            }
        }

        //static public LotteryWebSite Parse(JsonObject jsonObj)
        //{
        //    string uri = jsonObj["Uri"].GetString();

        //    return new LotteryWebSite() 
        //    { 
        //        Name = jsonObj["Name"].GetString(), 
        //        Uri = uri.Length > 0 ? new Uri(uri) : null, 
        //        OutputFormat = jsonObj["Format"].GetString(),
        //        Customized = uri.Length > 0
        //    };
        //}

        //static public string GetJSONString(List<LotteryWebSite> sites)
        //{
        //    // Set as default.
        //    JsonArray jsonArray = new JsonArray();

        //    foreach (LotteryWebSite site in sites)
        //    {
        //        JsonObject jsonObject = new JsonObject();
        //        jsonObject["Name"] = JsonValue.CreateStringValue(site.Name);
        //        jsonObject["Uri"] = JsonValue.CreateStringValue(site.Uri != null ? site.Uri.ToString() : "");
        //        jsonObject["Format"] = JsonValue.CreateStringValue(site.OutputFormat);
        //        jsonArray.Add(jsonObject);
        //    }

        //    return jsonArray.Stringify();
        //}
    }
}
