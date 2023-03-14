using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arkenstone.Entities.DbSet
{
    [Table("Tickets")]
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ItemId { get; set; }

        public virtual Item Item { get; set; }

        public int Quantity { get; set; }

        public int? TicketParentId { get; set; }

        public virtual Ticket TicketParent { get; set; }

        public StateEnum State { get; set; }

        public int CharacterId { get; set; }

        public virtual ICollection<Ticket> TicketEnfant { get; set; }

        public Ticket()
        {
            TicketEnfant = new HashSet<Ticket>();
        }
    }

    public enum StateEnum
    {
        Production,
        Achat,
        Vente,
        Livraison,
    }
}
