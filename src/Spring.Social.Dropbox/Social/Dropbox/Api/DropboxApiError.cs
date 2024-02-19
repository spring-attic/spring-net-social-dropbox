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

namespace Spring.Social.Dropbox.Api
{
    /// <summary>
    /// The <see cref="DropboxApiError"/> enumeration is used by the <see cref="DropboxApiException"/> class 
    /// to indicate what kind of error caused the exception.
    /// </summary>
    /// <author>Bruno Baia</author>
    public enum DropboxApiError
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// Bad request parameter.
        /// </summary>
        BadParameter,

        /// <summary>
        /// Bad or expired OAuth token. 
        /// </summary>
        NotAuthorized,

        /// <summary>
        /// Invalid operation attempted.
        /// </summary>
        OperationNotPermitted,

        /// <summary>
        /// File or folder path not found.
        /// </summary>
        PathNotFound,

        /// <summary>
        /// Too many metadata entries to return.
        /// </summary>
        TooManyEntries,

        /// <summary>
        /// Chunked encoding not supported.
        /// </summary>
        ChunkedEncodingNotSupported,

        /// <summary>
        /// Thumbnail cannot be created for the input file.
        /// </summary>
        ThumbnailNotSupported,

        /// <summary>
        /// Internal server error.
        /// </summary>
        Server,

        /// <summary>
        /// Server is down or is being upgraded.
        /// </summary>
        ServerDown,

        /// <summary>
        /// Server is overloaded with request or the rate limit has been exceeded.
        /// </summary>
        ServerOverloadedOrRateLimitExceeded,

        /// <summary>
        /// User is over quota.
        /// </summary>
        StorageQuotaExceeded
    }
}
