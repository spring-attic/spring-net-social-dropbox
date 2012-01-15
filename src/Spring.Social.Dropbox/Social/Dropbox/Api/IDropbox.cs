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
#if NET_4_0 || SILVERLIGHT_5
using System.Threading.Tasks;
#endif

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

#if NET_4_0 || SILVERLIGHT_5  
        /// <summary>
        /// Asynchronously retrieves the authenticated user's Dropbox profile details.
        /// </summary>
        /// <returns>
        /// A <code>Task</code> that represents the asynchronous operation that can return 
        /// a <see cref="DropboxProfile"/> object representing the user's profile.
        /// </returns>
        /// <exception cref="ApiException">If there is an error while communicating with Dropbox.</exception>
        Task<DropboxProfile> GetUserProfileAsync();

        /// <summary>
        /// Asynchronously creates a folder.
        /// </summary>
        /// <param name="path">The path to the new folder to create relative to root.</param>
        /// <returns>
        /// A <code>Task</code> that represents the asynchronous operation that can return 
        /// a metadata <see cref="Entry"/> for the new folder.
        /// </returns>
        /// <exception cref="ApiException">If there is an error while communicating with Dropbox.</exception>
        Task<Entry> CreateFolderAsync(string path);

        /// <summary>
        /// Asynchronously deletes a file or folder.
        /// </summary>
        /// <param name="path">The path to the file or folder to be deleted relative to root.</param>
        /// <returns>
        /// A <code>Task</code> that represents the asynchronous operation that can return 
        /// a metadata <see cref="Entry"/> for the deleted file or folder.
        /// </returns>
        /// <exception cref="ApiException">If there is an error while communicating with Dropbox.</exception>
        Task<Entry> DeleteAsync(string path);

        /// <summary>
        /// Asynchronously moves a file or folder to a new location.
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to be copied from, relative to root.</param>
        /// <param name="toPath">The destination path, including the new name for the file or folder, relative to root.</param>
        /// <returns>
        /// A <code>Task</code> that represents the asynchronous operation that can return 
        /// a metadata <see cref="Entry"/> for the moved file or folder.
        /// </returns>
        /// <exception cref="ApiException">If there is an error while communicating with Dropbox.</exception>
        Task<Entry> MoveAsync(string fromPath, string toPath);

        /// <summary>
        /// Asynchronously copies a file or folder to a new location.
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to be copied from, relative to root.</param>
        /// <param name="toPath">The destination path, including the new name for the file or folder, relative to root.</param>
        /// <returns>
        /// A <code>Task</code> that represents the asynchronous operation that can return 
        /// a metadata <see cref="Entry"/> for the moved file or folder.
        /// </returns>
        /// <exception cref="ApiException">If there is an error while communicating with Dropbox.</exception>
        Task<Entry> CopyAsync(string fromPath, string toPath);
#else
#if !SILVERLIGHT
        /// <summary>
        /// Retrieves the authenticated user's Dropbox profile details.
        /// </summary>
        /// <returns>A <see cref="DropboxProfile"/> object representing the user's profile.</returns>
        /// <exception cref="ApiException">If there is an error while communicating with Dropbox.</exception>
        DropboxProfile GetUserProfile();

        /// <summary>
        /// Creates a folder.
        /// </summary>
        /// <param name="path">The path to the new folder to create relative to root.</param>
        /// <returns>
        /// A metadata <see cref="Entry"/> for the new folder.
        /// </returns>
        /// <exception cref="ApiException">If there is an error while communicating with Dropbox.</exception>
        Entry CreateFolder(string path);

        /// <summary>
        /// Deletes a file or folder.
        /// </summary>
        /// <param name="path">The path to the file or folder to be deleted relative to root.</param>
        /// <returns>
        /// A metadata <see cref="Entry"/> for the deleted file or folder.
        /// </returns>
        /// <exception cref="ApiException">If there is an error while communicating with Dropbox.</exception>
        Entry Delete(string path);

        /// <summary>
        /// Moves a file or folder to a new location.
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to be copied from, relative to root.</param>
        /// <param name="toPath">The destination path, including the new name for the file or folder, relative to root.</param>
        /// <returns>
        /// A metadata <see cref="Entry"/> for the moved file or folder.
        /// </returns>
        /// <exception cref="ApiException">If there is an error while communicating with Dropbox.</exception>
        Entry Move(string fromPath, string toPath);

        /// <summary>
        /// Copies a file or folder to a new location.
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to be copied from, relative to root.</param>
        /// <param name="toPath">The destination path, including the new name for the file or folder, relative to root.</param>
        /// <returns>
        /// A metadata <see cref="Entry"/> for the moved file or folder.
        /// </returns>
        /// <exception cref="ApiException">If there is an error while communicating with Dropbox.</exception>
        Entry Copy(string fromPath, string toPath);
#endif

        /// <summary>
        /// Asynchronously retrieves the authenticated user's Dropbox profile details.
        /// </summary>
        /// <param name="operationCompleted">
        /// The <code>Action&lt;&gt;</code> to perform when the asynchronous request completes. 
        /// Provides a <see cref="DropboxProfile"/>object representing the user's profile.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="ApiException">If there is an error while communicating with Dropbox.</exception>
        RestOperationCanceler GetUserProfileAsync(Action<RestOperationCompletedEventArgs<DropboxProfile>> operationCompleted);

        /// <summary>
        /// Asynchronously creates a folder.
        /// </summary>
        /// <param name="path">The path to the new folder to create relative to root.</param>
        /// <param name="operationCompleted">
        /// The <code>Action&lt;&gt;</code> to perform when the asynchronous request completes. 
        /// Provides a metadata <see cref="Entry"/> for the new folder.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="ApiException">If there is an error while communicating with Dropbox.</exception>
        RestOperationCanceler CreateFolderAsync(string path, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted);

        /// <summary>
        /// Asynchronously deletes a file or folder.
        /// </summary>
        /// <param name="path">The path to the file or folder to be deleted relative to root.</param>
        /// <param name="operationCompleted">
        /// The <code>Action&lt;&gt;</code> to perform when the asynchronous request completes. 
        /// Provides a metadata <see cref="Entry"/> for the deleted file or folder.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="ApiException">If there is an error while communicating with Dropbox.</exception>
        RestOperationCanceler DeleteAsync(string path, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted);

        /// <summary>
        /// Asynchronously moves a file or folder to a new location.
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to be copied from, relative to root.</param>
        /// <param name="toPath">The destination path, including the new name for the file or folder, relative to root.</param>
        /// <param name="operationCompleted">
        /// The <code>Action&lt;&gt;</code> to perform when the asynchronous request completes. 
        /// Provides a metadata <see cref="Entry"/> for the moved file or folder.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="ApiException">If there is an error while communicating with Dropbox.</exception>
        RestOperationCanceler MoveAsync(string fromPath, string toPath, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted);

        /// <summary>
        /// Asynchronously copies a file or folder to a new location.
        /// </summary>
        /// <param name="fromPath">The path to the file or folder to be copied from, relative to root.</param>
        /// <param name="toPath">The destination path, including the new name for the file or folder, relative to root.</param>
        /// <param name="operationCompleted">
        /// The <code>Action&lt;&gt;</code> to perform when the asynchronous request completes. 
        /// Provides a metadata <see cref="Entry"/> for the moved file or folder.
        /// </param>
        /// <returns>
        /// A <see cref="RestOperationCanceler"/> instance that allows to cancel the asynchronous operation.
        /// </returns>
        /// <exception cref="ApiException">If there is an error while communicating with Dropbox.</exception>
        RestOperationCanceler CopyAsync(string fromPath, string toPath, Action<RestOperationCompletedEventArgs<Entry>> operationCompleted);
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
