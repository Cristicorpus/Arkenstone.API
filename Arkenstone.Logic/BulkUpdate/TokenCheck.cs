using Arkenstone.Logic.Entities;
using Arkenstone.Logic.Esi;
using Arkenstone.Logic.GlobalTools;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Arkenstone.Logic.Repository;

namespace Arkenstone.Logic.BulkUpdate
{
    public static class CCPTokenCheck
    {

        /// <summary>
        /// refresh tout les scopes de tout le monde
        /// </summary>
        public static void checkAllToken()
        {
            Logs.ClassLog.writeLog("checkAllToken => Begin...");

            using (ArkenstoneContext context = ArkenstoneContext.GetContextWithDefaultOption())
            {
                foreach (var character in context.Characters.Where(x => x.Token != "" || x.CharacterMainId != x.Id))
                    checkToken(context, character);

                context.SaveChanges();
            }
            
        }
        /// <summary>
        /// Verifie si l'utilisateur est encore accessible
        /// </summary>
        private static void checkToken(ArkenstoneContext context, Character character)
        {
            CharacterRepository characterRepository = new CharacterRepository(context);
            if (character.Token != "")
            {
                EveEsiConnexion tmpEsiConnection = new EveEsiConnexion();

                tmpEsiConnection.RefreshConnection(character.RefreshToken).Wait();

                if (tmpEsiConnection.authorizedCharacterData == null)
                    characterRepository.ClearToken(character);
            }

            if (character.CharacterMainId == 0 || (character.Token == "" && character.CharacterMainId != character.Id))
                characterRepository.ResetMain(character);
        }

    }
}
