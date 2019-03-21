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

using Spring.Json;

namespace Spring.Social.Dropbox.Api.Impl.Json
{
    /// <summary>
    /// JSON deserializer for Dropbox metadata entry.
    /// </summary>
    /// <author>Bruno Baia</author>
    class EntryDeserializer : IJsonDeserializer
    {
        public object Deserialize(JsonValue value, JsonMapper mapper)
        {
            return new Entry()
            {
                Bytes = value.GetValue<long>("bytes"),
                Hash = value.ContainsName("hash") ? value.GetValue<string>("hash") : null,
                Icon = value.GetValue<string>("icon"),
                IsDeleted = value.ContainsName("is_deleted") ? value.GetValue<bool>("is_deleted") : false,
                IsDirectory = value.GetValue<bool>("is_dir"),
                MimeType = value.ContainsName("mime_type") ? value.GetValue<string>("mime_type") : null,
                ModifiedDate = value.ContainsName("modified") ? JsonUtils.ToDropboxDateTime(value.GetValue<string>("modified")) : null,
                Path = value.GetValue<string>("path"),
                Revision = value.ContainsName("rev") ? value.GetValue<string>("rev") : null,
                Root = value.GetValue<string>("root"),
                Size = value.GetValue<string>("size"),
                ThumbExists = value.GetValue<bool>("thumb_exists"),
                Contents = value.ContainsName("contents") ? mapper.Deserialize<IList<Entry>>(value.GetValue("contents")) : null
            };
        }
    }
}