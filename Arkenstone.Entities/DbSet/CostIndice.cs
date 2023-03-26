using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arkenstone.Entities.DbSet
{
    [Table("CostIndices")]
    public class CostIndice
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SolarSystemId { get; set; }
        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public CostIndiceType type { get; set; }
        public decimal Cost { get; set; }
    }

    public enum CostIndiceType
    {
        manufacturing, 
        reaction
    }

}
