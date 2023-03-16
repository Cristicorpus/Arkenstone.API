using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using Arkenstone.API.Models;
using EveMiningFleet.API.Models;

namespace Arkenstone.API.Services
{
    public class StructureService
    {
        private ArkenstoneContext _context;

        public StructureService(ArkenstoneContext context)
        {
            _context = context;
        }

        public List<StructureModel> GetBasicModel(long? LocationId)
        {
            return GetCore(LocationId).Select(x => new StructureModel(x)).ToList();
        }

        public List<StructureModelDetails> GetDetailledModel(long? LocationId)
        {
            return GetCore(LocationId).Select(x => new StructureModelDetails(x)).ToList();
        }

        private List<Location> GetCore(long? LocationId)
        {
            if (LocationId == null)
                return _context.Locations.Include("StructureType").ToList();
            else
            {
                List<Location> returnvalue = new List<Location>();
                var targetStructure = _context.Locations.Include("StructureType").FirstOrDefault(x => x.Id == LocationId.Value);
                if (targetStructure != null)
                    returnvalue.Add(targetStructure);
                return returnvalue;
            }

        }
        
        public void SetFitToStructure(long StructureId, string CopyPastFit)
        {

            var structure = _context.Locations.Find(StructureId);
            if (structure == null)
                throw new Exception("La structure " + StructureId.ToString() + " n'existe pas");

            if (structure.Id < 1000000000)
                throw new Exception(StructureId.ToString() + " est une station.");

            // Récupérer les lignes de la TextBox
            string[] lines = CopyPastFit.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            if (lines.Count() <= 1)
                throw new Exception("format de merde refais tout ca");


            // Parcourir chaque ligne pour recuperer les item(text) et leur quantité
            Dictionary<string, int> Fitraw = new Dictionary<string, int>();
            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i] != "")
                {

                    int quantity = 0;
                    string itemName = "";

                    if (lines[i].Contains(" x"))
                    {
                        try
                        {

                            var itemraw = lines[i].Split(" x");
                            quantity = Convert.ToInt32(itemraw[1]);
                            itemName = itemraw[0];
                        }
                        catch (Exception)
                        {

                        }
                    }

                    if (quantity == 0)
                    {
                        quantity = 1;
                        itemName = lines[i];
                    }

                    if (!Fitraw.TryAdd(itemName, quantity))
                        Fitraw[itemName] = quantity + 1;

                }
            }

            Dictionary<Entities.DbSet.Item, int> Fit = new Dictionary<Entities.DbSet.Item, int>();
            foreach (var keypair in Fitraw)
            {
                var item = _context.Items.FirstOrDefault(x => x.Name.Trim().Replace(" ", "%20") == keypair.Key.Trim().Replace(" ", "%20"));
                if (item == null)
                    throw new Exception(keypair.Key + " item inconnu");
                else
                {
                    if (!Fit.TryAdd(item, 1))
                        Fit[item] = Fit[item] + 1;
                }
            }
            
            List<int> fitedRigsManufacturing = new List<int>();
            List<int> AllRigsManufacturing = _context.RigsManufacturings.Select(x => x.Id).ToList();
            foreach (var keypair in Fit.Where(x => AllRigsManufacturing.Contains(x.Key.Id)))
            {
                fitedRigsManufacturing.Add(keypair.Key.Id);
            }
            _context.LocationRigsManufacturings.Where(x => x.LocationId == structure.Id).ToList().ForEach(x => _context.LocationRigsManufacturings.Remove(x));

            foreach (var rig in fitedRigsManufacturing)
            {
                _context.LocationRigsManufacturings.Add(new LocationRigsManufacturing() { LocationId = structure.Id, RigsManufacturingId = rig });
            }
            _context.SaveChanges();


        }




    }
}
