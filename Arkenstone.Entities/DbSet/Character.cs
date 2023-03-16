using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkenstone.Entities.DbSet
{
    [Table("Characters")]
    public class Character
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }


        public int CorporationId { get; set; }
        public virtual Corporation Corporation { get; set; }
        public int AllianceId { get; set; }
        public virtual Alliance Alliance { get; set; }
        public int CharacterMainId { get; set; }
        public virtual Character CharacterMain { get; set; }
    }
}
