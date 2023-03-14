using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Intrinsics.X86;

namespace Arkenstone.Entities.DbSet
{
    [Index(nameof(LocationId), nameof(Flag), nameof(CorporationId), IsUnique =true, Name = "IX_SubLocations_LocationId")]
    [Table("SubLocations")]
    public class SubLocation
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        

        public long LocationId { get; set; }
        public virtual Location Location { get; set; }

        public string Flag { get; set; }
        public int CorporationId { get; set; }

        public bool IsAssetAnalysed { get; set; }

        public SubLocation()
        {
            
        }



    }
}
