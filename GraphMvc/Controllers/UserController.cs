using GraphMvc.Helpers;
using GraphMvc.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GraphMvc.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var token = await GetAccessToken();
            // wait for model class 'Models/UserDetailModel' to be populated....
            var user = await UserDetailModel.GetUserDetail("me", token.AccessToken);

            // pass model data to 'Views/User/Index'
            return View(user);
        }

        [Authorize]
        public async Task<ActionResult> Detail(Guid id)
        {
            var token = await GetAccessToken();
            // wait for model class 'Models/UserDetailModel' to be populated....
            var user = await UserDetailModel.GetUserDetail(String.Format("{0}/users/{1}", SettingsHelper.AzureAdTenant, id.ToString()), token.AccessToken);

            // pass model data to 'Views/User/Detail'
            return View(user);
        }

        private async Task<AuthenticationResult> GetAccessToken()
        {
            AuthenticationContext context = new AuthenticationContext(SettingsHelper.AzureADAuthority);
            var clientCredential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientSecret);

            // get the Session[User_Token] saved from 'Startup.Auth.cs'
            AuthenticationResult result = (AuthenticationResult)this.Session[SettingsHelper.UserTokenCacheKey];
            return await context.AcquireTokenByRefreshTokenAsync(result.RefreshToken, clientCredential, SettingsHelper.UnifiedApiResource);
        }
    }
}