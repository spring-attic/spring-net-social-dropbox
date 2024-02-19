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

using Spring.Json;

namespace Spring.Social.Dropbox.Api.Impl.Json
{
    /// <summary>
    /// JSON deserializer for Dropbox user's profile.
    /// </summary>
    /// <author>Bruno Baia</author>
    class DropboxProfileDeserializer : IJsonDeserializer
    {
        public object Deserialize(JsonValue value, JsonMapper mapper)
        {
            return new DropboxProfile()
            {
                ReferralLink = value.GetValue<string>("referral_link"),
                DisplayName = value.GetValue<string>("display_name"),
                ID = value.GetValue<long>("uid"),
                Country = value.GetValue<string>("country"),
                QuotaShared = value.GetValue("quota_info").GetValue<long>("shared"),
                Quota = value.GetValue("quota_info").GetValue<long>("quota"),
                QuotaNormal = value.GetValue("quota_info").GetValue<long>("normal"),
                Email = value.GetValue<string>("email")
            };
        }
    }
}