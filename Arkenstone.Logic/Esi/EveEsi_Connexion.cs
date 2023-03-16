using Arkenstone;
using ESI.NET;
using ESI.NET.Enumerations;
using ESI.NET.Models.SSO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Arkenstone.Logic.Esi
{
    public class EveEsiConnexion
    {
        public EsiClient EsiClient;
        public SsoToken ssoToken { get; set; }
        public AuthorizedCharacterData authorizedCharacterData { get; set; }

        private IOptions<EsiConfig> EsiConfiguration { get; set; }

        #region Constructors
        public EveEsiConnexion()
        {
            EsiConfiguration = Options.Create(new EsiConfig()
            {
                EsiUrl = "https://esi.evetech.net/",
                DataSource = DataSource.Tranquility,
                ClientId = Environment.GetEnvironmentVariable("ESIclientId"),
                SecretKey = Environment.GetEnvironmentVariable("EsiSecret"),
                CallbackUrl = Environment.GetEnvironmentVariable("EsiCallBack"),
                UserAgent = "Arkenstone"
            });

            Open();
        }
        #endregion

        private void Open()
        {
            try
            {
                EsiClient = new EsiClient(EsiConfiguration);
            }
            catch (Exception ex)
            {
                Logs.ClassLog.writeException(ex);
                throw ex;
            }
        }
        public string GetUrlConnection(string state = "0")
        {
            List<string> scopes = new List<string>();
            scopes.Add("publicData");
            scopes.Add("esi-universe.read_structures.v1");
            scopes.Add("esi-assets.read_assets.v1");
            scopes.Add("esi-industry.read_character_jobs.v1");
            scopes.Add("esi-characters.read_blueprints.v1");
            scopes.Add("esi-assets.read_corporation_assets.v1");
            scopes.Add("esi-corporations.read_blueprints.v1");
            scopes.Add("esi-industry.read_corporation_jobs.v1");

            return this.EsiClient.SSO.CreateAuthenticationUrl(scopes,state);
        }

        public async Task GetToken(string acodeEsiReturn)
        {
            try
            {
                ssoToken = await EsiClient.SSO.GetToken(GrantType.AuthorizationCode, acodeEsiReturn);
                await ConnectCharCCP();
            }
            catch
            {
                ssoToken = null;
            }
        }
        public async Task RefreshConnection(string arefreshtoken)
        {
            try
            {
                ssoToken = await EsiClient.SSO.GetToken(GrantType.RefreshToken, arefreshtoken);
                await ConnectCharCCP();
            }
            catch
            {
                ssoToken = null;
            }
        }
        public async Task ConnectCharCCP()
        {
            try
            {
                authorizedCharacterData = await EsiClient.SSO.Verify(ssoToken);
                EsiClient.SetCharacterData(authorizedCharacterData);
            }
            catch
            {
                authorizedCharacterData = null;
            }
        }
    }
}