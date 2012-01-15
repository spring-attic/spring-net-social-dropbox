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

using NUnit.Framework;
using Spring.Rest.Client.Testing;

using Spring.IO;
using Spring.Http;

namespace Spring.Social.Dropbox.Api.Impl
{
    /// <summary>
    /// Unit tests for the DropboxTemplate class.
    /// </summary>
    /// <author>Bruno Baia</author>
    [TestFixture]
    public class DropboxTemplateTests
    {
        protected DropboxTemplate dropbox;
        protected MockRestServiceServer mockServer;
        protected HttpHeaders responseHeaders;

        [SetUp]
        public void Setup()
        {
            dropbox = new DropboxTemplate("CONSUMER_KEY", "CONSUMER_SECRET", "ACCESS_TOKEN", "ACCESS_TOKEN_SECRET");
            mockServer = MockRestServiceServer.CreateServer(dropbox.RestTemplate);
            responseHeaders = new HttpHeaders();
            responseHeaders.ContentType = MediaType.APPLICATION_JSON;
        }

        [TearDown]
        public void TearDown()
        {
            mockServer.Verify();
        }

        [Test]
	    public void IsAuthorizedForUser() 
        {
		    DropboxTemplate dropbox = new DropboxTemplate("API_KEY", "API_SECRET", "ACCESS_TOKEN", "ACCESS_TOKEN_SECRET");
		    Assert.IsTrue(dropbox.IsAuthorized);
	    }

        [Test]
        public void GetUserProfile()
        {
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api.dropbox.com/1/account/info")
                .AndExpectMethod(HttpMethod.GET)
                .AndRespondWith(JsonResource("Dropbox_Profile"), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
            DropboxProfile profile = dropbox.GetUserProfileAsync().Result;
#else
            DropboxProfile profile = dropbox.GetUserProfile();
#endif
            Assert.AreEqual("US", profile.Country);
            Assert.AreEqual("John P. User", profile.DisplayName);
            Assert.AreEqual("john@example.com", profile.Email);
            Assert.AreEqual(12345678, profile.ID);
            Assert.AreEqual(107374182400000, profile.Quota);
            Assert.AreEqual(680031877871, profile.QuotaNormal);
            Assert.AreEqual(253738410565, profile.QuotaShared);
            Assert.AreEqual("https://www.dropbox.com/referrals/r1a2n3d4m5s6t7", profile.ReferralLink);
        }


        // tests helpers

        private IResource JsonResource(string filename)
        {
            return new AssemblyResource("assembly://Spring.Social.Dropbox.Tests/Spring.Social.Dropbox.Api.Impl/" + filename + ".json");
        }
    }
}
