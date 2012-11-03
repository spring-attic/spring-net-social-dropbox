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

using System.Collections.Generic;

using Spring.Json;

namespace Spring.Social.Dropbox.Api.Impl.Json
{
    /// <summary>
    /// JSON deserializer for delta pages.
    /// </summary>
    /// <author>Bruno Baia</author>
    class DeltaPageDeserializer : IJsonDeserializer
    {
        public object Deserialize(JsonValue value, JsonMapper mapper)
        {
            DeltaPage deltaPage = new DeltaPage();
            deltaPage.Cursor = value.GetValue<string>("cursor");
            deltaPage.HasMore = value.GetValue<bool>("has_more");
            deltaPage.Reset = value.GetValue<bool>("reset");
            deltaPage.Entries = new List<DeltaEntry>();
            JsonValue entriesValue = value.GetValue("entries");
            if (entriesValue != null)
            {
                foreach (JsonValue entryValue in entriesValue.GetValues())
                {
                    DeltaEntry deltaEntry = new DeltaEntry();
                    deltaEntry.Path = entryValue.GetValue<string>(0);
                    JsonValue metadataValue = entryValue.GetValue(1) as JsonObject;
                    if (metadataValue != null)
                    {
                        deltaEntry.Metadata = mapper.Deserialize<Entry>(metadataValue);
                    }
                    deltaPage.Entries.Add(deltaEntry);
                }
            }
            return deltaPage;
        }
    }
}