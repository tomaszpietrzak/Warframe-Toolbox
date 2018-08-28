using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Warframe_Toolbox
{
    public class MarketItem
    {
        public string Url_name { get; set; }
        public string ID { get; set; }
        public string Item_name { get; set; }
        public string Price { get; set; }

        public MarketItem()
        {
            Price = "0";
        }
    }

    public abstract class Notification
    {
        public string NotificationString { get; set; }
        public abstract string ToNotificationString();
    }
    public class Id
    {
        [JsonProperty("$oid")]
        public string oid { get; set; }
    }

    public class Date
    {
        [JsonProperty("$numberLong")]
        public string numberLong { get; set; }
    }

    public class Activation
    {
        [JsonProperty("$date")]
        public Date date { get; set; }
    }

    public class Expiry
    {
        [JsonProperty("$date")]
        public Date date { get; set; }
    }

    public class MissionReward
    {
        public int credits { get; set; }
        public List<string> items { get; set; }
    }

    public class MissionInfo
    {
        public string missionType { get; set; }
        public string faction { get; set; }
        public string location { get; set; }
        public string levelOverride { get; set; }
        public string enemySpec { get; set; }
        public string extraEnemySpec { get; set; }
        public int minEnemyLevel { get; set; }
        public int maxEnemyLevel { get; set; }
        public double difficulty { get; set; }
        public int seed { get; set; }
        public MissionReward missionReward { get; set; }
    }

    public class Alert : Notification
    {
        public Id _id { get; set; }
        public Activation Activation { get; set; }
        public Expiry Expiry { get; set; }
        public MissionInfo MissionInfo { get; set; }

        public string Node { get; set; }
        public string Faction { get; set; }
        public string MissionType { get; set; }
        public string Level { get; set; }
        public string Rewards { get; set; }

        public override string ToNotificationString()
        {
            Node = WarframeInfoProvider.Nodes[MissionInfo.location]["value"].ToString();
            MissionType = WarframeInfoProvider.MissionTypes[MissionInfo.missionType]["value"].ToString();
            Faction = WarframeInfoProvider.Factions[MissionInfo.faction]["value"].ToString();
            Level = MissionInfo.minEnemyLevel.ToString() +"-"+ MissionInfo.maxEnemyLevel.ToString();
            Rewards = "Credits "+ MissionInfo.missionReward.credits;
            if (MissionInfo.missionReward.items != null)
            {
                foreach (string item in MissionInfo.missionReward.items)
                {
                    var last = item.LastIndexOf("/") +1;
                    var name = item.Substring(last, item.Length - last);
                    Rewards = Rewards + "\r\n " + name;
                }
            }
            StringBuilder sb = new StringBuilder();
            NotificationString = sb.AppendFormat(" Alert: {0} \r\n {1}({2}) \r\n Level:{3} \r\n {4}",
                                                Node,
                                                MissionType,
                                                Faction,
                                                Level,
                                                Rewards).ToString();
            return NotificationString;
        }
    }

    public class Invasion : Notification
    {
        public Id _id { get; set; }
        public Activation Activation { get; set; }
        
        public override string ToNotificationString()
        {
            StringBuilder sb = new StringBuilder();
            NotificationString = sb.AppendFormat("Alert: {0}", _id.oid).ToString();
            return NotificationString;
        }
    }
}