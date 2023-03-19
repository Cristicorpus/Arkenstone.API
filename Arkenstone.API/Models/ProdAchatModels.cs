using Arkenstone.Entities.DbSet;
using System;
using System.Collections.Generic;

namespace Arkenstone.API.Models
{
    public class ProdAchatModel
    {
        public ProdAchatModel()
        { }
        public long Id { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }

        public decimal? MEefficiency { get; set; }
        public ProdAchatTypeEnum Type { get; set; }
        public LocationModel Location { get; set; }
        public ProdAchatModel ProdAchatParent { get; set; }
        public List<ProdAchatModel> ProdAchatEnfants { get; set; }
        public ProdAchatStateEnum State { get; set; }
        public int? CharacterIdReservation { get; set; }
        public DateTime? DatetimeReservation { get; set; }
        
        public ProdAchatModel(ProdAchat target, ProdAchatModelRecursiv recursive = ProdAchatModelRecursiv.standart)
        {
            this.Id = target.Id;
            this.Item = target.Item;
            this.Quantity = target.Quantity;
            this.MEefficiency = target.MEefficiency;
            this.Type = target.Type;
            this.Location = new LocationModel(target.Location);

            if ((recursive == ProdAchatModelRecursiv.standart || recursive == ProdAchatModelRecursiv.up) && target.ProdAchatParent != null)
                this.ProdAchatParent = new ProdAchatModel(target.ProdAchatParent, ProdAchatModelRecursiv.up);

            if (recursive == ProdAchatModelRecursiv.standart || recursive == ProdAchatModelRecursiv.down)
            {
                this.ProdAchatEnfants = new List<ProdAchatModel>();
                foreach (var child in target.ProdAchatEnfants)
                {
                    this.ProdAchatEnfants.Add(new ProdAchatModel(child, ProdAchatModelRecursiv.down));
                }
            }
            this.State = target.State;
            this.CharacterIdReservation = target.CharacterIdReservation;
            this.DatetimeReservation = target.DatetimeReservation;
        }
        public enum ProdAchatModelRecursiv
        {
            none,
            standart,
            up,
            down
        }
    }
    
}
