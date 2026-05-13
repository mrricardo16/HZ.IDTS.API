using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTS.SimulateService
{
    public class AGVSimulateModel
    {
        public int carid { get; set; }
        public string name { get; set; } = "";
        public double x { get; set; }
        public double y { get; set; }
        public double th { get; set; }
        public string carType { get; set; } = "";
        public int siteID { get; set; }
        public int[] holdingLocks { get; set; }
        public int statusWorld { get; set; }
    }
}
