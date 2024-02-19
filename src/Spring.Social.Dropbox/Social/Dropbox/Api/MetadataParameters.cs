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
    /// Defines parameters for retrieving file or folder metadata.
    /// </summary>
    /// <author>Bruno Baia</author>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class MetadataParameters 
    {
        /// <summary>
        /// Gets or sets the maximum numbers of childrens to retrieve when listing a folder. Default is 10,000 if set to 0.
        /// </summary>
        public int FileLimit { get; set; }

        /// <summary>
        /// Gets or sets the metadata hash from a previous folder listing,
        /// </summary>
	    public string Hash { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the list of metadata entries for the contents of the folder is retrieved. 
        /// Default is <see langword="true"/>.
        /// </summary>
        public bool IncludeContents { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not deleted files and folders are retrieved.
        /// </summary>
        public bool IncludeDeleted { get; set; }

        /// <summary>
        /// Gets or sets the revision of the file metadata to retrieve.
        /// </summary>
        public string Revision { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="MetadataParameters"/> class.
        /// </summary>
        public MetadataParameters()
        {
            this.IncludeContents = true;
        }
    }
}
