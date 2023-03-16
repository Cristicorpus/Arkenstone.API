using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arkenstone.Entities.DbSet
{
    [Table("Corporations")]
    public class Corporation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
