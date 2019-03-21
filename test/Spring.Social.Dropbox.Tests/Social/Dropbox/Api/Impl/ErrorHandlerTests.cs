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
using System.Net;

using NUnit.Framework;
using Spring.Rest.Client.Testing;

using Spring.Http;

namespace Spring.Social.Dropbox.Api.Impl
{
    /// <summary>
    /// Unit tests for the DropboxErrorHandler class.
    /// </summary>
    /// <author>Bruno Baia</author>
    [TestFixture]
    public class ErrorHandlerTests 
    {
        protected DropboxTemplate dropbox;
        protected MockRestServiceServer mockServer;
        protected HttpHeaders responseHeaders;

        [SetUp]
        public void Setup()
        {
            dropbox = new DropboxTemplate("CONSUMER_KEY", "CONSUMER_SECRET", "ACCESS_TOKEN", "ACCESS_TOKEN_SECRET", AccessLevel.AppFolder);
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
        public void PathNotFound() 
        {
		    mockServer.ExpectNewRequest()
                .AndExpectUri("https://api.dropbox.com/1/metadata/sandbox/Spring Social/Temp.txt")
			    .AndExpectMethod(HttpMethod.GET)
			    .AndRespondWith("{\"error\": \"Path '/Spring Social/Temp.txt' not found\"}", responseHeaders, HttpStatusCode.NotFound, "");

#if NET_4_0 || SILVERLIGHT_5
            dropbox.GetMetadataAsync("/Spring Social/Temp.txt")
                .ContinueWith(task =>
                {
                    AssertDropboxApiException(task.Exception, "Path '/Spring Social/Temp.txt' not found", DropboxApiError.PathNotFound);
                })
                .Wait();
#else
            try
            {
                dropbox.GetMetadata("/Spring Social/Temp.txt");
                Assert.Fail("Exception expected");
            }
            catch (Exception ex)
            {
                AssertDropboxApiException(ex, "Path '/Spring Social/Temp.txt' not found", DropboxApiError.PathNotFound);
            }
#endif
        }


        // tests helpers

#if NET_4_0 || SILVERLIGHT_5
        private void AssertDropboxApiException(AggregateException ae, string expectedMessage, DropboxApiError error)
        {
            ae.Handle(ex =>
            {
                if (ex is DropboxApiException)
                {
                    Assert.AreEqual(expectedMessage, ex.Message);
                    Assert.AreEqual(error, ((DropboxApiException)ex).Error);
                    return true;
                }
                return false;
            });
        }
#else
        private void AssertDropboxApiException(Exception ex, string expectedMessage, DropboxApiError error)
        {
            if (ex is DropboxApiException)
            {
                Assert.AreEqual(expectedMessage, ex.Message);
                Assert.AreEqual(error, ((DropboxApiException)ex).Error);
            }
            else
            {
                Assert.Fail("DropboxApiException expected");
            }
        }
#endif
    }
}
