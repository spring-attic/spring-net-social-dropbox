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
using System.Globalization;

using Spring.Json;

namespace Spring.Social.Dropbox.Api.Impl.Json
{
    // Create method extension?
    // (Requires ref to System.Core.dll and specific ExtensionAttribute for .NET 2.0)

    /// <summary>
    /// Utility methods for generating and parsing JSON strings.
    /// </summary>
    /// <author>Bruno Baia</author>
    static class JsonUtils
    {
        private const string DROPBOX_DATE_FORMAT = "ddd, dd MMM yyyy HH:mm:ss zzz"; // Sat, 21 Aug 2010 22:31:20 +0000

        public static DateTime? ToDropboxDateTime(string date)
        {
            DateTime dateTime;
            if (DateTime.TryParseExact(date, DROPBOX_DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                return new DateTime?(dateTime);
            }
            return null;
        }
    }
}