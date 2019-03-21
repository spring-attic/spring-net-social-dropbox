#region License

/*
 * Copyright 2002-2012 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      https://www.apache.org/licenses/LICENSE-2.0
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
    }
}
