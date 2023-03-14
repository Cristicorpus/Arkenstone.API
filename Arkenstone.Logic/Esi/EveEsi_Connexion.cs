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
    public class EveEsi_Connexion
    {
        public EsiClient EsiClient;
        public SsoToken ssoToken { get; set; }
        public AuthorizedCharacterData authorizedCharacterData { get; set; }

        private IOptions<EsiConfig> EsiConfiguration { get; set; }

        #region Constructors
        public EveEsi_Connexion()
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
        public string GetUrlConnection()
        {
            List<string> scopes = new List<string>();
            scopes.Add("publicData");
            scopes.Add("esi-assets.read_assets.v1");
            scopes.Add("esi-industry.read_character_jobs.v1");
            scopes.Add("esi-characters.read_blueprints.v1");
            scopes.Add("esi-assets.read_corporation_assets.v1");
            scopes.Add("esi-corporations.read_blueprints.v1");
            scopes.Add("esi-industry.read_corporation_jobs.v1");

            return CreateAuthenticationUrl(scopes);
        }
        public string CreateAuthenticationUrl(List<string> scope = null, string state = null)
        {
            DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(72, 3);

            var _ssoUrl = "";
            switch (EsiConfiguration.Value.DataSource)
            {
                case DataSource.Tranquility:
                    _ssoUrl = "login.eveonline.com";
                    break;
                case DataSource.Singularity:
                    _ssoUrl = "login.eveonline.com";
                    break;
                case DataSource.Serenity:
                    _ssoUrl = "login.evepc.163.com";
                    break;
            }


            defaultInterpolatedStringHandler.AppendLiteral("https://");
            defaultInterpolatedStringHandler.AppendFormatted(_ssoUrl);
            defaultInterpolatedStringHandler.AppendLiteral("/v2/oauth/authorize/?response_type=code&redirect_uri=");
            defaultInterpolatedStringHandler.AppendFormatted(Uri.EscapeDataString(EsiConfiguration.Value.CallbackUrl));
            defaultInterpolatedStringHandler.AppendLiteral("&client_id=");
            defaultInterpolatedStringHandler.AppendFormatted(EsiConfiguration.Value.ClientId);
            string text = defaultInterpolatedStringHandler.ToStringAndClear();
            if (scope != null)
            {
                text = text + "&scope=" + string.Join("+", scope.Distinct().ToList());
            }

            if (state != null)
                text = text + "&state=" + state;
            else
                text = text + "&state=0";


            return text;
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