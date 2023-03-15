using Arkenstone.Entities.DbSet;
using System;
using System.Collections.Generic;

namespace Arkenstone.API.ControllerModel
{
    public class ProdAchatModel
    {
        public int Id { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }

        public decimal? MEefficiency { get; set; }
        public ProdAchatTypeEnum Type { get; set; }
        public virtual StructureModel Location { get; set; }
        public virtual ProdAchatModel ProdAchatParent { get; set; }
        public virtual ICollection<ProdAchatModel> ProdAchatEnfants { get; set; }
        public ProdAchatStateEnum State { get; set; }
        public int? CharacterIdReservation { get; set; }
        public DateTime? DatetimeReservation { get; set; }

        public ProdAchatModel(ProdAchat target)
        {
            this.Id = target.Id;
            this.Item = target.Item;
            this.Quantity = target.Quantity;
            this.MEefficiency = target.MEefficiency;
            this.Type = target.Type;
            this.Location = new StructureModel(target.Location);

            if (target.ProdAchatParent != null)
                this.ProdAchatParent = new ProdAchatModel(target.ProdAchatParent);

            this.ProdAchatEnfants = new List<ProdAchatModel>();
            foreach (var child in target.ProdAchatEnfants)
            {
                this.ProdAchatEnfants.Add(new ProdAchatModel(child));
            }
            this.State = target.State;
            this.CharacterIdReservation = target.CharacterIdReservation;
            this.DatetimeReservation = target.DatetimeReservation;
        }

    }
}
