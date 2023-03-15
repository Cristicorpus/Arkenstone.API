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
        public static async Task RefreshAllNameAsync()
        {
            var _dbConnectionString = System.Environment.GetEnvironmentVariable("DB_DATA_connectionstring");
            var options = new DbContextOptionsBuilder<ArkenstoneContext>().UseMySql(_dbConnectionString, ServerVersion.AutoDetect(_dbConnectionString)).Options;
            using (ArkenstoneContext context = new ArkenstoneContext(options))
            {
                var eveEsi = new EveEsi_Connexion();
                //ajout des stations et structure inconnu
                foreach (var structure in context.Locations)
                {
                    foreach (var character in context.Characters)
                    {
                        try
                        {
                            await eveEsi.RefreshConnection(character.RefreshToken);
                            //TODO ajouter le scope pour lire les structure
                            //var temp = await eveEsi.EsiClient.Universe.Structure(structure.Id);
                            //if (temp.Data != null)
                            //{
                            //    structure.Name = temp.Data.Name;
                            //    break;
                            //}
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                }
                context.SaveChanges();
            }
        }



        public static void SetFitToStructure(string CopyPastFit)
        {
            string shipName = "";
            Dictionary<string, int> Fitraw = new Dictionary<string, int>();

            // Récupérer les lignes de la TextBox
            string[] lines = CopyPastFit.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            // recuperation du nom du vaisseau
            var shipraw = lines[0].Replace("[", "").Replace("]", "").Split(",");
            shipName = shipraw[1];

            // Parcourir chaque ligne pour recuperer les item(text) et leur quantité
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

            var _dbConnectionString = System.Environment.GetEnvironmentVariable("DB_DATA_connectionstring");
            var options = new DbContextOptionsBuilder<ArkenstoneContext>().UseMySql(_dbConnectionString, ServerVersion.AutoDetect(_dbConnectionString)).Options;
            using (ArkenstoneContext context = new ArkenstoneContext(options))
            {
                Dictionary<Entities.DbSet.Item, int> Fit = new Dictionary<Entities.DbSet.Item, int>();
                foreach (var keypair in Fitraw)
                {
                    var itemID = Retry.Do(() => FuzzWorkTools.GetIdByName(keypair.Key.Replace(" ", "%20")), TimeSpan.FromMilliseconds(50), 3);
                    if (itemID == 0)
                        throw new Exception(keypair.Key + " item inconnu");

                    var item = context.Items.Find(itemID);
                    if (item != null)
                    {
                        if (!Fit.TryAdd(item, 1))
                            Fit[item] = Fit[item] + 1;
                    }
                }

                //TODO SetStationType and bonus



                //TODO Set rigs
                List<int> AllRigs = new List<int>();                
                foreach (var keypair in Fit.Where(x=> AllRigs.Contains(x.Key.Id)))
                {

                }

            }


        }



    }
}
