using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arkenstone.Entities.DbSet;
using Arkenstone.Entities;

namespace Arkenstone.ControllerModel
{
    public class TicketModel
    {
        public int Id { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }
        public virtual Ticket TicketParent { get; set; }
        public StateEnum State { get; set; }
        public int CharacterId { get; set; }
        public virtual ICollection<Ticket> TicketEnfant { get; set; }

        public TicketModel()
        {

        }

        public TicketModel(Ticket target)
        {
            this.Id = target.Id;
            this.Item = target.Item;
            this.Quantity = target.Quantity;
            this.TicketParent = target.TicketParent;
            this.State = target.State;
            this.CharacterId = target.CharacterId;
            this.TicketEnfant = target.TicketEnfant;
        }
    }

    public class TicketModelPost
    {
        public int Item { get; set; }
        public int Quantity { get; set; }
        public int CharacterId { get; set; }

        public TicketModelPost()
        {

        }

        public Ticket getTicketDb(ArkenstoneContext context, bool subticket = false, bool addContext = false)
        {
            Ticket ticket = new Ticket();

            ticket.Item = context.Items.Single(x => x.Id == this.Item);
            ticket.Quantity = this.Quantity;
            ticket.State = StateEnum.Production;
            ticket.CharacterId = this.CharacterId;
            if (subticket)
            {
                ticket.TicketEnfant = getSubTicketDb(ticket, context, addContext);
            }

            return ticket;
        }

        public List<Ticket> getSubTicketDb(Ticket parent, ArkenstoneContext context, bool addContext)
        {
            List<Ticket> tickets = new List<Ticket>();
            Recipe recipe = context.Recipes.SingleOrDefault(x => x.Id == parent.Item.Id);

            if(recipe != null)
            {
                foreach (var recipeRessource in recipe.RecipeRessource)
                {
                    Recipe subRecipe = context.Recipes.SingleOrDefault(x => x.ItemId == recipeRessource.ItemId);
                    if (subRecipe != null)
                    {
                        Ticket ticket = new Ticket();
                        
                        ticket.Item = context.Items.Single(x => x.Id == recipeRessource.ItemId);
                        ticket.Quantity = recipeRessource.Quantity * parent.Quantity;
                        ticket.TicketParent = parent;
                        ticket.State = StateEnum.Production;
                        ticket.CharacterId = this.CharacterId;
                        ticket.TicketEnfant = getSubTicketDb(ticket, context, addContext);

                        tickets.Add(ticket);
                    }
                    
                }
            }

            return tickets;
        }
    }
}
