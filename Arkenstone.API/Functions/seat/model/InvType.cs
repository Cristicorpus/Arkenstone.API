using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace seat
{
    public class InvType
    {
        public int typeID { get; set; }
        public int groupID { get; set; }
        public string typeName { get; set; }
        public string description { get; set; }
        public double? mass { get; set; }
        public double? volume { get; set; }
        public double? capacity { get; set; }
        public int? portionSize { get; set; }
        public int? raceID { get; set; }
        public double? basePrice { get; set; }
        public bool? published { get; set; }
        public int? marketGroupID { get; set; }
        public int? iconID { get; set; }
        public int? soundID { get; set; }
        public int? graphicID { get; set; }

    }
}
