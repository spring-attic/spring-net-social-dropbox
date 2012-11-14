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
using System.Text;
using System.Collections.Generic;
#if SILVERLIGHT
using Spring.Collections.Specialized;
#else
using System.Collections.Specialized;
#endif
#if NET_4_0 || SILVERLIGHT_5
using System.Threading;
using System.Threading.Tasks;
#endif

using Spring.IO;
using Spring.Json;
using Spring.Rest.Client;
using Spring.Rest.Client.Support;
using Spring.Social.OAuth1;
using Spring.Http;
using Spring.Http.Client;
using Spring.Http.Converters;
using Spring.Http.Converters.Json;

using Spring.Social.Dropbox.Api.Impl.Json;

namespace Spring.Social.Dropbox.Api.Impl
{
    /// <summary>
    /// This is the central class for interacting with Dropbox.
    /// </summary>
    /// <remarks>
    /// All Dropbox operations require OAuth authentication.
    /// </remarks>
    /// <author>Bruno Baia</author>
    public class DropboxTemplate : AbstractOAuth1ApiBinding, IDropbox 
    {
        private static readonly Uri API_URI_BASE = new Uri("https://api.dropbox.com/1/");

        private AccessLevel accessLevel;
        private string locale;
        private string root;

        /// <summary>
        /// Creates a new instance of <see cref="DropboxTemplate"/>.
        /// </summary>
        /// <param name="consumerKey">The application's API key.</param>
        /// <param name="consumerSecret">The application's API secret.</param>
        /// <param name="accessToken">An access token acquired through OAuth authentication with Dropbox.</param>
        /// <param name="accessTokenSecret">An access token secret acquired through OAuth authentication with Dropbox.</param>
        /// <param name="accessLevel">The application access level.</param>
        public DropboxTemplate(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret, AccessLevel accessLevel) 
            : base(consumerKey, consumerSecret, accessToken, accessTokenSecret)
        {
            this.accessLevel = accessLevel;
            this.root = this.accessLevel == Api.AccessLevel.AppFolder ? "sandbox" : "dropbox";
	    }

        #region IDropbox Members

        /// <summary>
        /// Gets the application access level. 
        /// </summary>
        public AccessLevel AccessLevel
        {
            get { return this.accessLevel; }
        }

        /// <summary>
        /// Gets or sets the locale parameter to specify language settings of content responses. 
        /// <para/>
        /// Uses IETF language tag (ex: "pt-BR" for Brazilian Portuguese).
        /// <para/>
        /// Default is <see langword="null"/> for Dropbox server's locale.
        /// </summary>
        public string Locale
        {
            get { return this.locale; }
            set { this.locale = value; }
        }

#if NET_4_0 || SILVERLIGHT_5
        /// <summary>
        /// Asynchronously retrieves the authenticated user's Dropbox profile details.
        /// </summary>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a <see cref="DropboxProfile"/> object representing the user's profile.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<DropboxProfile> GetUserProfileAsync()
        {
            NameValueCollection parameters = new NameValueCollection();
            this.AddLocaleTo(parameters);
            return this.RestTemplate.GetForObjectAsync<DropboxProfile>(BuildUrl("account/info", parameters));
        }

        /// <summary>
        /// Asynchronously creates a folder.
        /// </summary>
        /// <param name="path">The Dropbox path to the new folder to create relative to root.</param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a metadata <see cref="Entry"/> for the new folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<Entry> CreateFolderAsync(string path)
        {
            NameValueCollection request = new NameValueCollection();
            request.Add("root", this.root);
            request.Add("path", path);
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObjectAsync<Entry>("fileops/create_folder", request);
        }

        /// <summary>
        /// Asynchronously deletes a file or folder.
        /// </summary>
        /// <param name="path">The Dropbox path to the file or folder to be deleted relative to root.</param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a metadata <see cref="Entry"/> for the deleted file or folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<Entry> DeleteAsync(string path)
        {
            NameValueCollection request = new NameValueCollection();
            request.Add("root", this.root);
            request.Add("path", path);
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObjectAsync<Entry>("fileops/delete", request);
        }

        /// <summary>
        /// Asynchronously moves a file or folder to a new location.
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to be copied from, relative to root.</param>
        /// <param name="toPath">The destination path, including the new name for the file or folder, relative to root.</param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a metadata <see cref="Entry"/> for the moved file or folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<Entry> MoveAsync(string fromPath, string toPath)
        {
            NameValueCollection request = new NameValueCollection();
            request.Add("root", this.root);
            request.Add("from_path", fromPath);
            request.Add("to_path", toPath);
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObjectAsync<Entry>("fileops/move", request);
        }

        /// <summary>
        /// Asynchronously creates a reference to another user's Dropbox file that can be used to 
        /// copy files directly between two Dropbox accounts (with permission from both users) 
        /// using the <see cref="M:CopyFileRefAsync"/> method.
        /// No need to download and re-upload files.
        /// </summary>
        /// <param name="path">The path to the file you want a reference.</param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a <see cref="FileRef">reference to another user's Dropbox file</see>.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        /// <seealso cref="M:CopyFileRefAsync"/>
        public Task<FileRef> CreateFileRefAsync(string path)
        {
            return this.RestTemplate.GetForObjectAsync<FileRef>(this.BuildUrl("copy_ref/", path));
        }

        /// <summary>
        /// Asynchronously copies a file or folder to a new location.
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to be copied from, relative to root.</param>
        /// <param name="toPath">The destination path, including the new name for the file or folder, relative to root.</param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a metadata <see cref="Entry"/> for the moved file or folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<Entry> CopyAsync(string fromPath, string toPath)
        {
            NameValueCollection request = new NameValueCollection();
            request.Add("root", this.root);
            request.Add("from_path", fromPath);
            request.Add("to_path", toPath);
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObjectAsync<Entry>("fileops/copy", request);
        }

        /// <summary>
        /// Asynchronously copies another user's Dropbox file without the need to download and re-upload the file.
        /// </summary>
        /// <param name="fromFileRef">
        /// The reference to another user's Dropbox file to be copied from, 
        /// obtained by calling the <see cref="M:CreateFileRefAsync"/> method.
        /// </param>
        /// <param name="toPath">The destination file path, including the new name for the file, relative to root.</param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a metadata <see cref="Entry"/> for the copied file.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        /// <seealso cref="M:CreateFileRefAsync"/>
        public Task<Entry> CopyFileRefAsync(string fromFileRef, string toPath)
        {
            NameValueCollection request = new NameValueCollection();
            request.Add("root", this.root);
            request.Add("from_copy_ref", fromFileRef);
            request.Add("to_path", toPath);
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObjectAsync<Entry>("fileops/copy", request);
        }

        /// <summary>
        /// Asynchronously uploads a file.
        /// </summary>
        /// <param name="file">The file resource to be uploaded.</param>
        /// <param name="path">
        /// The path to the file you want to write to, relative to root. 
        /// This parameter should not point to a folder.
        /// </param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a metadata <see cref="Entry"/> for the uploaded file.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<Entry> UploadFileAsync(IResource file, string path)
        {
            return this.UploadFileAsync(file, path, true, null, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously uploads a file.
        /// </summary>
        /// <param name="file">The file resource to be uploaded.</param>
        /// <param name="path">
        /// The path to the file you want to write to, relative to root. 
        /// This parameter should not point to a folder.
        /// </param>
        /// <param name="overwrite">
        /// If <see langword="true"/>, the existing file will be overwritten by the new one. 
        /// If <see langword="false"/>, the new file will be automatically renamed. 
        /// The new name can be obtained from the returned metadata.
        /// </param>
        /// <param name="revision">
        /// The revision of the file you're editing or null if this is a new upload. 
        /// If <paramref name="revision"/> matches the latest version of the file on the user's Dropbox, that file will be replaced.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> that will be assigned to the task. 
        /// <para/>
        /// Use <see cref="P:CancellationToken.None"/> for an empty <see cref="CancellationToken"/> value.
        /// </param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a metadata <see cref="Entry"/> for the uploaded file.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<Entry> UploadFileAsync(IResource file, string path, bool overwrite, string revision, CancellationToken cancellationToken)
        {
            HttpEntityRequestCallback requestCallback = new HttpEntityRequestCallback(file, typeof(Entry), this.RestTemplate.MessageConverters);
            MessageConverterResponseExtractor<Entry> responseExtractor = new MessageConverterResponseExtractor<Entry>(this.RestTemplate.MessageConverters);
            return this.RestTemplate.ExecuteAsync<Entry>(this.BuildUploadUrl(path, overwrite, revision), HttpMethod.PUT, requestCallback, responseExtractor, cancellationToken);
        }

        /// <summary>
        /// Asynchronously downloads a file and its metadata.
        /// </summary>
        /// <param name="path">The Dropbox path to the file you want to retrieve, relative to root.</param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a <see cref="DropboxFile"/> object containing the file's content and metadata.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<DropboxFile> DownloadFileAsync(string path)
        {
            return this.DownloadFileAsync(path, null, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously downloads a file and its metadata.
        /// </summary>
        /// <param name="path">The Dropbox path to the file you want to retrieve, relative to root.</param>
        /// <param name="revision">
        /// The revision of the file to retrieve, or <see langword="null"/> for the latest version.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> that will be assigned to the task. 
        /// <para/>
        /// Use <see cref="P:CancellationToken.None"/> for an empty <see cref="CancellationToken"/> value.
        /// </param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a <see cref="DropboxFile"/> object containing the file's content and metadata.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<DropboxFile> DownloadFileAsync(string path, string revision, CancellationToken cancellationToken)
        {
            AcceptHeaderRequestCallback requestCallback = new AcceptHeaderRequestCallback(typeof(DropboxFile), this.RestTemplate.MessageConverters);
            DropboxFileResponseExtractor responseExtractor = new DropboxFileResponseExtractor(this.RestTemplate.MessageConverters);
            return this.RestTemplate.ExecuteAsync<DropboxFile>(this.BuildDownloadUrl(path, revision), HttpMethod.GET, requestCallback, responseExtractor, cancellationToken);
        }

        /// <summary>
        /// Asynchronously downloads part of a file and its metadata.
        /// </summary>
        /// <param name="path">The Dropbox path to the file you want to retrieve, relative to root.</param>
        /// <param name="startOffset">The zero-based starting position of the part file.</param>
        /// <param name="length">The number of bytes of the part file.</param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a <see cref="DropboxFile"/> object containing the part of file's content and metadata.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<DropboxFile> DownloadPartialFileAsync(string path, long startOffset, long length)
        {
            return this.DownloadPartialFileAsync(path, startOffset, length, null, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously downloads part of a file and its metadata.
        /// </summary>
        /// <param name="path">The Dropbox path to the file you want to retrieve, relative to root.</param>
        /// <param name="startOffset">The zero-based starting position of the part file.</param>
        /// <param name="length">The number of bytes of the part file.</param>
        /// <param name="revision">
        /// The revision of the file to retrieve, or <see langword="null"/> for the latest version.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> that will be assigned to the task. 
        /// <para/>
        /// Use <see cref="P:CancellationToken.None"/> for an empty <see cref="CancellationToken"/> value.
        /// </param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a <see cref="DropboxFile"/> object containing the part of file's content and metadata.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<DropboxFile> DownloadPartialFileAsync(string path, long startOffset, long length, string revision, CancellationToken cancellationToken)
        {
            HttpEntity requestEntity = new HttpEntity();
            requestEntity.Headers.Add("Range", String.Format("bytes={0}-{1}", startOffset, startOffset + length - 1));
            HttpEntityRequestCallback requestCallback = new HttpEntityRequestCallback(requestEntity, typeof(DropboxFile), this.RestTemplate.MessageConverters);
            DropboxFileResponseExtractor responseExtractor = new DropboxFileResponseExtractor(this.RestTemplate.MessageConverters);
            return this.RestTemplate.ExecuteAsync<DropboxFile>(this.BuildDownloadUrl(path, revision), HttpMethod.GET, requestCallback, responseExtractor, cancellationToken);
        }

        /// <summary>
        /// Asynchronously keeps up with changes to files and folders in a user's Dropbox. 
        /// <para/>
        /// You can periodically call this method to get a list of metadatas, 
        /// which are instructions on how to update your local state to match the server's state.
        /// </summary>
        /// <param name="cursor">
        /// A value that is used to keep track of your current state. 
        /// <para/>
        /// On the first call, you should pass in <see langword="null"/>. 
        /// On subsequent calls, pass in the cursor value returned by the previous call.
        /// </param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a single <see cref="DeltaPage">delta page</see> of results. 
        /// <para/>
        /// The <see cref="DeltaPage"/>'s HasMore property will tell you whether the server has more pages of results to return. 
        /// If the server doesn't have more results, wait for at least five minutes (preferably longer) and poll again.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<DeltaPage> DeltaAsync(string cursor)
        {
            NameValueCollection request = new NameValueCollection();
            if (cursor != null)
            {
                request.Add("cursor", cursor);
            }
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObjectAsync<DeltaPage>("delta/", request);
        }

        /// <summary>
        /// Asynchronously retrieves file or folder metadata.
        /// </summary>
        /// <param name="path">The Dropbox path to the file or folder, relative to root.</param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a metadata <see cref="Entry"/> for the file or folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<Entry> GetMetadataAsync(string path)
        {
            return this.GetMetadataAsync(path, new MetadataParameters());
        }

        /// <summary>
        /// Asynchronously retrieves file or folder metadata. 
        /// May return <see langword="null"/> if an hash is provided through parameters.
        /// </summary>
        /// <param name="path">The Dropbox path to the file or folder, relative to root.</param>
        /// <param name="parameters">The parameters for retrieving file and folder metadata.</param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a metadata <see cref="Entry"/> for the file or folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<Entry> GetMetadataAsync(string path, MetadataParameters parameters)
        {
            return this.RestTemplate.GetForObjectAsync<Entry>(this.BuildMetadataUrl(path, parameters));
        }

        /// <summary>
        /// Asynchronously obtains metadata for the previous revisions of a file.
        /// </summary>
        /// <param name="path">The Dropbox path to the file, relative to root.</param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a list of metadata <see cref="Entry">entries</see>.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<IList<Entry>> GetRevisionsAsync(string path)
        {
            return this.GetRevisionsAsync(path, 0);
        }

        /// <summary>
        /// Asynchronously obtains metadata for the previous revisions of a file.
        /// </summary>
        /// <param name="path">The Dropbox path to the file, relative to root.</param>
        /// <param name="revLimit">
        /// The maximum numbers of revisions to retrieve. Default is 10, Max is 1,000.
        /// </param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a list of metadata <see cref="Entry">entries</see>.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<IList<Entry>> GetRevisionsAsync(string path, int revLimit)
        {
            NameValueCollection parameters = new NameValueCollection();
            if (revLimit > 0)
            {
                parameters.Add("rev_limit", revLimit.ToString());
            }
            this.AddLocaleTo(parameters);
            return this.RestTemplate.GetForObjectAsync<IList<Entry>>(this.BuildUrl("revisions/", path, parameters));
        }

        /// <summary>
        /// Asynchronously restores a file path to a previous revision.
        /// </summary>
        /// <param name="path">The Dropbox path to the file, relative to root.</param>
        /// <param name="revision">The revision of the file to restore.</param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a metadata <see cref="Entry"/> for the restored file. 
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<Entry> RestoreAsync(string path, string revision)
        {
            NameValueCollection request = new NameValueCollection();
            request.Add("rev", revision);
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObjectAsync<Entry>(this.BuildUrl("restore/", path), request);
        }

        /// <summary>
        /// Asynchronously searches for all files and folders that match the search query.
        /// </summary>
        /// <param name="path">The Dropbox path to the folder you want to search from, relative to root.</param>
        /// <param name="query">The search string. Must be at least three characters long.</param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a list of metadata <see cref="Entry">entries</see> for any matching files and folders.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<IList<Entry>> SearchAsync(string path, string query)
        {
            return this.SearchAsync(path, query, 0, false);
        }

        /// <summary>
        /// Asynchronously searches for all files and folders that match the search query.
        /// </summary>
        /// <param name="path">The Dropbox path to the folder you want to search from, relative to root.</param>
        /// <param name="query">The search string. Must be at least three characters long.</param>
        /// <param name="fileLimit">
        /// The maximum numbers of file entries to retrieve. The maximum and default value is 1000.
        /// </param>
        /// <param name="includeDeleted">
        /// If <see langword="true"/>, then files and folders that have been deleted will also be included in the search.
        /// </param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a list of metadata <see cref="Entry">entries</see> for any matching files and folders.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<IList<Entry>> SearchAsync(string path, string query, int fileLimit, bool includeDeleted)
        {
            NameValueCollection request = this.BuildSearchParameters(path, query, fileLimit, includeDeleted);
            return this.RestTemplate.PostForObjectAsync<IList<Entry>>(this.BuildUrl("search/", path), request);
        }

        /// <summary>
        /// Asynchronously creates and returns a shareable link to files or folders.
        /// </summary>
        /// <param name="path">
        /// The Dropbox path to the file or folder you want a sharable link to, relative to root.
        /// </param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a shareable link to the file or folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<DropboxLink> GetShareableLinkAsync(string path)
        {
            NameValueCollection request = new NameValueCollection();
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObjectAsync<DropboxLink>(this.BuildUrl("shares/", path), request);
        }

        /// <summary>
        /// Asynchronously returns a link for streaming media files.
        /// </summary>
        /// <param name="path">
        /// The Dropbox path to the media file you want a direct link to, relative to root.
        /// </param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a direct link to the media file.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<DropboxLink> GetMediaLinkAsync(string path)
        {
            NameValueCollection request = new NameValueCollection();
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObjectAsync<DropboxLink>(this.BuildUrl("media/", path), request);
        }

        /// <summary>
        /// Asynchronously downloads a thumbnail for an image and its metadata.
        /// </summary>
        /// <param name="path">
        /// The Dropbox path to the image file you want to thumbnail, relative to root.
        /// </param>
        /// <param name="format">The image format of the thumbnail to download.</param>
        /// <param name="size">The size of the thumbnail to download.</param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a <see cref="DropboxFile"/> object containing the thumbnail's content and metadata.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Task<DropboxFile> DownloadThumbnailAsync(string path, ThumbnailFormat format, ThumbnailSize size)
        {
            return this.RestTemplate.GetForObjectAsync<DropboxFile>(this.BuildThumbnailsUrl(path, format, size));
        }
#else
#if !SILVERLIGHT
        /// <summary>
        /// Retrieves the authenticated user's Dropbox profile details.
        /// </summary>
        /// <returns>A <see cref="DropboxProfile"/> object representing the user's profile.</returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public DropboxProfile GetUserProfile()
        {
            NameValueCollection parameters = new NameValueCollection();
            this.AddLocaleTo(parameters);
            return this.RestTemplate.GetForObject<DropboxProfile>(BuildUrl("account/info", parameters));
        }

        /// <summary>
        /// Creates a folder.
        /// </summary>
        /// <param name="path">The Dropbox path to the new folder to create relative to root.</param>
        /// <returns>
        /// A metadata <see cref="Entry"/> for the new folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Entry CreateFolder(string path)
        {
            NameValueCollection request = new NameValueCollection();
            request.Add("root", this.root);
            request.Add("path", path);
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObject<Entry>("fileops/create_folder", request);
        }

        /// <summary>
        /// Deletes a file or folder.
        /// </summary>
        /// <param name="path">The Dropbox path to the file or folder to be deleted relative to root.</param>
        /// <returns>
        /// A metadata <see cref="Entry"/> for the deleted file or folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Entry Delete(string path)
        {
            NameValueCollection request = new NameValueCollection();
            request.Add("root", this.root);
            request.Add("path", path);
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObject<Entry>("fileops/delete", request);
        }

        /// <summary>
        /// Moves a file or folder to a new location.
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to be copied from, relative to root.</param>
        /// <param name="toPath">The destination path, including the new name for the file or folder, relative to root.</param>
        /// <returns>
        /// A metadata <see cref="Entry"/> for the moved file or folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Entry Move(string fromPath, string toPath)
        {
            NameValueCollection request = new NameValueCollection();
            request.Add("root", this.root);
            request.Add("from_path", fromPath);
            request.Add("to_path", toPath);
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObject<Entry>("fileops/move", request);
        }

        /// <summary>
        /// Creates a reference to another user's Dropbox file that can be used to 
        /// copy files directly between two Dropbox accounts (with permission from both users) 
        /// using the <see cref="M:CopyFileRef"/> method.
        /// No need to download and re-upload files.
        /// </summary>
        /// <param name="path">The path to the file you want a reference.</param>
        /// <returns>
        /// A <see cref="FileRef">reference to another user's Dropbox file</see>.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        /// <seealso cref="M:CopyFileRef"/>
        public FileRef CreateFileRef(string path)
        {
            return this.RestTemplate.GetForObject<FileRef>(this.BuildUrl("copy_ref/", path));
        }

        /// <summary>
        /// Copies a file or folder to a new location.
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to be copied from, relative to root.</param>
        /// <param name="toPath">The destination path, including the new name for the file or folder, relative to root.</param>
        /// <returns>
        /// A metadata <see cref="Entry"/> for the moved file or folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Entry Copy(string fromPath, string toPath)
        {
            NameValueCollection request = new NameValueCollection();
            request.Add("root", this.root);
            request.Add("from_path", fromPath);
            request.Add("to_path", toPath);
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObject<Entry>("fileops/copy", request);
        }

        /// <summary>
        /// Copies another user's Dropbox file without the need to download and re-upload the file.
        /// </summary>
        /// <param name="fromFileRef">
        /// The reference to another user's Dropbox file to be copied from, 
        /// obtained by calling the <see cref="M:CreateFileRef"/> method.
        /// </param>
        /// <param name="toPath">The destination file path, including the new name for the file, relative to root.</param>
        /// <returns>
        /// A metadata <see cref="Entry"/> for the copied file.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        /// <seealso cref="M:CreateFileRef"/>
        public Entry CopyFileRef(string fromFileRef, string toPath)
        {
            NameValueCollection request = new NameValueCollection();
            request.Add("root", this.root);
            request.Add("from_copy_ref", fromFileRef);
            request.Add("to_path", toPath);
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObject<Entry>("fileops/copy", request);
        }

        /// <summary>
        /// Uploads a file.
        /// </summary>
        /// <param name="file">The file resource to be uploaded.</param>
        /// <param name="path">
        /// The path to the file you want to write to, relative to root. 
        /// This parameter should not point to a folder.
        /// </param>
        /// <returns>A metadata <see cref="Entry"/> for the uploaded file.</returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Entry UploadFile(IResource file, string path)
        {
            return this.UploadFile(file, path, true, null);
        }

        /// <summary>
        /// Uploads a file.
        /// </summary>
        /// <param name="file">The file resource to be uploaded.</param>
        /// <param name="path">
        /// The path to the file you want to write to, relative to root. 
        /// This parameter should not point to a folder.
        /// </param>
        /// <param name="overwrite">
        /// If <see langword="true"/>, the existing file will be overwritten by the new one. 
        /// If <see langword="false"/>, the new file will be automatically renamed. 
        /// The new name can be obtained from the returned metadata.
        /// </param>
        /// <param name="revision">
        /// The revision of the file you're editing. 
        /// If <paramref name="revision"/> matches the latest version of the file on the user's Dropbox, that file will be replaced.
        /// </param>
        /// <returns>A metadata <see cref="Entry"/> for the uploaded file.</returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Entry UploadFile(IResource file, string path, bool overwrite, string revision)
        {
            HttpEntityRequestCallback requestCallback = new HttpEntityRequestCallback(file, typeof(Entry), this.RestTemplate.MessageConverters);
            MessageConverterResponseExtractor<Entry> responseExtractor = new MessageConverterResponseExtractor<Entry>(this.RestTemplate.MessageConverters);
            return this.RestTemplate.Execute<Entry>(this.BuildUploadUrl(path, overwrite, revision), HttpMethod.PUT, requestCallback, responseExtractor);
        }

        /// <summary>
        /// Downloads a file and its metadata.
        /// </summary>
        /// <param name="path">The Dropbox path to the file you want to retrieve, relative to root.</param>
        /// <returns>
        /// A <see cref="DropboxFile"/> object containing the file's content and metadata.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public DropboxFile DownloadFile(string path)
        {
            return this.DownloadFile(path, null);
        }

        /// <summary>
        /// Downloads a file and its metadata.
        /// </summary>
        /// <param name="path">The Dropbox path to the file you want to retrieve, relative to root.</param>
        /// <param name="revision">
        /// The revision of the file to retrieve, or <see langword="null"/> for the latest version.
        /// </param>
        /// <returns>
        /// A <see cref="DropboxFile"/> object containing the file's content and metadata.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public DropboxFile DownloadFile(string path, string revision)
        {
            AcceptHeaderRequestCallback requestCallback = new AcceptHeaderRequestCallback(typeof(DropboxFile), this.RestTemplate.MessageConverters);
            DropboxFileResponseExtractor responseExtractor = new DropboxFileResponseExtractor(this.RestTemplate.MessageConverters);
            return this.RestTemplate.Execute<DropboxFile>(this.BuildDownloadUrl(path, revision), HttpMethod.GET, requestCallback, responseExtractor);
        }

        /// <summary>
        /// Downloads part of a file and its metadata.
        /// </summary>
        /// <param name="path">The Dropbox path to the file you want to retrieve, relative to root.</param>
        /// <param name="startOffset">The zero-based starting position of the part file.</param>
        /// <param name="length">The number of bytes of the part file.</param>
        /// <returns>
        /// A <see cref="DropboxFile"/> object containing the part of file's content and metadata.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public DropboxFile DownloadPartialFile(string path, long startOffset, long length)
        {
            return this.DownloadPartialFile(path, startOffset, length, null);
        }

        /// <summary>
        /// Downloads part of a file and its metadata.
        /// </summary>
        /// <param name="path">The Dropbox path to the file you want to retrieve, relative to root.</param>
        /// <param name="startOffset">The zero-based starting position of the part file.</param>
        /// <param name="length">The number of bytes of the part file.</param>
        /// <param name="revision">
        /// The revision of the file to retrieve, or <see langword="null"/> for the latest version.
        /// </param>
        /// <returns>
        /// A <see cref="DropboxFile"/> object containing the part of file's content and metadata.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public DropboxFile DownloadPartialFile(string path, long startOffset, long length, string revision)
        {
            HttpEntity requestEntity = new HttpEntity();
            requestEntity.Headers.Add("Range", String.Format("bytes={0}-{1}", startOffset, startOffset + length - 1));
            HttpEntityRequestCallback requestCallback = new HttpEntityRequestCallback(requestEntity, typeof(DropboxFile), this.RestTemplate.MessageConverters);
            DropboxFileResponseExtractor responseExtractor = new DropboxFileResponseExtractor(this.RestTemplate.MessageConverters);
            return this.RestTemplate.Execute<DropboxFile>(this.BuildDownloadUrl(path, revision), HttpMethod.GET, requestCallback, responseExtractor);
        }

        /// <summary>
        /// Keeps up with changes to files and folders in a user's Dropbox. 
        /// <para/>
        /// You can periodically call this method to get a list of metadatas, 
        /// which are instructions on how to update your local state to match the server's state.
        /// </summary>
        /// <param name="cursor">
        /// A value that is used to keep track of your current state. 
        /// <para/>
        /// On the first call, you should pass in <see langword="null"/>. 
        /// On subsequent calls, pass in the cursor value returned by the previous call.
        /// </param>
        /// <returns>
        /// A single <see cref="DeltaPage">delta page</see> of results. 
        /// <para/>
        /// The <see cref="DeltaPage"/>'s HasMore property will tell you whether the server has more pages of results to return. 
        /// If the server doesn't have more results, wait for at least five minutes (preferably longer) and poll again.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public DeltaPage Delta(string cursor)
        {
            NameValueCollection request = new NameValueCollection();
            if (cursor != null)
            {
                request.Add("cursor", cursor);
            }
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObject<DeltaPage>("delta/", request);
        }

        /// <summary>
        /// Retrieves file or folder metadata.
        /// </summary>
        /// <param name="path">The Dropbox path to the file or folder, relative to root.</param>
        /// <returns>
        /// A metadata <see cref="Entry"/> for the file or folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Entry GetMetadata(string path)
        {
            return this.GetMetadata(path, new MetadataParameters());
        }

        /// <summary>
        /// Retrieves file or folder metadata. 
        /// May return <see langword="null"/> if an hash is provided through parameters.
        /// </summary>
        /// <param name="path">The Dropbox path to the file or folder, relative to root.</param>
        /// <param name="parameters">The parameters for retrieving file and folder metadata.</param>
        /// <returns>
        /// A metadata <see cref="Entry"/> for the file or folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Entry GetMetadata(string path, MetadataParameters parameters)
        {
            return this.RestTemplate.GetForObject<Entry>(this.BuildMetadataUrl(path, parameters));
        }

        /// <summary>
        /// Obtains metadata for the previous revisions of a file.
        /// </summary>
        /// <param name="path">The Dropbox path to the file, relative to root.</param>
        /// <returns>
        /// A list of metadata <see cref="Entry">entries</see>.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public IList<Entry> GetRevisions(string path)
        {
            return this.GetRevisions(path, 0);
        }

        /// <summary>
        /// Obtains metadata for the previous revisions of a file.
        /// </summary>
        /// <param name="path">The Dropbox path to the file, relative to root.</param>
        /// <param name="revLimit">
        /// The maximum numbers of revisions to retrieve. Default is 10, Max is 1,000.
        /// </param>
        /// <returns>
        /// A list of metadata <see cref="Entry">entries</see>.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public IList<Entry> GetRevisions(string path, int revLimit)
        {
            NameValueCollection parameters = new NameValueCollection();
            if (revLimit > 0)
            {
                parameters.Add("rev_limit", revLimit.ToString());
            }
            this.AddLocaleTo(parameters);
            return this.RestTemplate.GetForObject<IList<Entry>>(this.BuildUrl("revisions/", path, parameters));
        }

        /// <summary>
        /// Restores a file path to a previous revision.
        /// </summary>
        /// <param name="path">The Dropbox path to the file, relative to root.</param>
        /// <param name="revision">The revision of the file to restore.</param>
        /// <returns>
        /// A metadata <see cref="Entry"/> for the restored file. 
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public Entry Restore(string path, string revision)
        {
            NameValueCollection request = new NameValueCollection();
            request.Add("rev", revision);
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObject<Entry>(this.BuildUrl("restore/", path), request);
        }

        /// <summary>
        /// Searches for all files and folders that match the search query.
        /// </summary>
        /// <param name="path">The Dropbox path to the folder you want to search from, relative to root.</param>
        /// <param name="query">The search string. Must be at least three characters long.</param>
        /// <returns>
        /// A list of metadata <see cref="Entry">entries</see> for any matching files and folders.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public IList<Entry> Search(string path, string query)
        {
            return this.Search(path, query, 0, false);
        }

        /// <summary>
        /// Searches for all files and folders that match the search query.
        /// </summary>
        /// <param name="path">The Dropbox path to the folder you want to search from, relative to root.</param>
        /// <param name="query">The search string. Must be at least three characters long.</param>
        /// <param name="fileLimit">
        /// The maximum numbers of file entries to retrieve. The maximum and default value is 1000.
        /// </param>
        /// <param name="includeDeleted">
        /// If <see langword="true"/>, then files and folders that have been deleted will also be included in the search.
        /// </param>
        /// <returns>
        /// A list of metadata <see cref="Entry">entries</see> for any matching files and folders.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public IList<Entry> Search(string path, string query, int fileLimit, bool includeDeleted)
        {
            NameValueCollection request = this.BuildSearchParameters(path, query, fileLimit, includeDeleted);
            return this.RestTemplate.PostForObject<IList<Entry>>(this.BuildUrl("search/", path), request);
        }

        /// <summary>
        /// Creates and returns a shareable link to files or folders.
        /// </summary>
        /// <param name="path">
        /// The Dropbox path to the file or folder you want a sharable link to, relative to root.
        /// </param>
        /// <returns>
        /// A shareable link to the file or folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public DropboxLink GetShareableLink(string path)
        {
            NameValueCollection request = new NameValueCollection();
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObject<DropboxLink>(this.BuildUrl("shares/", path), request);
        }

        /// <summary>
        /// Returns a link for streaming media files.
        /// </summary>
        /// <param name="path">
        /// The Dropbox path to the media file you want a direct link to, relative to root.
        /// </param>
        /// <returns>
        /// A direct link to the media file.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public DropboxLink GetMediaLink(string path)
        {
            NameValueCollection request = new NameValueCollection();
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObject<DropboxLink>(this.BuildUrl("media/", path), request);
        }

        /// <summary>
        /// Downloads a thumbnail for an image and its metadata.
        /// </summary>
        /// <param name="path">
        /// The Dropbox path to the image file you want to thumbnail, relative to root.
        /// </param>
        /// <param name="format">The image format of the thumbnail to download.</param>
        /// <param name="size">The size of the thumbnail to download.</param>
        /// <returns>
        /// A <see cref="DropboxFile"/> object containing the thumbnail's content and metadata.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public DropboxFile DownloadThumbnail(string path, ThumbnailFormat format, ThumbnailSize size)
        {
            return this.RestTemplate.GetForObject<DropboxFile>(this.BuildThumbnailsUrl(path, format, size));
        }
#endif

        /// <summary>
        /// Asynchronously retrieves the authenticated user's Dropbox profile details.
        /// </summary>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a <see cref="DropboxProfile"/>object representing the user's profile.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler GetUserProfileAsync(Action<RestOperationCompletedEventArgs<DropboxProfile>> operationCompleted)
        {
            NameValueCollection parameters = new NameValueCollection();
            this.AddLocaleTo(parameters);
            return this.RestTemplate.GetForObjectAsync<DropboxProfile>(BuildUrl("account/info", parameters), operationCompleted);
        }

        /// <summary>
        /// Asynchronously creates a folder.
        /// </summary>
        /// <param name="path">The Dropbox path to the new folder to create relative to root.</param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a metadata <see cref="Entry"/> for the new folder.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler CreateFolderAsync(string path, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted)
        {
            NameValueCollection request = new NameValueCollection();
            request.Add("root", this.root);
            request.Add("path", path);
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObjectAsync<Entry>("fileops/create_folder", request, operationCompleted);
        }

        /// <summary>
        /// Asynchronously deletes a file or folder.
        /// </summary>
        /// <param name="path">The Dropbox path to the file or folder to be deleted relative to root.</param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a metadata <see cref="Entry"/> for the deleted file or folder.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler DeleteAsync(string path, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted)
        {
            NameValueCollection request = new NameValueCollection();
            request.Add("root", this.root);
            request.Add("path", path);
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObjectAsync<Entry>("fileops/delete", request, operationCompleted);
        }

        /// <summary>
        /// Asynchronously moves a file or folder to a new location.
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to be copied from, relative to root.</param>
        /// <param name="toPath">The destination path, including the new name for the file or folder, relative to root.</param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a metadata <see cref="Entry"/> for the moved file or folder.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler MoveAsync(string fromPath, string toPath, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted)
        {
            NameValueCollection request = new NameValueCollection();
            request.Add("root", this.root);
            request.Add("from_path", fromPath);
            request.Add("to_path", toPath);
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObjectAsync<Entry>("fileops/move", request, operationCompleted);
        }

        /// <summary>
        /// Asynchronously creates a reference to another user's Dropbox file that can be used to 
        /// copy files directly between two Dropbox accounts (with permission from both users) 
        /// using the <see cref="M:CopyFileRefAsync"/> method.
        /// No need to download and re-upload files.
        /// </summary>
        /// <param name="path">The path to the file you want a reference.</param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a <see cref="FileRef">reference to another user's Dropbox file</see>.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        /// <seealso cref="M:CopyFileRefAsync"/>
        public RestOperationCanceler CreateFileRefAsync(string path, Action<RestOperationCompletedEventArgs<FileRef>> operationCompleted)
        {
            return this.RestTemplate.GetForObjectAsync<FileRef>(this.BuildUrl("copy_ref/", path), operationCompleted);
        }

        /// <summary>
        /// Asynchronously copies a file or folder to a new location.
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to be copied from, relative to root.</param>
        /// <param name="toPath">The destination path, including the new name for the file or folder, relative to root.</param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a metadata <see cref="Entry"/> for the moved file or folder.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler CopyAsync(string fromPath, string toPath, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted)
        {
            NameValueCollection request = new NameValueCollection();
            request.Add("root", this.root);
            request.Add("from_path", fromPath);
            request.Add("to_path", toPath);
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObjectAsync<Entry>("fileops/copy", request, operationCompleted);
        }

        /// <summary>
        /// Asynchronously copies another user's Dropbox file without the need to download and re-upload the file.
        /// </summary>
        /// <param name="fromFileRef">
        /// The reference to another user's Dropbox file to be copied from, 
        /// obtained by calling the <see cref="M:CreateFileRefAsync"/> method.
        /// </param>
        /// <param name="toPath">The destination file path, including the new name for the file, relative to root.</param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a metadata <see cref="Entry"/> for the copied file.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        /// <seealso cref="M:CreateFileRefAsync"/>
        public RestOperationCanceler CopyFileRefAsync(string fromFileRef, string toPath, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted)
        {
            NameValueCollection request = new NameValueCollection();
            request.Add("root", this.root);
            request.Add("from_copy_ref", fromFileRef);
            request.Add("to_path", toPath);
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObjectAsync<Entry>("fileops/copy", request, operationCompleted);
        }

        /// <summary>
        /// Asynchronously uploads a file.
        /// </summary>
        /// <param name="file">The file resource to be uploaded.</param>
        /// <param name="path">
        /// The path to the file you want to write to, relative to root. 
        /// This parameter should not point to a folder.
        /// </param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a metadata <see cref="Entry"/> for the uploaded file.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler UploadFileAsync(IResource file, string path, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted)
        {
            return this.UploadFileAsync(file, path, true, null, operationCompleted);
        }

        /// <summary>
        /// Asynchronously uploads a file.
        /// </summary>
        /// <param name="file">The file resource to be uploaded.</param>
        /// <param name="path">
        /// The path to the file you want to write to, relative to root. 
        /// This parameter should not point to a folder.
        /// </param>
        /// <param name="overwrite">
        /// If <see langword="true"/>, the existing file will be overwritten by the new one. 
        /// If <see langword="false"/>, the new file will be automatically renamed. 
        /// The new name can be obtained from the returned metadata.
        /// </param>
        /// <param name="revision">
        /// The revision of the file you're editing. 
        /// If <paramref name="revision"/> matches the latest version of the file on the user's Dropbox, that file will be replaced.
        /// </param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a metadata <see cref="Entry"/> for the uploaded file.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler UploadFileAsync(IResource file, string path, bool overwrite, string revision, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted)
        {
            HttpEntityRequestCallback requestCallback = new HttpEntityRequestCallback(file, typeof(Entry), this.RestTemplate.MessageConverters);
            MessageConverterResponseExtractor<Entry> responseExtractor = new MessageConverterResponseExtractor<Entry>(this.RestTemplate.MessageConverters);
            return this.RestTemplate.ExecuteAsync<Entry>(this.BuildUploadUrl(path, overwrite, revision), HttpMethod.PUT, requestCallback, responseExtractor, operationCompleted);
        }

        /// <summary>
        /// Asynchronously downloads a file and its metadata.
        /// </summary>
        /// <param name="path">The Dropbox path to the file you want to retrieve, relative to root.</param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a <see cref="DropboxFile"/> object containing the file's content and metadata.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler DownloadFileAsync(string path, Action<RestOperationCompletedEventArgs<DropboxFile>> operationCompleted)
        {
            return this.DownloadFileAsync(path, null, operationCompleted);
        }

        /// <summary>
        /// Asynchronously downloads a file and its metadata.
        /// </summary>
        /// <param name="path">The Dropbox path to the file you want to retrieve, relative to root.</param>
        /// <param name="revision">
        /// The revision of the file to retrieve, or <see langword="null"/> for the latest version.
        /// </param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a <see cref="DropboxFile"/> object containing the file's content and metadata.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler DownloadFileAsync(string path, string revision, Action<RestOperationCompletedEventArgs<DropboxFile>> operationCompleted)
        {
            AcceptHeaderRequestCallback requestCallback = new AcceptHeaderRequestCallback(typeof(DropboxFile), this.RestTemplate.MessageConverters);
            DropboxFileResponseExtractor responseExtractor = new DropboxFileResponseExtractor(this.RestTemplate.MessageConverters);
            return this.RestTemplate.ExecuteAsync<DropboxFile>(this.BuildDownloadUrl(path, revision), HttpMethod.GET, requestCallback, responseExtractor, operationCompleted);
        }

        /// <summary>
        /// Asynchronously downloads part of a file and its metadata.
        /// </summary>
        /// <param name="path">The Dropbox path to the file you want to retrieve, relative to root.</param>
        /// <param name="startOffset">The zero-based starting position of the part file.</param>
        /// <param name="length">The number of bytes of the part file.</param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides <see cref="DropboxFile"/> object containing the part of file's content and metadata.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler DownloadPartialFileAsync(string path, long startOffset, long length, Action<RestOperationCompletedEventArgs<DropboxFile>> operationCompleted)
        {
            return this.DownloadPartialFileAsync(path, startOffset, length, null, operationCompleted);
        }

        /// <summary>
        /// Asynchronously downloads part of a file and its metadata.
        /// </summary>
        /// <param name="path">The Dropbox path to the file you want to retrieve, relative to root.</param>
        /// <param name="startOffset">The zero-based starting position of the part file.</param>
        /// <param name="length">The number of bytes of the part file.</param>
        /// <param name="revision">
        /// The revision of the file to retrieve, or <see langword="null"/> for the latest version.
        /// </param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides <see cref="DropboxFile"/> object containing the part of file's content and metadata.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler DownloadPartialFileAsync(string path, long startOffset, long length, string revision, Action<RestOperationCompletedEventArgs<DropboxFile>> operationCompleted)
        {
            HttpEntity requestEntity = new HttpEntity();
            requestEntity.Headers.Add("Range", String.Format("bytes={0}-{1}", startOffset, startOffset + length - 1));
            HttpEntityRequestCallback requestCallback = new HttpEntityRequestCallback(requestEntity, typeof(DropboxFile), this.RestTemplate.MessageConverters);
            DropboxFileResponseExtractor responseExtractor = new DropboxFileResponseExtractor(this.RestTemplate.MessageConverters);
            return this.RestTemplate.ExecuteAsync<DropboxFile>(this.BuildDownloadUrl(path, revision), HttpMethod.GET, requestCallback, responseExtractor, operationCompleted);
        }

        /// <summary>
        /// Asynchronously keeps up with changes to files and folders in a user's Dropbox. 
        /// <para/>
        /// You can periodically call this method to get a list of metadatas, 
        /// which are instructions on how to update your local state to match the server's state.
        /// </summary>
        /// <param name="cursor">
        /// A value that is used to keep track of your current state. 
        /// <para/>
        /// On the first call, you should pass in <see langword="null"/>. 
        /// On subsequent calls, pass in the cursor value returned by the previous call.
        /// </param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a single <see cref="DeltaPage">delta page</see> of results. 
        /// <para/>
        /// The <see cref="DeltaPage"/>'s HasMore property will tell you whether the server has more pages of results to return. 
        /// If the server doesn't have more results, wait for at least five minutes (preferably longer) and poll again.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler DeltaAsync(string cursor, Action<RestOperationCompletedEventArgs<DeltaPage>> operationCompleted)
        {
            NameValueCollection request = new NameValueCollection();
            if (cursor != null)
            {
                request.Add("cursor", cursor);
            }
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObjectAsync<DeltaPage>("delta/", request, operationCompleted);
        }

        /// <summary>
        /// Asynchronously retrieves file or folder metadata.
        /// </summary>
        /// <param name="path">The Dropbox path to the file or folder, relative to root.</param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a metadata <see cref="Entry"/> for the file or folder.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler GetMetadataAsync(string path, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted)
        {
            return this.GetMetadataAsync(path, new MetadataParameters(), operationCompleted);
        }

        /// <summary>
        /// Asynchronously retrieves file or folder metadata. 
        /// May return <see langword="null"/> if an hash is provided through parameters.
        /// </summary>
        /// <param name="path">The Dropbox path to the file or folder, relative to root.</param>
        /// <param name="parameters">The parameters for retrieving file and folder metadata.</param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a metadata <see cref="Entry"/> for the file or folder.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler GetMetadataAsync(string path, MetadataParameters parameters, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted)
        {
            return this.RestTemplate.GetForObjectAsync<Entry>(this.BuildMetadataUrl(path, parameters), operationCompleted);
        }

        /// <summary>
        /// Asynchronously obtains metadata for the previous revisions of a file.
        /// </summary>
        /// <param name="path">The Dropbox path to the file, relative to root.</param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a list of metadata <see cref="Entry">entries</see>.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler GetRevisionsAsync(string path, Action<RestOperationCompletedEventArgs<IList<Entry>>> operationCompleted)
        {
            return this.GetRevisionsAsync(path, 0, operationCompleted);
        }

        /// <summary>
        /// Asynchronously obtains metadata for the previous revisions of a file.
        /// </summary>
        /// <param name="path">The Dropbox path to the file, relative to root.</param>
        /// <param name="revLimit">
        /// The maximum numbers of revisions to retrieve. Default is 10, Max is 1,000.
        /// </param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a list of metadata <see cref="Entry">entries</see>.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler GetRevisionsAsync(string path, int revLimit, Action<RestOperationCompletedEventArgs<IList<Entry>>> operationCompleted)
        {
            NameValueCollection parameters = new NameValueCollection();
            if (revLimit > 0)
            {
                parameters.Add("rev_limit", revLimit.ToString());
            }
            this.AddLocaleTo(parameters);
            return this.RestTemplate.GetForObjectAsync<IList<Entry>>(this.BuildUrl("revisions/", path, parameters), operationCompleted);
        }

        /// <summary>
        /// Asynchronously restores a file path to a previous revision.
        /// </summary>
        /// <param name="path">The Dropbox path to the file, relative to root.</param>
        /// <param name="revision">The revision of the file to restore.</param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a metadata <see cref="Entry"/> for the restored file. 
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler RestoreAsync(string path, string revision, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted)
        {
            NameValueCollection request = new NameValueCollection();
            request.Add("rev", revision);
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObjectAsync<Entry>(this.BuildUrl("restore/", path), request, operationCompleted);
        }

        /// <summary>
        /// Asynchronously searches for all files and folders that match the search query.
        /// </summary>
        /// <param name="path">The Dropbox path to the folder you want to search from, relative to root.</param>
        /// <param name="query">The search string. Must be at least three characters long.</param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a list of metadata <see cref="Entry">entries</see> for any matching files and folders.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler SearchAsync(string path, string query, Action<RestOperationCompletedEventArgs<IList<Entry>>> operationCompleted)
        {
            return this.SearchAsync(path, query, 0, false, operationCompleted);
        }

        /// <summary>
        /// Asynchronously searches for all files and folders that match the search query.
        /// </summary>
        /// <param name="path">The Dropbox path to the folder you want to search from, relative to root.</param>
        /// <param name="query">The search string. Must be at least three characters long.</param>
        /// <param name="fileLimit">
        /// The maximum numbers of file entries to retrieve. The maximum and default value is 1000.
        /// </param>
        /// <param name="includeDeleted">
        /// If <see langword="true"/>, then files and folders that have been deleted will also be included in the search.
        /// </param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a list of metadata <see cref="Entry">entries</see> for any matching files and folders.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler SearchAsync(string path, string query, int fileLimit, bool includeDeleted, Action<RestOperationCompletedEventArgs<IList<Entry>>> operationCompleted)
        {
            NameValueCollection request = this.BuildSearchParameters(path, query, fileLimit, includeDeleted);
            return this.RestTemplate.PostForObjectAsync<IList<Entry>>(this.BuildUrl("search/", path), request, operationCompleted);
        }

        /// <summary>
        /// Asynchronously creates and returns a shareable link to files or folders.
        /// </summary>
        /// <param name="path">
        /// The Dropbox path to the file or folder you want a sharable link to, relative to root.
        /// </param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a shareable link to the file or folder.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler GetShareableLinkAsync(string path, Action<RestOperationCompletedEventArgs<DropboxLink>> operationCompleted)
        {
            NameValueCollection request = new NameValueCollection();
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObjectAsync<DropboxLink>(this.BuildUrl("shares/", path), request, operationCompleted);
        }

        /// <summary>
        /// Asynchronously returns a link for streaming media files.
        /// </summary>
        /// <param name="path">
        /// The Dropbox path to the media file you want a direct link to, relative to root.
        /// </param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a direct link to the media file.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler GetMediaLinkAsync(string path, Action<RestOperationCompletedEventArgs<DropboxLink>> operationCompleted)
        {
            NameValueCollection request = new NameValueCollection();
            this.AddLocaleTo(request);
            return this.RestTemplate.PostForObjectAsync<DropboxLink>(this.BuildUrl("media/", path), request, operationCompleted);
        }

        /// <summary>
        /// Asynchronously downloads a thumbnail for an image and its metadata.
        /// </summary>
        /// <param name="path">
        /// The Dropbox path to the image file you want to thumbnail, relative to root.
        /// </param>
        /// <param name="format">The image format of the thumbnail to download.</param>
        /// <param name="size">The size of the thumbnail to download.</param>
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a <see cref="DropboxFile"/> object containing the thumbnail's content and metadata.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        public RestOperationCanceler DownloadThumbnailAsync(string path, ThumbnailFormat format, ThumbnailSize size, Action<RestOperationCompletedEventArgs<DropboxFile>> operationCompleted)
        {
            return this.RestTemplate.GetForObjectAsync<DropboxFile>(this.BuildThumbnailsUrl(path, format, size), operationCompleted);
        }
#endif

        /// <summary>
        /// Gets the underlying <see cref="IRestOperations"/> object allowing for consumption of Dropbox endpoints 
        /// that may not be otherwise covered by the API binding. 
        /// </summary>
        /// <remarks>
        /// The <see cref="IRestOperations"/> object returned is configured to include an OAuth "Authorization" header on all requests.
        /// </remarks>
        public IRestOperations RestOperations
        {
            get { return this.RestTemplate; }
        }

        #endregion

        /// <summary>
        /// Enables customization of the <see cref="RestTemplate"/> used to consume provider API resources.
        /// </summary>
        /// <remarks>
        /// An example use case might be to configure a custom error handler. 
        /// Note that this method is called after the RestTemplate has been configured with the message converters returned from GetMessageConverters().
        /// </remarks>
        /// <param name="restTemplate">The RestTemplate to configure.</param>
        protected override void ConfigureRestTemplate(RestTemplate restTemplate)
        {
            restTemplate.BaseAddress = API_URI_BASE;
            restTemplate.ErrorHandler = new DropboxErrorHandler();
        }

        /// <summary>
        /// Returns a list of <see cref="IHttpMessageConverter"/>s to be used by the internal <see cref="RestTemplate"/>.
        /// </summary>
        /// <remarks>
        /// This implementation adds <see cref="SpringJsonHttpMessageConverter"/> and <see cref="ByteArrayHttpMessageConverter"/> to the default list.
        /// </remarks>
        /// <returns>
        /// The list of <see cref="IHttpMessageConverter"/>s to be used by the internal <see cref="RestTemplate"/>.
        /// </returns>
        protected override IList<IHttpMessageConverter> GetMessageConverters()
        {
            JsonMapper jsonMapper = new JsonMapper();
            jsonMapper.RegisterDeserializer(typeof(DropboxProfile), new DropboxProfileDeserializer());
            jsonMapper.RegisterDeserializer(typeof(Entry), new EntryDeserializer());
            jsonMapper.RegisterDeserializer(typeof(IList<Entry>), new EntryListDeserializer());
            jsonMapper.RegisterDeserializer(typeof(DropboxLink), new DropboxLinkDeserializer());
            jsonMapper.RegisterDeserializer(typeof(FileRef), new FileRefDeserializer());
            jsonMapper.RegisterDeserializer(typeof(DeltaPage), new DeltaPageDeserializer());

            IList<IHttpMessageConverter> converters = base.GetMessageConverters();
            converters.Add(new ResourceHttpMessageConverter());
            converters.Add(new SpringJsonHttpMessageConverter(jsonMapper));
            converters.Add(new DropboxFileHttpMessageConverter(jsonMapper));
            return converters;
        }

        private void AddLocaleTo(NameValueCollection parameters)
        {
            if (this.locale != null)
            {
                parameters.Add("locale", this.locale);
            }
        }

        private NameValueCollection BuildSearchParameters(string dropboxPath, string query, int fileLimit, bool includeDeleted)
        {
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("query", query);
            if (fileLimit > 0)
            {
                parameters.Add("file_limit", fileLimit.ToString());
            }
            if (includeDeleted)
            {
                parameters.Add("include_deleted", "true");
            }
            this.AddLocaleTo(parameters);
            return parameters;
        }

        private string BuildUploadUrl(string dropboxPath, bool overwrite, string revision)
        {
            NameValueCollection parameters = new NameValueCollection();
            this.AddLocaleTo(parameters);
            if (!overwrite)
            {
                parameters.Add("overwrite", "false");
            }
            if (!String.IsNullOrEmpty(revision))
            {
                parameters.Add("parent_rev", revision);
            }
            return this.BuildUrl("https://api-content.dropbox.com/1/files_put/", dropboxPath, parameters);
        }

        private string BuildDownloadUrl(string dropboxPath, string revision)
        {
            NameValueCollection parameters = new NameValueCollection();
            if (!String.IsNullOrEmpty(revision))
            {
                parameters.Add("rev", revision);
            }
            return this.BuildUrl("https://api-content.dropbox.com/1/files/", dropboxPath, parameters);
        }

        private string BuildThumbnailsUrl(string dropboxPath, ThumbnailFormat format, ThumbnailSize size)
        {
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("format", format.ToString().ToUpper());
            parameters.Add("size", ThumbnailSizeToString(size));
            return this.BuildUrl("https://api-content.dropbox.com/1/thumbnails/", dropboxPath, parameters);
        }

        private string BuildMetadataUrl(string dropboxPath, MetadataParameters metadataParameters)
        {
            NameValueCollection parameters = new NameValueCollection();
            if (metadataParameters.FileLimit > 0)
            {
                parameters.Add("file_limit", metadataParameters.FileLimit.ToString());
            }
            if (metadataParameters.Hash != null)
            {
                parameters.Add("hash", metadataParameters.Hash);
            }
            if (!metadataParameters.IncludeContents)
            {
                parameters.Add("list", "false");
            }
            if (metadataParameters.IncludeDeleted)
            {
                parameters.Add("include_deleted", "true");
            }
            if (metadataParameters.Revision != null)
            {
                parameters.Add("rev", metadataParameters.Revision);
            }
            this.AddLocaleTo(parameters);
            return this.BuildUrl("metadata/", dropboxPath, parameters);
        }

        private string BuildUrl(string path, string dropboxPath)
        {
            return this.BuildUrl(path, dropboxPath, new NameValueCollection());
        }

        private string BuildUrl(string path, string dropboxPath, NameValueCollection parameters)
        {
            return BuildUrl(
                path.TrimEnd('/') + "/" + this.root + "/" + DropboxPathEncode(dropboxPath.TrimStart('/')), 
                parameters);
        }

        private static string BuildUrl(string path, NameValueCollection parameters)
        {
            StringBuilder qsBuilder = new StringBuilder();
            bool isFirst = true;
            foreach (string key in parameters)
            {
                if (isFirst)
                {
                    qsBuilder.Append('?');
                    isFirst = false;
                }
                else
                {
                    qsBuilder.Append('&');
                }
                qsBuilder.Append(HttpUtils.UrlEncode(key));
                qsBuilder.Append('=');
                qsBuilder.Append(HttpUtils.UrlEncode(parameters[key]));
            }
            return path + qsBuilder.ToString();
        }

        private const string DROPBOX_PATH_UNRESERVED_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_./";

        private static string DropboxPathEncode(string path)
        {
            StringBuilder result = new StringBuilder();
            foreach (char symbol in path)
            {
                if (DROPBOX_PATH_UNRESERVED_CHARS.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(new char[] { symbol });
                    foreach (byte b in bytes)
                    {
                        result.AppendFormat("%{0:X2}", b);
                    }
                }
            }
            return result.ToString();
        }

        private static string ThumbnailSizeToString(ThumbnailSize size)
        {
            switch(size)
            {
                case ThumbnailSize.Small:
                    return "s";
                case ThumbnailSize.Medium:
                    return "m";
                case ThumbnailSize.Large:
                    return "l";
                case ThumbnailSize.ExtraLarge:
                    return "xl";
                default:
                    return "small";
            }
        }

        // SPRNETSOCIALDB-9
        private class DropboxFileResponseExtractor : MessageConverterResponseExtractor<DropboxFile>
        {
            public DropboxFileResponseExtractor(IList<IHttpMessageConverter> messageConverters)
                : base(messageConverters)
            {
            }

            protected override bool HasMessageBody(IClientHttpResponse response)
            {
                return true;
            }
        }
    }
}