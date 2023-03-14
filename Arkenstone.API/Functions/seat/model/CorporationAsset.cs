using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace seat
{
    public class CorporationAsset
    {
        public long item_id { get; set; }
        public int quantity { get; set; }
        public long location_id { get; set; }
        public string location_type { get; set; }
        public string location_flag { get; set; }
        public bool is_singleton { get; set; }
        public double? x { get; set; }
        public double? y { get; set; }
        public double? z { get; set; }
        public long? map_id { get; set; }
        public string map_name { get; set; }
        public string name { get; set; }
        public InvType type { get; set; }
    }
}
