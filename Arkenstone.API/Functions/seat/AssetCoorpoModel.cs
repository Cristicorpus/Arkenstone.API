using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace seat
{
    public class AssetCoorpoModel
    {
        public List<CorporationAsset> data { get; set; }
        public ResourcePaginatedLinks links { get; set; }
        public ResourcePaginatedMetadata meta { get; set; }
    }
}
