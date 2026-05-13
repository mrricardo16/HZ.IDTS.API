using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity
{
    public class tn_mdg_dictionary
    {
        public string CODE { get; set; }
        public string NAME { get; set; }
        public string TEXT { get; set; }
        public string NUM { get; set; }
        public string VALUE { get; set; }
        public string TYPE { get; set; }

        public string dicName { get; set; }
        public string dicCode { get; set; }

        public List<tn_mdg_dictionary> child = new List<tn_mdg_dictionary>();

        public string parentName { get; set; }

    }
}
