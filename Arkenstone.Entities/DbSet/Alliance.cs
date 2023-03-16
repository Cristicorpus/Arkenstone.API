using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace Arkenstone.Entities.DbSet
{
    [Table("Alliances")]
    public class Alliance
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
