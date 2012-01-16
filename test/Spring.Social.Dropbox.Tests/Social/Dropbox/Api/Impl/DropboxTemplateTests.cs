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
using System.Net;

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
            dropbox = new DropboxTemplate("CONSUMER_KEY", "CONSUMER_SECRET", "ACCESS_TOKEN", "ACCESS_TOKEN_SECRET", AccessLevel.Full);
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
            DropboxTemplate dropbox = new DropboxTemplate("API_KEY", "API_SECRET", "ACCESS_TOKEN", "ACCESS_TOKEN_SECRET", AccessLevel.Full);
		    Assert.IsTrue(dropbox.IsAuthorized);
	    }

        [Test]
        public void GetUserProfile()
        {
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api.dropbox.com/1/account/info")
                .AndExpectMethod(HttpMethod.GET)
                .AndRespondWith(EmbeddedResource("Dropbox_Profile.json"), responseHeaders);

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

        [Test]
        public void CreateFolder()
        {
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api.dropbox.com/1/fileops/create_folder")
                .AndExpectMethod(HttpMethod.POST)
                .AndExpectBody("root=dropbox&path=new_folder")
                .AndRespondWith(EmbeddedResource("Create_Folder.json"), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
            Entry metadata = dropbox.CreateFolderAsync("new_folder").Result;
#else
            Entry metadata = dropbox.CreateFolder("new_folder");
#endif
            Assert.AreEqual(0, metadata.Bytes);
            Assert.IsNull(metadata.Hash);
            Assert.AreEqual("folder", metadata.Icon);
            Assert.AreEqual(false, metadata.IsDeleted);
            Assert.AreEqual(true, metadata.IsDirectory);
            Assert.IsNull(metadata.MimeType);
            Assert.IsNotNull(metadata.ModifiedDate);
            Assert.AreEqual("10/08/2011 18:21:30", metadata.ModifiedDate.Value.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss"));
            Assert.AreEqual("/new_folder", metadata.Path);
            Assert.AreEqual("1f477dd351f", metadata.Revision);
            Assert.AreEqual("dropbox", metadata.Root);
            Assert.AreEqual("0 bytes", metadata.Size);
            Assert.IsFalse(metadata.ThumbExists);
            Assert.IsNull(metadata.Contents);
        }

        [Test]
        public void Delete() // Delete file
        {
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api.dropbox.com/1/fileops/delete")
                .AndExpectMethod(HttpMethod.POST)
                .AndExpectBody("root=dropbox&path=test+.txt")
                .AndRespondWith(EmbeddedResource("Delete.json"), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
            Entry metadata = dropbox.DeleteAsync("test .txt").Result;
#else
            Entry metadata = dropbox.Delete("test .txt");
#endif
            Assert.AreEqual(0, metadata.Bytes);
            Assert.IsNull(metadata.Hash);
            Assert.AreEqual("page_white_text", metadata.Icon);
            Assert.AreEqual(true, metadata.IsDeleted);
            Assert.AreEqual(false, metadata.IsDirectory);
            Assert.AreEqual("text/plain", metadata.MimeType);
            Assert.IsNotNull(metadata.ModifiedDate);
            Assert.AreEqual("10/08/2011 18:21:30", metadata.ModifiedDate.Value.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss"));
            Assert.AreEqual("/test .txt", metadata.Path);
            Assert.AreEqual("1f33043551f", metadata.Revision);
            Assert.AreEqual("dropbox", metadata.Root);
            Assert.AreEqual("0 bytes", metadata.Size);
            Assert.IsFalse(metadata.ThumbExists);
            Assert.IsNull(metadata.Contents);
        }

        [Test]
        public void Move() // Move file
        {
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api.dropbox.com/1/fileops/move")
                .AndExpectMethod(HttpMethod.POST)
                .AndExpectBody("root=dropbox&from_path=test1.txt&to_path=test2.txt")
                .AndRespondWith(EmbeddedResource("Move.json"), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
            Entry metadata = dropbox.MoveAsync("test1.txt", "test2.txt").Result;
#else
            Entry metadata = dropbox.Move("test1.txt", "test2.txt");
#endif
            Assert.AreEqual(15, metadata.Bytes);
            Assert.IsNull(metadata.Hash);
            Assert.AreEqual("page_white_text", metadata.Icon);
            Assert.AreEqual(false, metadata.IsDeleted);
            Assert.AreEqual(false, metadata.IsDirectory);
            Assert.AreEqual("text/plain", metadata.MimeType);
            Assert.IsNotNull(metadata.ModifiedDate);
            Assert.AreEqual("10/08/2011 18:21:29", metadata.ModifiedDate.Value.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss"));
            Assert.AreEqual("/test2.txt", metadata.Path);
            Assert.AreEqual("1e0a503351f", metadata.Revision);
            Assert.AreEqual("dropbox", metadata.Root);
            Assert.AreEqual("15 bytes", metadata.Size);
            Assert.IsFalse(metadata.ThumbExists);
            Assert.IsNull(metadata.Contents);
        }

        [Test]
        public void Copy()
        {
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api.dropbox.com/1/fileops/copy")
                .AndExpectMethod(HttpMethod.POST)
                .AndExpectBody("root=dropbox&from_path=test2.txt&to_path=test1.txt")
                .AndRespondWith(EmbeddedResource("Copy.json"), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
            Entry metadata = dropbox.CopyAsync("test2.txt", "test1.txt").Result;
#else
            Entry metadata = dropbox.Copy("test2.txt", "test1.txt");
#endif
            Assert.AreEqual(15, metadata.Bytes);
            Assert.IsNull(metadata.Hash);
            Assert.AreEqual("page_white_text", metadata.Icon);
            Assert.AreEqual(false, metadata.IsDeleted);
            Assert.AreEqual(false, metadata.IsDirectory);
            Assert.AreEqual("text/plain", metadata.MimeType);
            Assert.IsNotNull(metadata.ModifiedDate);
            Assert.AreEqual("10/08/2011 18:21:29", metadata.ModifiedDate.Value.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss"));
            Assert.AreEqual("/test1.txt", metadata.Path);
            Assert.AreEqual("1f0a503351f", metadata.Revision);
            Assert.AreEqual("dropbox", metadata.Root);
            Assert.AreEqual("15 bytes", metadata.Size);
            Assert.IsFalse(metadata.ThumbExists);
            Assert.IsNull(metadata.Contents);
        }

        [Test]
        public void UploadFile()
        {
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api-content.dropbox.com/1/files_put/dropbox/Dir/File.txt?overwrite=false&parent_rev=a123z")
                .AndExpectMethod(HttpMethod.PUT)
                .AndRespondWith(EmbeddedResource("Upload_File.json"), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
            Entry metadata = dropbox.UploadFileAsync(EmbeddedResource("File.txt"), "Dir/File.txt", false, "a123z", System.Threading.CancellationToken.None).Result;
#else
            Entry metadata = dropbox.UploadFile(EmbeddedResource("File.txt"), "Dir/File.txt", false, "a123z");
#endif
            Assert.AreEqual(230783, metadata.Bytes);
            Assert.IsNull(metadata.Hash);
            Assert.AreEqual("page_white_acrobat", metadata.Icon);
            Assert.AreEqual(false, metadata.IsDeleted);
            Assert.AreEqual(false, metadata.IsDirectory);
            Assert.AreEqual("application/pdf", metadata.MimeType);
            Assert.IsNotNull(metadata.ModifiedDate);
            Assert.AreEqual("19/07/2011 21:55:38", metadata.ModifiedDate.Value.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss"));
            Assert.AreEqual("/Getting_Started.pdf", metadata.Path);
            Assert.AreEqual("35e97029684fe", metadata.Revision);
            Assert.AreEqual("dropbox", metadata.Root);
            Assert.AreEqual("225.4KB", metadata.Size);
            Assert.IsFalse(metadata.ThumbExists);
            Assert.IsNull(metadata.Contents);
        }

        [Test]
        public void DownloadFile()
        {
            responseHeaders.ContentType = MediaType.TEXT_PLAIN;
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api-content.dropbox.com/1/files/dropbox/Dir/File.txt?rev=a123z")
                .AndExpectMethod(HttpMethod.GET)
                .AndRespondWith(EmbeddedResource("File.txt"), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
            byte[] file = dropbox.DownloadFileAsync("Dir/File.txt", "a123z", System.Threading.CancellationToken.None).Result;
#else
            byte[] file = dropbox.DownloadFile("Dir/File.txt", "a123z");
#endif
            Assert.IsNotNull(file);
            Assert.IsNotEmpty(file);
        }

        [Test]
        public void GetFileMetadata()
        {
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api.dropbox.com/1/metadata/dropbox/Dir/File.txt?list=false")
                .AndExpectMethod(HttpMethod.GET)
                .AndRespondWith(EmbeddedResource("Metadata_File.json"), responseHeaders);

            MetadataParameters parameters = new MetadataParameters();
            parameters.IncludeContents = false;
#if NET_4_0 || SILVERLIGHT_5
            Entry metadata = dropbox.GetMetadataAsync("Dir/File.txt", parameters).Result;
#else
            Entry metadata = dropbox.GetMetadata("Dir/File.txt", parameters);
#endif
            Assert.AreEqual(230783, metadata.Bytes);
            Assert.IsNull(metadata.Hash);
            Assert.AreEqual("page_white_acrobat", metadata.Icon);
            Assert.AreEqual(false, metadata.IsDeleted);
            Assert.AreEqual(false, metadata.IsDirectory);
            Assert.AreEqual("application/pdf", metadata.MimeType);
            Assert.IsNotNull(metadata.ModifiedDate);
            Assert.AreEqual("19/07/2011 21:55:38", metadata.ModifiedDate.Value.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss"));
            Assert.AreEqual("/Getting_Started.pdf", metadata.Path);
            Assert.AreEqual("35e97029684fe", metadata.Revision);
            Assert.AreEqual("dropbox", metadata.Root);
            Assert.AreEqual("225.4KB", metadata.Size);
            Assert.IsFalse(metadata.ThumbExists);
            Assert.IsNull(metadata.Contents);
        }

        [Test]
        public void GetFolderMetadata()
        {
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api.dropbox.com/1/metadata/dropbox/Dir/?file_limit=100&hash=a123z&include_deleted=true&rev=abcrev123")
                .AndExpectMethod(HttpMethod.GET)
                .AndRespondWith(EmbeddedResource("Metadata_Folder.json"), responseHeaders);

            MetadataParameters parameters = new MetadataParameters();
            parameters.FileLimits = 100;
            parameters.Hash = "a123z";
            parameters.IncludeDeleted = true;
            parameters.Revision = "abcrev123";
#if NET_4_0 || SILVERLIGHT_5
            Entry metadata = dropbox.GetMetadataAsync("Dir/", parameters).Result;
#else
            Entry metadata = dropbox.GetMetadata("Dir/", parameters);
#endif
            Assert.AreEqual(0, metadata.Bytes);
            Assert.AreEqual("37eb1ba1849d4b0fb0b28caf7ef3af52", metadata.Hash);
            Assert.AreEqual("folder_public", metadata.Icon);
            Assert.AreEqual(false, metadata.IsDeleted);
            Assert.AreEqual(true, metadata.IsDirectory);
            Assert.IsNull(metadata.MimeType);
            Assert.IsNotNull(metadata.ModifiedDate);
            Assert.AreEqual("27/04/2011 22:18:51", metadata.ModifiedDate.Value.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss"));
            Assert.AreEqual("/Public", metadata.Path);
            Assert.AreEqual("714f029684fe", metadata.Revision);
            Assert.AreEqual("dropbox", metadata.Root);
            Assert.AreEqual("0 bytes", metadata.Size);
            Assert.IsFalse(metadata.ThumbExists);
            Assert.IsNotNull(metadata.Contents);
            Assert.AreEqual(1, metadata.Contents.Count);
            Assert.AreEqual(0, metadata.Contents[0].Bytes);
            Assert.IsNull(metadata.Contents[0].Hash);
            Assert.AreEqual("page_white_text", metadata.Contents[0].Icon);
            Assert.AreEqual(false, metadata.Contents[0].IsDeleted);
            Assert.AreEqual(false, metadata.Contents[0].IsDirectory);
            Assert.AreEqual("text/plain", metadata.Contents[0].MimeType);
            Assert.IsNotNull(metadata.Contents[0].ModifiedDate);
            Assert.AreEqual("18/07/2011 20:13:43", metadata.Contents[0].ModifiedDate.Value.ToUniversalTime().ToString("dd/MM/yyyy HH:mm:ss"));
            Assert.AreEqual("/Public/latest.txt", metadata.Contents[0].Path);
            Assert.AreEqual("35c1f029684fe", metadata.Contents[0].Revision);
            Assert.AreEqual("dropbox", metadata.Contents[0].Root);
            Assert.AreEqual("0 bytes", metadata.Contents[0].Size);
            Assert.IsFalse(metadata.Contents[0].ThumbExists);
            Assert.IsNull(metadata.Contents[0].Contents);
        }

        [Test]
        public void GetFolderMetadata_304()
        {
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api.dropbox.com/1/metadata/dropbox/Dir/?hash=a123z")
                .AndExpectMethod(HttpMethod.GET)
                .AndRespondWith("", responseHeaders, HttpStatusCode.NotModified, "");

            MetadataParameters parameters = new MetadataParameters();
            parameters.Hash = "a123z";
#if NET_4_0 || SILVERLIGHT_5
            Entry metadata = dropbox.GetMetadataAsync("Dir/", parameters).Result;
#else
            Entry metadata = dropbox.GetMetadata("Dir/", parameters);
#endif
            Assert.IsNull(metadata);
        }

        // tests helpers

        private IResource EmbeddedResource(string filename)
        {
            return new AssemblyResource("assembly://Spring.Social.Dropbox.Tests/Spring.Social.Dropbox.Api.Impl/" + filename);
        }
    }
}
