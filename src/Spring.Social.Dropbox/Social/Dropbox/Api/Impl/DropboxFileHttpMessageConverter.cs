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
using Spring.Http;
using Spring.Http.Converters;

using Spring.Social.Dropbox.Api.Impl.Json;

namespace Spring.Social.Dropbox.Api.Impl
{
    /// <summary>
    /// Implementation of <see cref="IHttpMessageConverter"/> that can read <see cref="DropboxFile">Dropbox file's content and metadata</see>.
    /// <para/>
    /// Reads file's content from message body and file's metadata from HTTP header 'x-dropbox-metadata'.
    /// </summary>
    /// <author>Bruno Baia</author>
    class DropboxFileHttpMessageConverter : ByteArrayHttpMessageConverter
    {
        private JsonMapper jsonMapper;

        /// <summary>
        /// Creates a new instance of the <see cref="DropboxFileHttpMessageConverter"/>.
        /// </summary>
        /// <param name="jsonMapper">A <see cref="JsonMapper"/> to use for converting custom types.</param>
        public DropboxFileHttpMessageConverter(JsonMapper jsonMapper) :
            base()
        {
            this.jsonMapper = jsonMapper;
	    }

        /// <summary>
        /// Indicates whether the given class is supported by this converter.
        /// </summary>
        /// <param name="type">The type to test for support.</param>
        /// <returns><see langword="true"/> if supported; otherwise <see langword="false"/></returns>
        protected override bool Supports(Type type)
        {
            return type.Equals(typeof(DropboxFile));
        }

        /// <summary>
        /// Abstract template method that reads the actualy object. Invoked from <see cref="M:Read"/>.
        /// </summary>
        /// <typeparam name="T">The type of object to return.</typeparam>
        /// <param name="message">The HTTP message to read from.</param>
        /// <returns>The converted object.</returns>
        /// <exception cref="HttpMessageNotReadableException">In case of conversion errors</exception>
        protected override T ReadInternal<T>(IHttpInputMessage message)
        {
            DropboxFile file = new DropboxFile();

            // Read file's content as an array of bytes
            file.Content = base.ReadInternal<byte[]>(message);

            // Read metadata content from 'x-dropbox-metadata' header
            string metadataHeaderContent = message.Headers["x-dropbox-metadata"];
            if (metadataHeaderContent != null)
            {
                JsonValue metadataJsonValue;
                if (JsonValue.TryParse(metadataHeaderContent, out metadataJsonValue))
                {
                    file.Metadata = this.jsonMapper.Deserialize<Entry>(metadataJsonValue);
                }
            }

            return file as T;
        }

        /// <summary>
        /// Abstract template method that writes the actual body. Invoked from <see cref="M:Write"/>.
        /// </summary>
        /// <param name="content">The object to write to the HTTP message.</param>
        /// <param name="message">The HTTP message to write to.</param>
        /// <exception cref="HttpMessageNotWritableException">In case of conversion errors</exception>
        protected override void WriteInternal(object content, IHttpOutputMessage message)
        {
            throw new NotSupportedException();
        }
    }
}