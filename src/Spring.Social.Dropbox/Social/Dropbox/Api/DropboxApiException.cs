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
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Spring.Social.Dropbox.Api
{
    /// <summary>
    /// The exception that is thrown when a error occurs while consuming Dropbox REST API.
    /// </summary>
    /// <author>Bruno Baia</author>
#if !SILVERLIGHT
    [Serializable]
#endif
    public class DropboxApiException : SocialException
    {
        private DropboxApiError error;

        /// <summary>
        /// Gets the Dropbox error.
        /// </summary>
        public DropboxApiError Error
        {
            get { return this.error; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DropboxApiException"/> class.
        /// </summary>
        /// <param name="message">A message about the exception.</param>
        /// <param name="error">The Dropbox error.</param>
        public DropboxApiException(string message, DropboxApiError error)
            : base(message)
        {
            this.error = error;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DropboxApiException"/> class.
        /// </summary>
        /// <param name="message">A message about the exception.</param>
        /// <param name="innerException">The inner exception that is the cause of the current exception.</param>
        public DropboxApiException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.error = DropboxApiError.Unknown;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Creates a new instance of the <see cref="DropboxApiException"/> class.
        /// </summary>
        /// <param name="info">
        /// The <see cref="System.Runtime.Serialization.SerializationInfo"/>
        /// that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="System.Runtime.Serialization.StreamingContext"/>
        /// that contains contextual information about the source or destination.
        /// </param>
        protected DropboxApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info != null)
            {
                this.error = (DropboxApiError)info.GetValue("Error", typeof(DropboxApiError));
            }
        }

        /// <summary>
        /// Populates the <see cref="System.Runtime.Serialization.SerializationInfo"/> with 
        /// information about the exception.
        /// </summary>
        /// <param name="info">
        /// The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds 
        /// the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual 
        /// information about the source or destination.
        /// </param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            if (info != null)
            {
                info.AddValue("Error", this.error);
            }
        }
#endif
    }
}
