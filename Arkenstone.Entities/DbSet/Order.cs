using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arkenstone.Entities.DbSet
{
    [Table("Orders")]
    public class Order
    {
        public Order()
        {
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public TypeOrderEnum TypeOrder { get; set; }
        public int CharacterId { get; set; }
        public int LocationId { get; set; }
        public virtual Location Location { get; set; }
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
    public enum TypeOrderEnum
    {
        Production,
        Achat,
        Vente,
        Livraison,
    }
}
