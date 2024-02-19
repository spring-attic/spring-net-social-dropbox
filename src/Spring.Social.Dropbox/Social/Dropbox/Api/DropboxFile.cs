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
    /// Represents a Dropbox downloaded file with content and metadata information.
    /// </summary>
    /// <author>Bruno Baia</author>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class DropboxFile
    {
        /// <summary>
        /// Gets or sets the metadata information about the file.
        /// </summary>
	    public Entry Metadata { get; set; }

        /// <summary>
        /// Gets or sets the array of bytes containing the file's content.
        /// </summary>
        public byte[] Content { get; set; }
    }
}
