using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using Arkenstone.API.Models;
using EveMiningFleet.API.Models;
using Arkenstone.Logic.BusinessException;
using Microsoft.AspNetCore.Mvc;

namespace Arkenstone.API.Services
{
    public class LocationService
    {
        private ArkenstoneContext _context;

        public LocationService(ArkenstoneContext context)
        {
            _context = context;
        }
        private IQueryable<Location> RequestLocAndSubLoc()
        {
            return _context.Locations.Include("StructureType.Item").Include("SubLocations");
        }

        private IQueryable<Location> RequestLocFit()
        {
            return RequestLocAndSubLoc().Include("LocationRigsManufacturings.RigsManufacturing");
        }

        public List<Location> GetList(int corpID)
        {
            var temp = RequestLocFit().Where(x => x.SubLocations.Any(y => y.CorporationId == corpID)).ToList();
            if (temp.Count() == 0)
                throw new NoContent("Location");
            return temp;
        }
        public Location Get(long LocationID)
        {
            Location temp = RequestLocFit().FirstOrDefault(x => x.Id == LocationID);
            if (temp == null)
                throw new NotFound("Location");
            return temp;
        }


        public void SetFitToStructure(Location location, string CopyPastFit)
        {
            if (location.Id < 1000000000)
                throw new BadRequestException(location.ToString() + " isn t an struture, is an station.");

            
            // Récupérer les lignes de la TextBox
            string[] lines = CopyPastFit.Split(new[] { "\r\n" }, StringSplitOptions.None);

            if (lines.Count() <= 1)
                throw new BadRequestException("format de merde refais tout ca");


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
                    throw new BadRequestException(keypair.Key + " item inconnu");
                else
                {
                    if (!Fit.TryAdd(item, 1))
                        Fit[item] = Fit[item] + 1;
                }
            }

            List<int> fitedRigsManufacturing = new List<int>();
            List<int> AllRigsManufacturing = _context.RigsManufacturings.Select(x => x.ItemId).ToList();
            foreach (var keypair in Fit.Where(x => AllRigsManufacturing.Contains(x.Key.Id)))
            {
                fitedRigsManufacturing.Add(keypair.Key.Id);
            }
            _context.LocationRigsManufacturings.Where(x => x.LocationId == location.Id).ToList().ForEach(x => _context.LocationRigsManufacturings.Remove(x));

            foreach (var rig in fitedRigsManufacturing)
            {
                _context.LocationRigsManufacturings.Add(new LocationRigsManufacturing() { LocationId = location.Id, RigsManufacturingId = rig });
            }
            _context.SaveChanges();


        }

    }

    public static class LocationExtension
    {
        public static Location ThrowNotAuthorized(this Location location,int corpId)
        {
            if (!location.SubLocations.Any(x => x.CorporationId == corpId))
                throw new NotAuthorized();
            return location;
        }
    }

}
