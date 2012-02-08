using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Spring.Social.OAuth1;
using Spring.Social.Dropbox.Api;
using Spring.Social.Dropbox.Connect;

namespace Spring.MvcQuickStart.Controllers
{
    public class DropboxController : Controller
    {
        // Register your own Dropbox app at https://www.dropbox.com/developers/apps with "Full Dropbox" access level.
        // Set your consumer key & secret here
        private const string DropboxAppKey = TODO;
        private const string DropboxAppSecret = TODO;

        IOAuth1ServiceProvider<IDropbox> dropboxProvider =
            new DropboxServiceProvider(DropboxAppKey, DropboxAppSecret, AccessLevel.Full);

        // GET: /Dropbox/Index
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Dropbox/SignIn
        public ActionResult SignIn()
        {
            OAuthToken requestToken = dropboxProvider.OAuthOperations.FetchRequestTokenAsync(null , null).Result;
            Session["RequestToken"] = requestToken;

            OAuth1Parameters parameters = new OAuth1Parameters();
            parameters.CallbackUrl = "http://localhost/Dropbox/Callback";
            //parameters.Add("locale", System.Globalization.CultureInfo.CurrentUICulture.IetfLanguageTag); // for a localized version of the authorization website
            return Redirect(dropboxProvider.OAuthOperations.BuildAuthenticateUrl(requestToken.Value, parameters));
        }

        // GET: /Dropbox/Callback
        public ActionResult Callback()
        {
            OAuthToken requestToken = Session["RequestToken"] as OAuthToken;
            AuthorizedRequestToken authorizedRequestToken = new AuthorizedRequestToken(requestToken, null);
            OAuthToken token = dropboxProvider.OAuthOperations.ExchangeForAccessTokenAsync(authorizedRequestToken, null).Result;

            Session["TokenValue"] = token.Value;
            Session["TokenSecret"] = token.Secret;

            IDropbox dropboxClient = dropboxProvider.GetApi(token.Value, token.Secret);
            DropboxProfile profile = dropboxClient.GetUserProfileAsync().Result;
            return View(profile);
        }

        // GET: /Dropbox/Browser
        public ActionResult Browser(string path)
        {
            string tokenValue = Session["TokenValue"] as string;
            string tokenSecret = Session["TokenSecret"] as string;
            IDropbox dropboxClient = dropboxProvider.GetApi(tokenValue, tokenSecret);
            //dropboxClient.Locale = System.Globalization.CultureInfo.CurrentUICulture.IetfLanguageTag; // for a localized version of the content response

            Entry root = dropboxClient.GetMetadataAsync(path).Result;
            return View(root);
        }
    }
}
