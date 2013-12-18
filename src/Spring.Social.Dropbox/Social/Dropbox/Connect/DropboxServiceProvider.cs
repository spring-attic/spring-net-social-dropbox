#region License

/*
 * Copyright 2002-2012 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

using System;

using Spring.Social.OAuth1;
using Spring.Social.Dropbox.Api;
using Spring.Social.Dropbox.Api.Impl;
using System.Net;
using System.IO;
using System.Text;

namespace Spring.Social.Dropbox.Connect
{
    /// <summary>
    /// Dropbox <see cref="IServiceProvider"/> implementation.
    /// </summary>
    /// <author>Bruno Baia</author>
    public class DropboxServiceProvider : AbstractOAuth1ServiceProvider<IDropbox>
    {
        private AccessLevel accessLevel;

        /// <summary>
        /// Gets the application access level. 
        /// </summary>
        public AccessLevel AccessLevel
        {
            get { return accessLevel; }
        }

        /// <summary>
        /// Creates a new instance of <see cref="DropboxServiceProvider"/>.
        /// </summary>
        /// <param name="consumerKey">The application's API key.</param>
        /// <param name="consumerSecret">The application's API secret.</param>
        /// <param name="accessLevel">The application access level.</param>
        public DropboxServiceProvider(string consumerKey, string consumerSecret, AccessLevel accessLevel)
            : base(consumerKey, consumerSecret, new OAuth1Template(consumerKey, consumerSecret,
                "https://api.dropbox.com/1/oauth/request_token",
                "https://www.dropbox.com/1/oauth/authorize",
                "https://api.dropbox.com/1/oauth/access_token",
                OAuth1Version.Core10))
        {
            this.accessLevel = accessLevel;
        }

        /// <summary>
        /// Returns an API interface allowing the client application to access protected resources on behalf of a user.
        /// </summary>
        /// <param name="accessToken">The API access token.</param>
        /// <param name="secret">The access token secret.</param>
        /// <returns>A binding to the service provider's API.</returns>
        public override IDropbox GetApi(string accessToken, string secret)
        {
            return new DropboxTemplate(this.ConsumerKey, this.ConsumerSecret, accessToken, secret, this.accessLevel);
        }

        /// <summary>
        /// Links application to user's account
        /// </summary>
        /// <param name="email">user's account email</param>
        /// <param name="password">user's account password</param>
        /// <returns></returns>
        public OAuthToken LinkAccount(string email, string password)
        {
            OAuthToken oauthToken = this.OAuthOperations.FetchRequestTokenAsync(null, null).Result;
            OAuth1Parameters parameters = new OAuth1Parameters();
            string authenticateUrl = this.OAuthOperations.BuildAuthorizeUrl(oauthToken.Value, parameters);

            if (LinkAccount(email, password, authenticateUrl))
            {
                AuthorizedRequestToken requestToken = new AuthorizedRequestToken(oauthToken, null);
                return this.OAuthOperations.ExchangeForAccessTokenAsync(requestToken, null).Result;
            }

            return null;
        }

        private bool LinkAccount(string email, string password, string authenticateURL)
        {
            CookieContainer cookieContainer = new CookieContainer();

            bool userLoggedIn = LogUser(email, password, cookieContainer);

            if (!userLoggedIn)
            {
                return false;
            }

            string authorizePageHTML = Get(cookieContainer, authenticateURL);

            string t = GetHiddenFieldValue(authorizePageHTML, "t");
            string oauthToken = GetHiddenFieldValue(authorizePageHTML, "oauth_token");
            string postData = string.Format("t={0}&allow_access=Allow&osx_protocol=&oauth_token={1}&display=&embedded=&oauth_callback=", t, oauthToken);

            HttpWebResponse authenticationRespone = Post(postData, cookieContainer, "https://www.dropbox.com/1/oauth/authorize");
            return authenticationRespone.StatusCode == HttpStatusCode.OK;
        }

        private bool LogUser(string email, string password, CookieContainer cookieContainer)
        {
            const string LOGIN_URL = "https://www.dropbox.com/login";

            string loginPageHTML = Get(cookieContainer, LOGIN_URL);

            string t = GetHiddenFieldValue(loginPageHTML, "t");
            email = Uri.EscapeDataString(email);
            password = Uri.EscapeDataString(password);
            string postData = string.Format("t={0}&lhs_type=default&display=desktop&login_email={1}&login_password={2}&login_submit=1&login_submit_dummy=Sign+in", t, email, password);

            HttpWebResponse loginRespone = Post(postData, cookieContainer, LOGIN_URL);
            using (Stream stream_ = loginRespone.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream_))
                {
                    var htmlPage = reader.ReadToEnd();

                    if (htmlPage.Contains("error-message"))
                    {
                        throw new ArgumentException("Invalid e-mail or password");
                    }
                }
            }

            return true;
        }

        private string Get(CookieContainer cookieContainer, string url)
        {
            HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(url);
            getRequest.CookieContainer = cookieContainer;
            HttpWebResponse response = (HttpWebResponse)getRequest.GetResponse();
            string htmlInput;

            using (Stream stream_ = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream_))
                {
                    htmlInput = reader.ReadToEnd();
                }
            }

            return htmlInput;
        }

        private HttpWebResponse Post(string postData, CookieContainer cookieContainer, string url)
        {
            HttpWebRequest postRequest = (HttpWebRequest)WebRequest.Create(url);
            postRequest.CookieContainer = cookieContainer;

            byte[] data = new ASCIIEncoding().GetBytes(postData);

            postRequest.Method = "POST";
            postRequest.ContentType = "application/x-www-form-urlencoded";
            postRequest.ContentLength = data.Length;

            using (Stream stream = postRequest.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse loginRespone = (HttpWebResponse)postRequest.GetResponse();
            return loginRespone;
        }

        private string GetHiddenFieldValue(string htmlInput, string hiddenFieldName)
        {
            var token = string.Format("<input type=\"hidden\" name=\"{0}\" value=\"", hiddenFieldName);
            int startIndex = htmlInput.IndexOf(token);
            startIndex += token.Length;
            int endIndex = htmlInput.IndexOf('"', startIndex);
            return htmlInput.Substring(startIndex, endIndex - startIndex);
        }
    }
}
