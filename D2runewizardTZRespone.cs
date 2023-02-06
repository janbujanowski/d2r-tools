using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2Rbots
{
    //https://d2runewizard.com/api/terror-zone?token=S8mY7MbhhxMyL7YbmYut6w
    public class D2runewizardTZRespone
    {
        public TerrorZone terrorZone;
    }

    public class TerrorZone
    {
        public string zone;
        public string act;
        public HighestProbabilityZone highestProbabilityZone;
    }
    public class HighestProbabilityZone
    {
        public string zone;
        public string act;
        public int amount;
        public int probability;
    }
}