using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.Esi;
using ESI.NET.Models.Character;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkenstone.Logic.BulkUpdate
{
    public static class TokenCheck
    {

        /// <summary>
        /// refresh tout les scopes de tout le monde
        /// </summary>
        public static void checkAllToken()
        {
            Logs.ClassLog.writeLog("checkAllToken => Begin...");
            
            var _dbConnectionString = System.Environment.GetEnvironmentVariable("DB_DATA_connectionstring");
            var options = new DbContextOptionsBuilder<ArkenstoneContext>().UseMySql(_dbConnectionString, ServerVersion.AutoDetect(_dbConnectionString)).Options;
            using (ArkenstoneContext context = new ArkenstoneContext(options))
            {
                foreach (var character in context.Characters.Where(x => x.Token != "" || x.CharacterMainId != x.Id))
                    checkToken(context, character);

                context.SaveChanges();
            }
            
        }
        /// <summary>
        /// Verifie si l'utilisateur est encore accessible
        /// </summary>
        private static void checkToken(ArkenstoneContext context, Entities.DbSet.Character character)
        {
            if (character.Token != "")
            {
                EveEsiConnexion tmpEsiConnection = new EveEsiConnexion();
                var countertest = 0;

                //TODO: me parait pas propre quand meme .....
                do
                {
                    tmpEsiConnection.RefreshConnection(character.RefreshToken).Wait();
                    countertest++;
                } while (countertest < 5 && tmpEsiConnection.authorizedCharacterData == null);

                if (tmpEsiConnection.authorizedCharacterData == null)
                {
                    character.Token = "";
                    character.RefreshToken = "";
                    character.CharacterMainId = character.Id;
                }
                else
                {
                    if (character.CharacterMainId == 0)
                        character.CharacterMainId = character.Id;
                }
            }

            if (character.Token == "" && character.CharacterMainId != character.Id)
                character.CharacterMainId = character.Id;

        }

    }
}
