﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arkenstone.Entities.DbSet
{
    [Table("ProdAchats")]
    public class ProdAchat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "bigint")]
        public long Id { get; set; }
        
        public int CorporationId { get; set; }
        [ForeignKey("CorporationId")]
        public virtual Corporation Corporation { get; set; }

        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }
        public long Quantity { get; set; }

        public decimal? MEefficiency { get; set; }
        public ProdAchatTypeEnum Type { get; set; }

        public long LocationId { get; set; }
        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }

        public long? ProdAchatParentId { get; set; }
        [ForeignKey("ProdAchatParentId")]
        public virtual ProdAchat ProdAchatParent { get; set; }
        public virtual ICollection<ProdAchat> ProdAchatEnfants { get; set; }

        public ProdAchatStateEnum State { get; set; }

        public int? CharacterIdReservation { get; set; }
        public DateTime? DatetimeReservation { get; set; }

        public ProdAchat()
        {
            ProdAchatEnfants = new HashSet<ProdAchat>();
        }
    }
    public enum ProdAchatTypeEnum
    {
        achat,
        reprocess,
        production,
        invention,
        copy,
    }
    public enum ProdAchatStateEnum
    {
        planifier,
        reserver,
        livraison,
        terminer,
    }
}
