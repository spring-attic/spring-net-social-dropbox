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
using System.Threading;
using System.Collections.Generic;

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
            Assert.AreEqual("10/08/2011 18:21:30", metadata.ModifiedDate.Value.ToUniversalTime().ToString("dd'/'MM'/'yyyy HH:mm:ss"));
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
            Assert.AreEqual("10/08/2011 18:21:30", metadata.ModifiedDate.Value.ToUniversalTime().ToString("dd'/'MM'/'yyyy HH:mm:ss"));
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
            Assert.AreEqual("10/08/2011 18:21:29", metadata.ModifiedDate.Value.ToUniversalTime().ToString("dd'/'MM'/'yyyy HH:mm:ss"));
            Assert.AreEqual("/test2.txt", metadata.Path);
            Assert.AreEqual("1e0a503351f", metadata.Revision);
            Assert.AreEqual("dropbox", metadata.Root);
            Assert.AreEqual("15 bytes", metadata.Size);
            Assert.IsFalse(metadata.ThumbExists);
            Assert.IsNull(metadata.Contents);
        }

        [Test]
        public void CreateFileRef()
        {
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api.dropbox.com/1/copy_ref/dropbox/test1.txt")
                .AndExpectMethod(HttpMethod.GET)
                .AndRespondWith(EmbeddedResource("Copy_Ref.json"), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
            FileRef fileRef = dropbox.CreateFileRefAsync("test1.txt").Result;
#else
            FileRef fileRef = dropbox.CreateFileRef("test1.txt");
#endif
            Assert.AreEqual("z1X6ATl6aWtzOGq0c3g5Ng", fileRef.Value);
            Assert.AreEqual("31/01/2042 21:01:05", fileRef.ExpireDate.ToUniversalTime().ToString("dd'/'MM'/'yyyy HH:mm:ss"));
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
            Assert.AreEqual("10/08/2011 18:21:29", metadata.ModifiedDate.Value.ToUniversalTime().ToString("dd'/'MM'/'yyyy HH:mm:ss"));
            Assert.AreEqual("/test1.txt", metadata.Path);
            Assert.AreEqual("1f0a503351f", metadata.Revision);
            Assert.AreEqual("dropbox", metadata.Root);
            Assert.AreEqual("15 bytes", metadata.Size);
            Assert.IsFalse(metadata.ThumbExists);
            Assert.IsNull(metadata.Contents);
        }

        [Test]
        public void CopyFileRef()
        {
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api.dropbox.com/1/fileops/copy")
                .AndExpectMethod(HttpMethod.POST)
                .AndExpectBody("root=dropbox&from_copy_ref=z1X6ATl6aWtzOGq0c3g5Ng&to_path=test1.txt")
                .AndRespondWith(EmbeddedResource("Copy.json"), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
            Entry metadata = dropbox.CopyFileRefAsync("z1X6ATl6aWtzOGq0c3g5Ng", "test1.txt").Result;
#else
            Entry metadata = dropbox.CopyFileRef("z1X6ATl6aWtzOGq0c3g5Ng", "test1.txt");
#endif
            Assert.AreEqual(15, metadata.Bytes);
            Assert.IsNull(metadata.Hash);
            Assert.AreEqual("page_white_text", metadata.Icon);
            Assert.AreEqual(false, metadata.IsDeleted);
            Assert.AreEqual(false, metadata.IsDirectory);
            Assert.AreEqual("text/plain", metadata.MimeType);
            Assert.IsNotNull(metadata.ModifiedDate);
            Assert.AreEqual("10/08/2011 18:21:29", metadata.ModifiedDate.Value.ToUniversalTime().ToString("dd'/'MM'/'yyyy HH:mm:ss"));
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
            Entry metadata = dropbox.UploadFileAsync(EmbeddedResource("File.txt"), "Dir/File.txt", false, "a123z", CancellationToken.None).Result;
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
            Assert.AreEqual("19/07/2011 21:55:38", metadata.ModifiedDate.Value.ToUniversalTime().ToString("dd'/'MM'/'yyyy HH:mm:ss"));
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
            responseHeaders["x-dropbox-metadata"] = "{ \"size\": \"225.4KB\", \"rev\": \"35e97029684fe\", \"thumb_exists\": false, \"bytes\": 230783, \"modified\": \"Tue, 19 Jul 2011 21:55:38 +0000\", \"path\": \"/Getting_Started.pdf\", \"is_dir\": false, \"icon\": \"page_white_acrobat\", \"root\": \"dropbox\", \"mime_type\": \"application/pdf\", \"revision\": 220823 }";
            responseHeaders.ContentType = MediaType.TEXT_PLAIN;
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api-content.dropbox.com/1/files/dropbox/Dir/File.txt?rev=a123z")
                .AndExpectMethod(HttpMethod.GET)
                .AndRespondWith(EmbeddedResource("File.txt"), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
            DropboxFile file = dropbox.DownloadFileAsync("Dir/File.txt", "a123z", CancellationToken.None).Result;
#else
            DropboxFile file = dropbox.DownloadFile("Dir/File.txt", "a123z");
#endif
            Assert.IsNotNull(file);

            // Content
            Assert.IsNotNull(file.Content);
            Assert.IsNotEmpty(file.Content);

            // Metadata
            Assert.AreEqual(230783, file.Metadata.Bytes);
            Assert.IsNull(file.Metadata.Hash);
            Assert.AreEqual("page_white_acrobat", file.Metadata.Icon);
            Assert.AreEqual(false, file.Metadata.IsDeleted);
            Assert.AreEqual(false, file.Metadata.IsDirectory);
            Assert.AreEqual("application/pdf", file.Metadata.MimeType);
            Assert.IsNotNull(file.Metadata.ModifiedDate);
            Assert.AreEqual("19/07/2011 21:55:38", file.Metadata.ModifiedDate.Value.ToUniversalTime().ToString("dd'/'MM'/'yyyy HH:mm:ss"));
            Assert.AreEqual("/Getting_Started.pdf", file.Metadata.Path);
            Assert.AreEqual("35e97029684fe", file.Metadata.Revision);
            Assert.AreEqual("dropbox", file.Metadata.Root);
            Assert.AreEqual("225.4KB", file.Metadata.Size);
            Assert.IsFalse(file.Metadata.ThumbExists);
            Assert.IsNull(file.Metadata.Contents);
        }

        [Test]
        public void DownloadPartialFile()
        {
            responseHeaders["x-dropbox-metadata"] = "{ \"size\": \"225.4KB\", \"rev\": \"35e97029684fe\", \"thumb_exists\": false, \"bytes\": 230783, \"modified\": \"Tue, 19 Jul 2011 21:55:38 +0000\", \"path\": \"/Getting_Started.pdf\", \"is_dir\": false, \"icon\": \"page_white_acrobat\", \"root\": \"dropbox\", \"mime_type\": \"application/pdf\", \"revision\": 220823 }";
            responseHeaders.ContentType = MediaType.TEXT_PLAIN;
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api-content.dropbox.com/1/files/dropbox/Dir/File.txt?rev=a123z")
                .AndExpectMethod(HttpMethod.GET)
                .AndExpectHeader("Range", "bytes=0-1023")
                .AndRespondWith(EmbeddedResource("File.txt"), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
            DropboxFile file = dropbox.DownloadPartialFileAsync("Dir/File.txt", 0, 1024, "a123z", CancellationToken.None).Result;
#else
            DropboxFile file = dropbox.DownloadPartialFile("Dir/File.txt", 0, 1024, "a123z");
#endif
            Assert.IsNotNull(file);

            // Content
            Assert.IsNotNull(file.Content);
            Assert.IsNotEmpty(file.Content);

            // Metadata
            Assert.AreEqual(230783, file.Metadata.Bytes);
            Assert.IsNull(file.Metadata.Hash);
            Assert.AreEqual("page_white_acrobat", file.Metadata.Icon);
            Assert.AreEqual(false, file.Metadata.IsDeleted);
            Assert.AreEqual(false, file.Metadata.IsDirectory);
            Assert.AreEqual("application/pdf", file.Metadata.MimeType);
            Assert.IsNotNull(file.Metadata.ModifiedDate);
            Assert.AreEqual("19/07/2011 21:55:38", file.Metadata.ModifiedDate.Value.ToUniversalTime().ToString("dd'/'MM'/'yyyy HH:mm:ss"));
            Assert.AreEqual("/Getting_Started.pdf", file.Metadata.Path);
            Assert.AreEqual("35e97029684fe", file.Metadata.Revision);
            Assert.AreEqual("dropbox", file.Metadata.Root);
            Assert.AreEqual("225.4KB", file.Metadata.Size);
            Assert.IsFalse(file.Metadata.ThumbExists);
            Assert.IsNull(file.Metadata.Contents);
        }

        [Test]
        public void Delta()
        {
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api.dropbox.com/1/delta/")
                .AndExpectMethod(HttpMethod.POST)
                .AndExpectBody("cursor=123azed54")
                .AndRespondWith(EmbeddedResource("Delta.json"), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
            DeltaPage deltaPage = dropbox.DeltaAsync("123azed54").Result;
#else
            DeltaPage deltaPage = dropbox.Delta("123azed54");
#endif
            Assert.AreEqual("AuYe6kpu-M6pToHfszmwEtnEuE8Xiz4NqiDs4BKp2w2OeJxj_JqrzMCww5VZ5l8SAwOjGwMDAzMDBAjpF-cmFpWoFeenlSjoKiSlpKQxi_n8YkAAff3EgoJi_eKCosy8dL281BKF4vzkzMQchcLSzOTs4hKgbgUNoBKFtPyclNQiTdb3W-yQtAvoJyVmJiok5-cVl-aUAI1gyWl_D5IAANo_J44", deltaPage.Cursor);
            Assert.AreEqual(true, deltaPage.HasMore);
            Assert.AreEqual(true, deltaPage.Reset);
            Assert.AreEqual(11, deltaPage.Entries.Count);
            Assert.AreEqual("/photos", deltaPage.Entries[0].Path);
            Assert.IsNotNull(deltaPage.Entries[0].Metadata);
            Assert.AreEqual("/Photos", deltaPage.Entries[0].Metadata.Path);
            Assert.AreEqual("/deleted_dir", deltaPage.Entries[10].Path);
            Assert.IsNull(deltaPage.Entries[10].Metadata);
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
            Assert.AreEqual("19/07/2011 21:55:38", metadata.ModifiedDate.Value.ToUniversalTime().ToString("dd'/'MM'/'yyyy HH:mm:ss"));
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
            parameters.FileLimit = 100;
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
            Assert.AreEqual("27/04/2011 22:18:51", metadata.ModifiedDate.Value.ToUniversalTime().ToString("dd'/'MM'/'yyyy HH:mm:ss"));
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
            Assert.AreEqual("18/07/2011 20:13:43", metadata.Contents[0].ModifiedDate.Value.ToUniversalTime().ToString("dd'/'MM'/'yyyy HH:mm:ss"));
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

        [Test]
        public void GetRevisions()
        {
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api.dropbox.com/1/revisions/dropbox/Dir/File.txt?rev_limit=20")
                .AndExpectMethod(HttpMethod.GET)
                .AndRespondWith(EmbeddedResource("Revisions.json"), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
            IList<Entry> revisions = dropbox.GetRevisionsAsync("Dir/File.txt", 20).Result;
#else
            IList<Entry> revisions = dropbox.GetRevisions("Dir/File.txt", 20);
#endif
            Assert.IsNotNull(revisions);
            Assert.AreEqual(2, revisions.Count);
            Assert.AreEqual(0, revisions[0].Bytes);
            Assert.IsNull(revisions[0].Hash);
            Assert.AreEqual("page_white", revisions[0].Icon);
            Assert.AreEqual(true, revisions[0].IsDeleted);
            Assert.AreEqual(false, revisions[0].IsDirectory);
            Assert.AreEqual("application/octet-stream", revisions[0].MimeType);
            Assert.IsNotNull(revisions[0].ModifiedDate);
            Assert.AreEqual("20/07/2011 22:41:09", revisions[0].ModifiedDate.Value.ToUniversalTime().ToString("dd'/'MM'/'yyyy HH:mm:ss"));
            Assert.AreEqual("/hi2", revisions[0].Path);
            Assert.AreEqual("40000000d", revisions[0].Revision);
            Assert.AreEqual("app_folder", revisions[0].Root);
            Assert.AreEqual("0 bytes", revisions[0].Size);
            Assert.IsFalse(revisions[0].ThumbExists);
            Assert.IsNull(revisions[0].Contents);
        }

        [Test]
        public void Restore()
        {
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api.dropbox.com/1/restore/dropbox/Dir/File.txt")
                .AndExpectMethod(HttpMethod.POST)
                .AndExpectBody("rev=a123z")
                .AndRespondWith(EmbeddedResource("Restore.json"), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
            Entry metadata = dropbox.RestoreAsync("Dir/File.txt", "a123z").Result;
#else
            Entry metadata = dropbox.Restore("Dir/File.txt", "a123z");
#endif
            Assert.AreEqual(0, metadata.Bytes);
            Assert.IsNull(metadata.Hash);
            Assert.AreEqual("page_white", metadata.Icon);
            Assert.AreEqual(true, metadata.IsDeleted);
            Assert.AreEqual(false, metadata.IsDirectory);
            Assert.AreEqual("application/octet-stream", metadata.MimeType);
            Assert.IsNotNull(metadata.ModifiedDate);
            Assert.AreEqual("20/07/2011 22:41:09", metadata.ModifiedDate.Value.ToUniversalTime().ToString("dd'/'MM'/'yyyy HH:mm:ss"));
            Assert.AreEqual("/hi2", metadata.Path);
            Assert.AreEqual("40000000d", metadata.Revision);
            Assert.AreEqual("sandbox", metadata.Root);
            Assert.AreEqual("0 bytes", metadata.Size);
            Assert.IsFalse(metadata.ThumbExists);
            Assert.IsNull(metadata.Contents);
        }

        [Test]
        public void Search()
        {
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api.dropbox.com/1/search/dropbox/Dir/")
                .AndExpectMethod(HttpMethod.POST)
                .AndExpectBody("query=.txt&file_limit=20&include_deleted=true")
                .AndRespondWith(EmbeddedResource("Search.json"), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
            IList<Entry> results = dropbox.SearchAsync("Dir/", ".txt", 20, true).Result;
#else
            IList<Entry> results = dropbox.Search("Dir/", ".txt", 20, true);
#endif
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(0, results[0].Bytes);
            Assert.IsNull(results[0].Hash);
            Assert.AreEqual("page_white_text", results[0].Icon);
            Assert.AreEqual(false, results[0].IsDeleted);
            Assert.AreEqual(false, results[0].IsDirectory);
            Assert.AreEqual("text/plain", results[0].MimeType);
            Assert.IsNotNull(results[0].ModifiedDate);
            Assert.AreEqual("18/07/2011 20:13:43", results[0].ModifiedDate.Value.ToUniversalTime().ToString("dd'/'MM'/'yyyy HH:mm:ss"));
            Assert.AreEqual("/Public/latest.txt", results[0].Path);
            Assert.AreEqual("35c1f029684fe", results[0].Revision);
            Assert.AreEqual("dropbox", results[0].Root);
            Assert.AreEqual("0 bytes", results[0].Size);
            Assert.IsFalse(results[0].ThumbExists);
            Assert.IsNull(results[0].Contents);
        }

        [Test]
        public void GetShareableLink()
        {
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api.dropbox.com/1/shares/dropbox/Dir/File.txt")
                .AndExpectMethod(HttpMethod.POST)
                .AndExpectBody("")
                .AndRespondWith(EmbeddedResource("GetShareableLink.json"), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
            DropboxLink link = dropbox.GetShareableLinkAsync("Dir/File.txt").Result;
#else
            DropboxLink link = dropbox.GetShareableLink("Dir/File.txt");
#endif
            Assert.IsNotNull(link);
            Assert.AreEqual("http://db.tt/APqhX1", link.Url);
            Assert.AreEqual("17/08/2011 02:34:33", link.ExpireDate.ToUniversalTime().ToString("dd'/'MM'/'yyyy HH:mm:ss"));
        }

        [Test]
        public void GetMediaLink()
        {
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api.dropbox.com/1/media/dropbox/Dir/File.txt")
                .AndExpectMethod(HttpMethod.POST)
                .AndExpectBody("")
                .AndRespondWith(EmbeddedResource("GetMediaLink.json"), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
            DropboxLink link = dropbox.GetMediaLinkAsync("Dir/File.txt").Result;
#else
            DropboxLink link = dropbox.GetMediaLink("Dir/File.txt");
#endif
            Assert.IsNotNull(link);
            Assert.AreEqual("http://www.dropbox.com/s/m/a2mbDa2", link.Url);
            Assert.AreEqual("16/09/2011 01:01:25", link.ExpireDate.ToUniversalTime().ToString("dd'/'MM'/'yyyy HH:mm:ss"));
        }

        [Test]
        public void DownloadThumbnail()
        {
            responseHeaders["x-dropbox-metadata"] = "{ \"size\": \"225.4KB\", \"rev\": \"35e97029684fe\", \"thumb_exists\": false, \"bytes\": 230783, \"modified\": \"Tue, 19 Jul 2011 21:55:38 +0000\", \"path\": \"/Getting_Started.pdf\", \"is_dir\": false, \"icon\": \"page_white_acrobat\", \"root\": \"dropbox\", \"mime_type\": \"application/pdf\", \"revision\": 220823 }";
            responseHeaders.ContentType = MediaType.IMAGE_PNG;
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api-content.dropbox.com/1/thumbnails/dropbox/Dir/Image.jpg?format=PNG&size=xl")
                .AndExpectMethod(HttpMethod.GET)
                .AndRespondWith(EmbeddedResource("Image.png"), responseHeaders);

#if NET_4_0 || SILVERLIGHT_5
            DropboxFile thumbnail = dropbox.DownloadThumbnailAsync("Dir/Image.jpg", ThumbnailFormat.Png, ThumbnailSize.ExtraLarge).Result;
#else
            DropboxFile thumbnail = dropbox.DownloadThumbnail("Dir/Image.jpg", ThumbnailFormat.Png, ThumbnailSize.ExtraLarge);
#endif
            Assert.IsNotNull(thumbnail);

            // Content
            Assert.IsNotNull(thumbnail);
            Assert.IsNotEmpty(thumbnail.Content);

            // Metadata
            Assert.AreEqual(230783, thumbnail.Metadata.Bytes);
            Assert.IsNull(thumbnail.Metadata.Hash);
            Assert.AreEqual("page_white_acrobat", thumbnail.Metadata.Icon);
            Assert.AreEqual(false, thumbnail.Metadata.IsDeleted);
            Assert.AreEqual(false, thumbnail.Metadata.IsDirectory);
            Assert.AreEqual("application/pdf", thumbnail.Metadata.MimeType);
            Assert.IsNotNull(thumbnail.Metadata.ModifiedDate);
            Assert.AreEqual("19/07/2011 21:55:38", thumbnail.Metadata.ModifiedDate.Value.ToUniversalTime().ToString("dd'/'MM'/'yyyy HH:mm:ss"));
            Assert.AreEqual("/Getting_Started.pdf", thumbnail.Metadata.Path);
            Assert.AreEqual("35e97029684fe", thumbnail.Metadata.Revision);
            Assert.AreEqual("dropbox", thumbnail.Metadata.Root);
            Assert.AreEqual("225.4KB", thumbnail.Metadata.Size);
            Assert.IsFalse(thumbnail.Metadata.ThumbExists);
            Assert.IsNull(thumbnail.Metadata.Contents);
        }

        [Test]
        public void DropboxPathSpecialCharacters()
        {
            mockServer.ExpectNewRequest()
                .AndExpectUri("https://api.dropbox.com/1/metadata/dropbox/%24%26%2B%2C%3B%3D%40%23%7B%7D%5E%7E%5B%5D%60%27%25%28%29%21-_/Spring%20Social.txt?list=false")
                .AndExpectMethod(HttpMethod.GET);

            MetadataParameters parameters = new MetadataParameters();
            parameters.IncludeContents = false;
#if NET_4_0 || SILVERLIGHT_5
            dropbox.GetMetadataAsync("$&+,;=@#{}^~[]`'%()!-_/Spring Social.txt", parameters).Wait();
#else
            dropbox.GetMetadata("$&+,;=@#{}^~[]`'%()!-_/Spring Social.txt", parameters);
#endif
        }


        // tests helpers

        private IResource EmbeddedResource(string filename)
        {
            return new AssemblyResource("assembly://Spring.Social.Dropbox.Tests/Spring.Social.Dropbox.Api.Impl/" + filename);
        }
    }
}
