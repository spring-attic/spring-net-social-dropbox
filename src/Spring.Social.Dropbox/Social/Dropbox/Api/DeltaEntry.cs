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

namespace Spring.Social.Dropbox.Api
{
    /// <summary>
    /// Represents a single entry in a <see cref="DeltaPage">delta page</see>.
    /// </summary>
    /// <author>Bruno Baia</author>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class DeltaEntry
    {
        /// <summary>
        /// Gets or sets the lower-cased path of the entry. 
        /// <para/>
        /// Value is lower-cased because Dropbox treats file names in a case-insensitive but case-preserving way. 
        /// The Metadata property has the original case-preserved path.
        /// </summary>
	    public string Path { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Entry">metadata information</see> for the entry to update your local state.
        /// <para/>
        /// If <see langword="null"/>, it means that this path should not exist on your Dropbox's copy of the file system.
        /// Otherwise, it means that Dropbox has a file/folder at this path with the given metadata.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If <see langword="null"/>, indicates that there is no file/folder at the given path. 
        /// To update your local state to match, anything at path and all its children should be deleted. 
        /// Deleting a folder in your Dropbox will sometimes send down a single deleted entry for that folder, 
        /// and sometimes separate entries for the folder and all child paths. 
        /// If your local state doesn't have anything at path, ignore this entry.
        /// </para>
        /// <para>
        /// If not <see langword="null"/>, indicates that there is a file/folder at the given path.
        /// You should add the entry to your local path. To correctly process delta entries:
        /// <list type="bullet">
        /// <item>
        /// If the new entry includes parent folders that don't yet exist in your local state, 
        /// create those parent folders in your local state.
        /// </item>
        /// <item>
        /// If the new entry is a file, replace whatever your local state has at path with the new entry.
        /// </item>
        /// <item>
        /// If the new entry is a folder, check what your local state has at path. 
        /// If it's a file, replace it with the new entry. 
        /// If it's a folder, apply the new entry to the folder, but do not modify the folder's children.
        /// </item>
        /// </list>
        /// </para>
        /// </remarks>
        public Entry Metadata { get; set; }
    }
}
