﻿using Arkenstone.Entities.DbSet;
using System.Collections.Generic;


namespace Arkenstone.API.Models
{
    public class EfficiencyModel
    {
        public EfficiencyModel()
        { }

        public decimal MEefficiency { get; set; }

        public LocationModel Station { get; set; }
        
        public List<RigsManufacturing> rigsEffect { get; set; }
    }
}
