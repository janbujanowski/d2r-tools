using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2Rbots
{
    public class DiscordWebhookRequest
    {
        public string content;
        public string username;
   
        public DiscordWebhookRequest(D2runewizardTZRespone tzUpdate)
        {
            this.username = "D2R TZ bot service";
            this.content = $"Current TZ : {tzUpdate.terrorZone.zone} with probability : {tzUpdate.terrorZone.highestProbabilityZone.probability}";
        }
    }
}
