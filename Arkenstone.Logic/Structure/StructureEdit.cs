using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.Esi;
using Arkenstone.Logic.FuzzWork;
using Arkenstone.Logic.GlobalTools;
using ESI.NET.Models.Assets;
using ESI.NET.Models.Industry;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Arkenstone.Logic.Structure
{
    public class StructureEdit
    {
        public static void SetFitToStructure(long StructureId, string CopyPastFit)
        {

            var _dbConnectionString = System.Environment.GetEnvironmentVariable("DB_DATA_connectionstring");
            var options = new DbContextOptionsBuilder<ArkenstoneContext>().UseMySql(_dbConnectionString, ServerVersion.AutoDetect(_dbConnectionString)).Options;
            using (ArkenstoneContext context = new ArkenstoneContext(options))
            {
                var structure = context.Locations.Find(StructureId);
                if (structure == null)
                    throw new Exception("La structure "+ StructureId.ToString() + " n'existe pas");
                
                if (structure.Id <1000000000)
                    throw new Exception(StructureId.ToString() + " est une station.");





                // Récupérer les lignes de la TextBox
                string[] lines = CopyPastFit.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                if(lines.Count()<=1)
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
                    var item = context.Items.FirstOrDefault(x=> x.Name.Trim().Replace(" ", "%20") == keypair.Key.Trim().Replace(" ", "%20"));
                    if (item == null)
                        throw new Exception(keypair.Key + " item inconnu");
                    else
                    {
                        if (!Fit.TryAdd(item, 1))
                            Fit[item] = Fit[item] + 1;
                    }
                }
                //TODO Set rigs
                List<int> fitedRigsManufacturing = new List<int>();
                List<int> AllRigsManufacturing = context.RigsManufacturings.Select(x => x.Id).ToList();
                foreach (var keypair in Fit.Where(x=> AllRigsManufacturing.Contains(x.Key.Id)))
                {
                    fitedRigsManufacturing.Add(keypair.Key.Id);
                }

                context.LocationRigsManufacturings.Where(x => x.LocationId == structure.Id).ToList().ForEach(x => context.LocationRigsManufacturings.Remove(x));

                foreach (var rig in fitedRigsManufacturing)
                {
                    context.LocationRigsManufacturings.Add(new LocationRigsManufacturing() { LocationId = structure.Id, RigsManufacturingId = rig });
                }
                context.SaveChanges();



            }


        }



    }
}
