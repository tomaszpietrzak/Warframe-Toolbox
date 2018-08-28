using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warframe_Toolbox
{
    static class WarframeInfoProvider
    {
        

        public static List<MarketItem> GetMarketItems()
        {
            RestClient client = new RestClient("https://api.warframe.market/v1");
            var request = new RestRequest("items", Method.GET);

            IRestResponse response = client.Execute(request);

            JObject parsedResponse = JObject.Parse(response.Content);

            // get JSON result objects into a list
            IList<JToken> results = parsedResponse["payload"]["items"]["en"].Children().ToList();

            List<MarketItem> _searchResults = new List<MarketItem>();
            foreach (JToken result in results)
            {
                // JToken.ToObject is a helper method that uses JsonSerializer internally
                MarketItem searchResult = result.ToObject<MarketItem>();
                _searchResults.Add(searchResult);
            }

            return _searchResults;
        }

        public static JObject GetWorldState()
        {
            RestClient client = new RestClient("http://content.warframe.com/dynamic/worldState.php");

            var request = new RestRequest("", Method.GET);

            IRestResponse response = client.Execute(request);

            return JObject.Parse(response.Content);
        }

        public static List<Alert> GetAlerts(JObject worldState)
        {
            var w = worldState["Alerts"].Children().ToList();
            List<Alert> alerts = new List<Alert>();
            foreach(JToken t in w)
            {
                var a = t.ToObject<Alert>();
                alerts.Add(a);
            }
            return alerts;
        }

        public static void GetInvasions(JObject worldState)
        {
            var w = worldState["Invasions"].Children().ToList();
            
        }

        private static JObject _factions;
        public static JObject Factions
        { 
            get
            {
                if (_factions == null)
                {
                    _factions = JObject.Parse(File.ReadAllText(@"./warframedata/factionsData.json"));
                }
                return _factions;
            }
        }

        private static JObject _nodes;
        public static JObject Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _nodes = JObject.Parse(File.ReadAllText(@"./warframedata/solNodes.json"));
                }
                return _nodes;
            }
        }

        private static JObject _missionTypes;
        public static JObject MissionTypes
        {
            get
            {
                if (_missionTypes == null)
                {
                    _missionTypes = JObject.Parse(File.ReadAllText(@"./warframedata/missionTypes.json"));
                }
                return _missionTypes;
            }
        }
        
    }
}