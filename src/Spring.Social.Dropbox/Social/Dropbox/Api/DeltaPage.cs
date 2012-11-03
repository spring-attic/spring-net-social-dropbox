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

namespace Spring.Social.Dropbox.Api
{
    /// <summary>
    /// Represents a page of delta entries returned by 'Delta(string cursor)' method.
    /// </summary>
    /// <author>Bruno Baia</author>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class DeltaPage
    {
        /// <summary>
        /// Gets or sets a value that encodes the latest information that has been returned.
        /// It is used to keep track of your current state. 
        /// <para/>
        /// On the next call to 'Delta(string cursor)' method, pass in this value 
        /// to return delta entries that have been recorded since this cursor was returned.
        /// </summary>
	    public string Cursor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not more entries are available.
        /// <para/>
        /// If <see langword="true"/> you can call 'Delta(string cursor)' method again immediately to retrieve those entries. 
        /// If <see langword="false"/>, then wait for at least five minutes (preferably longer) before checking again.
        /// </summary>
        public bool HasMore { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to clear your local state before processing the delta entries.
        /// <para/>
        /// Always <see langword="true"/> on the initial call to 'Delta(string cursor)' method (i.e. when no cursor is passed in). 
        /// Otherwise, it is <see langword="true"/> in rare situations, such as after server or account maintenance, 
        /// or if a user deletes their app folder.
        /// </summary>
        public bool Reset { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="DeltaEntry">delta entries</see>.
        /// <para/>
        /// Apply these entries to your local state to catch up with the Dropbox server's state.
        /// </summary>
        public IList<DeltaEntry> Entries { get; set; }
    }
}
