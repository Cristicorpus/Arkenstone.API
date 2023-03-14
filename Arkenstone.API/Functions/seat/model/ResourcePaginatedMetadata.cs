using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace seat
{
    public class ResourcePaginatedMetadata
    {
        public int current_page { get; set; }
        public int from { get; set; }
        public int last_page { get; set; }
        public string path { get; set; }
        public int per_page { get; set; }
        public int to { get; set; }
        public int total { get; set; }

    }
}
