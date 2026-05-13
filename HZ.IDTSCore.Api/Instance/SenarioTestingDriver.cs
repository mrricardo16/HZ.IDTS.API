using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Instance
{
    public class SenarioTestingDriver
    {
        private static readonly SenarioTestingDriver instance = new SenarioTestingDriver();

        private SenarioTestingDriver()
        {

        }

        public static SenarioTestingDriver Instance
        {
            get
            {
                return instance;
            }
        }

        
    }
}
