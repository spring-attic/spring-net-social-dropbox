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
using System.Collections.Generic;
#if NET_4_0 || SILVERLIGHT_5
using System.Threading;
using System.Threading.Tasks;
#endif

using Spring.IO;
using Spring.Rest.Client;

namespace Spring.Social.Dropbox.Api
{
    /// <summary>
    /// Interface specifying a basic set of operations for interacting with Dropbox.
    /// </summary>
    /// <author>Bruno Baia</author>
    public interface IDropbox : IApiBinding
    {
        /// <summary>
        /// Gets the application access level. 
        /// </summary>
        AccessLevel AccessLevel { get; }

        /// <summary>
        /// Gets or sets the locale parameter to specify language settings of content responses. 
        /// <para/>
        /// Uses IETF language tag (ex: "pt-BR" for Brazilian Portuguese).
        /// <para/>
        /// Default is <see langword="null"/> for Dropbox server's locale.
        /// </summary>
        string Locale { get; set; }

#if NET_4_0 || SILVERLIGHT_5  
        /// <summary>
        /// Asynchronously retrieves the authenticated user's Dropbox profile details.
        /// </summary>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a <see cref="DropboxProfile"/> object representing the user's profile.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        Task<DropboxProfile> GetUserProfileAsync();

        /// <summary>
        /// Asynchronously creates a folder.
        /// </summary>
        /// <param name="path">The Dropbox path to the new folder to create relative to root.</param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a metadata <see cref="Entry"/> for the new folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        Task<Entry> CreateFolderAsync(string path);

        /// <summary>
        /// Asynchronously deletes a file or folder.
        /// </summary>
        /// <param name="path">The Dropbox path to the file or folder to be deleted relative to root.</param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a metadata <see cref="Entry"/> for the deleted file or folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        Task<Entry> DeleteAsync(string path);

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
        Task<Entry> MoveAsync(string fromPath, string toPath);

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
        Task<FileRef> CreateFileRefAsync(string path);

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
        Task<Entry> CopyAsync(string fromPath, string toPath);

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
        Task<Entry> CopyFileRefAsync(string fromFileRef, string toPath);

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
        Task<Entry> UploadFileAsync(IResource file, string path);

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
        Task<Entry> UploadFileAsync(IResource file, string path, bool overwrite, string revision, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously downloads a file and its metadata.
        /// </summary>
        /// <param name="path">The Dropbox path to the file you want to retrieve, relative to root.</param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a <see cref="DropboxFile"/> object containing the file's content and metadata.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        Task<DropboxFile> DownloadFileAsync(string path);

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
        Task<DropboxFile> DownloadFileAsync(string path, string revision, CancellationToken cancellationToken);

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
        Task<DropboxFile> DownloadPartialFileAsync(string path, long startOffset, long length);

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
        Task<DropboxFile> DownloadPartialFileAsync(string path, long startOffset, long length, string revision, CancellationToken cancellationToken);

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
        Task<DeltaPage> DeltaAsync(string cursor);

        /// <summary>
        /// Asynchronously retrieves file or folder metadata.
        /// </summary>
        /// <param name="path">The Dropbox path to the file or folder, relative to root.</param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a metadata <see cref="Entry"/> for the file or folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        Task<Entry> GetMetadataAsync(string path);

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
        Task<Entry> GetMetadataAsync(string path, MetadataParameters parameters);

        /// <summary>
        /// Asynchronously obtains metadata for the previous revisions of a file.
        /// </summary>
        /// <param name="path">The Dropbox path to the file, relative to root.</param>
        /// <returns>
        /// A 'Task' that represents the asynchronous operation that can return 
        /// a list of metadata <see cref="Entry">entries</see>.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        Task<IList<Entry>> GetRevisionsAsync(string path);

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
        Task<IList<Entry>> GetRevisionsAsync(string path, int revLimit);

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
        Task<Entry> RestoreAsync(string path, string revision);

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
        Task<IList<Entry>> SearchAsync(string path, string query);

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
        Task<IList<Entry>> SearchAsync(string path, string query, int fileLimit, bool includeDeleted);

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
        Task<DropboxLink> GetShareableLinkAsync(string path);

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
        Task<DropboxLink> GetMediaLinkAsync(string path);

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
        Task<DropboxFile> DownloadThumbnailAsync(string path, ThumbnailFormat format, ThumbnailSize size);
#else
#if !SILVERLIGHT
        /// <summary>
        /// Retrieves the authenticated user's Dropbox profile details.
        /// </summary>
        /// <returns>A <see cref="DropboxProfile"/> object representing the user's profile.</returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        DropboxProfile GetUserProfile();

        /// <summary>
        /// Creates a folder.
        /// </summary>
        /// <param name="path">The Dropbox path to the new folder to create relative to root.</param>
        /// <returns>
        /// A metadata <see cref="Entry"/> for the new folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        Entry CreateFolder(string path);

        /// <summary>
        /// Deletes a file or folder.
        /// </summary>
        /// <param name="path">The Dropbox path to the file or folder to be deleted relative to root.</param>
        /// <returns>
        /// A metadata <see cref="Entry"/> for the deleted file or folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        Entry Delete(string path);

        /// <summary>
        /// Moves a file or folder to a new location.
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to be copied from, relative to root.</param>
        /// <param name="toPath">The destination path, including the new name for the file or folder, relative to root.</param>
        /// <returns>
        /// A metadata <see cref="Entry"/> for the moved file or folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        Entry Move(string fromPath, string toPath);

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
        FileRef CreateFileRef(string path);

        /// <summary>
        /// Copies a file or folder to a new location.
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to be copied from, relative to root.</param>
        /// <param name="toPath">The destination path, including the new name for the file or folder, relative to root.</param>
        /// <returns>
        /// A metadata <see cref="Entry"/> for the moved file or folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        Entry Copy(string fromPath, string toPath);

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
        Entry CopyFileRef(string fromFileRef, string toPath);

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
        Entry UploadFile(IResource file, string path);

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
        /// The revision of the file you're editing or null if this is a new upload. 
        /// If <paramref name="revision"/> matches the latest version of the file on the user's Dropbox, that file will be replaced.
        /// </param>
        /// <returns>A metadata <see cref="Entry"/> for the uploaded file.</returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        Entry UploadFile(IResource file, string path, bool overwrite, string revision);

        /// <summary>
        /// Downloads a file and its metadata.
        /// </summary>
        /// <param name="path">The Dropbox path to the file you want to retrieve, relative to root.</param>
        /// <returns>
        /// A <see cref="DropboxFile"/> object containing the file's content and metadata.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        DropboxFile DownloadFile(string path);

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
        DropboxFile DownloadFile(string path, string revision);

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
        DropboxFile DownloadPartialFile(string path, long startOffset, long length);

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
        DropboxFile DownloadPartialFile(string path, long startOffset, long length, string revision);

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
        DeltaPage Delta(string cursor);

        /// <summary>
        /// Retrieves file or folder metadata.
        /// </summary>
        /// <param name="path">The Dropbox path to the file or folder, relative to root.</param>
        /// <returns>
        /// A metadata <see cref="Entry"/> for the file or folder.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        Entry GetMetadata(string path);

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
        Entry GetMetadata(string path, MetadataParameters parameters);

        /// <summary>
        /// Obtains metadata for the previous revisions of a file.
        /// </summary>
        /// <param name="path">The Dropbox path to the file, relative to root.</param>
        /// <returns>
        /// A list of metadata <see cref="Entry">entries</see>.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        IList<Entry> GetRevisions(string path);

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
        IList<Entry> GetRevisions(string path, int revLimit);

        /// <summary>
        /// Restores a file path to a previous revision.
        /// </summary>
        /// <param name="path">The Dropbox path to the file, relative to root.</param>
        /// <param name="revision">The revision of the file to restore.</param>
        /// <returns>
        /// A metadata <see cref="Entry"/> for the restored file. 
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        Entry Restore(string path, string revision);

        /// <summary>
        /// Searches for all files and folders that match the search query.
        /// </summary>
        /// <param name="path">The Dropbox path to the folder you want to search from, relative to root.</param>
        /// <param name="query">The search string. Must be at least three characters long.</param>
        /// <returns>
        /// A list of metadata <see cref="Entry">entries</see> for any matching files and folders.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        IList<Entry> Search(string path, string query);

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
        IList<Entry> Search(string path, string query, int fileLimit, bool includeDeleted);

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
        DropboxLink GetShareableLink(string path);

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
        DropboxLink GetMediaLink(string path);

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
        DropboxFile DownloadThumbnail(string path, ThumbnailFormat format, ThumbnailSize size);
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
        RestOperationCanceler GetUserProfileAsync(Action<RestOperationCompletedEventArgs<DropboxProfile>> operationCompleted);

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
        RestOperationCanceler CreateFolderAsync(string path, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted);

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
        RestOperationCanceler DeleteAsync(string path, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted);

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
        RestOperationCanceler MoveAsync(string fromPath, string toPath, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted);

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
        RestOperationCanceler CreateFileRefAsync(string path, Action<RestOperationCompletedEventArgs<FileRef>> operationCompleted);

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
        RestOperationCanceler CopyAsync(string fromPath, string toPath, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted);

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
        RestOperationCanceler CopyFileRefAsync(string fromFileRef, string toPath, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted);

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
        RestOperationCanceler UploadFileAsync(IResource file, string path, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted);

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
        /// <param name="operationCompleted">
        /// The 'Action&lt;&gt;' to perform when the asynchronous request completes. 
        /// Provides a metadata <see cref="Entry"/> for the uploaded file.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="DropboxApiException">If there is an error while communicating with Dropbox.</exception>
        RestOperationCanceler UploadFileAsync(IResource file, string path, bool overwrite, string revision, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted);

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
        RestOperationCanceler DownloadFileAsync(string path, Action<RestOperationCompletedEventArgs<DropboxFile>> operationCompleted);

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
        RestOperationCanceler DownloadFileAsync(string path, string revision, Action<RestOperationCompletedEventArgs<DropboxFile>> operationCompleted);

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
        RestOperationCanceler DownloadPartialFileAsync(string path, long startOffset, long length, Action<RestOperationCompletedEventArgs<DropboxFile>> operationCompleted);

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
        RestOperationCanceler DownloadPartialFileAsync(string path, long startOffset, long length, string revision, Action<RestOperationCompletedEventArgs<DropboxFile>> operationCompleted);

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
        RestOperationCanceler DeltaAsync(string cursor, Action<RestOperationCompletedEventArgs<DeltaPage>> operationCompleted);

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
        RestOperationCanceler GetMetadataAsync(string path, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted);

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
        RestOperationCanceler GetMetadataAsync(string path, MetadataParameters parameters, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted);

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
        RestOperationCanceler GetRevisionsAsync(string path, Action<RestOperationCompletedEventArgs<IList<Entry>>> operationCompleted);

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
        RestOperationCanceler GetRevisionsAsync(string path, int revLimit, Action<RestOperationCompletedEventArgs<IList<Entry>>> operationCompleted);

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
        RestOperationCanceler RestoreAsync(string path, string revision, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted);

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
        RestOperationCanceler SearchAsync(string path, string query, Action<RestOperationCompletedEventArgs<IList<Entry>>> operationCompleted);

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
        RestOperationCanceler SearchAsync(string path, string query, int fileLimit, bool includeDeleted, Action<RestOperationCompletedEventArgs<IList<Entry>>> operationCompleted);

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
        RestOperationCanceler GetShareableLinkAsync(string path, Action<RestOperationCompletedEventArgs<DropboxLink>> operationCompleted);

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
        RestOperationCanceler GetMediaLinkAsync(string path, Action<RestOperationCompletedEventArgs<DropboxLink>> operationCompleted);

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
        RestOperationCanceler DownloadThumbnailAsync(string path, ThumbnailFormat format, ThumbnailSize size, Action<RestOperationCompletedEventArgs<DropboxFile>> operationCompleted);
#endif

        /// <summary>
        /// Gets the underlying <see cref="IRestOperations"/> object allowing for consumption of Dropbox endpoints 
        /// that may not be otherwise covered by the API binding. 
        /// </summary>
        /// <remarks>
        /// The <see cref="IRestOperations"/> object returned is configured to include an OAuth "Authorization" header on all requests.
        /// </remarks>
        IRestOperations RestOperations { get; }
    }
}
