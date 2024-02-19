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
    /// Represents a Dropbox user's profile information.
    /// </summary>
    /// <author>Bruno Baia</author>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class DropboxProfile 
    {
        /// <summary>
        /// Gets or sets the user's unique Dropbox ID
        /// </summary>
	    public long ID { get; set; }

        /// <summary>
        /// Gets or sets the user's display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user's referral link.
        /// </summary>
        public string ReferralLink { get; set; }

        /// <summary>
        /// Gets or sets the user's ISO country code, if available.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the user's quota (in bytes).
        /// </summary>
        public long Quota { get; set; }

        /// <summary>
        /// Gets or sets the user's used quota outside of shared folders (in bytes).
        /// </summary>
        public long QuotaNormal { get; set; }

        /// <summary>
        /// Gets or sets the user's used quota in shared folders (in bytes).
        /// </summary>
        public long QuotaShared { get; set; }
    }
}
