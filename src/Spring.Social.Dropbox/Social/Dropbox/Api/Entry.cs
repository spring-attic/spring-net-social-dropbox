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
using System.Collections.Generic;

namespace Spring.Social.Dropbox.Api
{
    /// <summary>
    /// Represents metadata information about a file or folder.
    /// </summary>
    /// <author>Bruno Baia</author>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class Entry 
    {
        /// <summary>
        /// Gets or sets a human-readable description of the file size (translated by locale).
        /// </summary>
	    public string Size { get; set; }

        /// <summary>
        /// Gets or sets the file size in bytes.
        /// </summary>
        public long Bytes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the given entry is a folder or not.
        /// </summary>
        public bool IsDirectory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the given entry is deleted 
        /// (only included if deleted files are being returned).
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets a unique identifier for the current revision of a file. 
        /// <para/>
        /// This field can be used to detect changes and avoid conflicts.
        /// </summary>
        public string Revision { get; set; }

        /// <summary>
        /// Gets or sets the hash of a folder's metadata useful in later calls to GetMetadata(). May be <see langword="null"/>.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the file is an image 
        /// and can be converted to a thumbnail via the GetThumbnails() method.
        /// </summary>
        public bool ThumbExists { get; set; }

        /// <summary>
        /// Gets or sets the name of the icon used to illustrate the file type in Dropbox's icon library.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the last time the file was modified on Dropbox, 
        /// in the standard date format (<see langword="null"/> for the root folder).
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the root or top-level folder depending on your access level (Folder or Full access). 
        /// <para/>
        /// All paths returned are relative to this root level. Permitted values are either 'dropbox' or 'app_folder'.
        /// </summary>
        public string Root { get; set; }

        /// <summary>
        /// Gets or sets the file's MIME type. May be <see langword="null"/>.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the file or folder's path relative to root.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the list of children entries if this is a directory. May be <see langword="null"/>.
        /// </summary>
        public IList<Entry> Contents { get; set; }
    }
}
