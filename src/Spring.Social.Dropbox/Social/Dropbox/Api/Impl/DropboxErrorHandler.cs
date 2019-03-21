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
using System.Net;
using System.Text;

using Spring.Json;
using Spring.Http;
using Spring.Rest.Client;
using Spring.Rest.Client.Support;

namespace Spring.Social.Dropbox.Api.Impl
{
    /// <summary>
    /// Implementation of the <see cref="IResponseErrorHandler"/> that handles errors from Dropbox's REST API, 
    /// interpreting them into appropriate exceptions.
    /// </summary>
    /// <author>Bruno Baia</author>
    class DropboxErrorHandler : DefaultResponseErrorHandler
    {
        // Default encoding for JSON
        private static readonly Encoding DEFAULT_CHARSET = new UTF8Encoding(false); // Remove byte Order Mask (BOM)

        /// <summary>
        /// Handles the error in the given response. 
        /// <para/>
        /// This method is only called when HasError() method has returned <see langword="true"/>.
        /// </summary>
        /// <remarks>
        /// This implementation throws appropriate exception if the response status code 
        /// is a client code error (4xx) or a server code error (5xx). 
        /// </remarks>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="requestMethod">The request method.</param>
        /// <param name="response">The response message with the error.</param>
        public override void HandleError(Uri requestUri, HttpMethod requestMethod, HttpResponseMessage<byte[]> response)
        {
            int type = (int)response.StatusCode / 100;
            if (type == 4)
            {
                this.HandleClientErrors(response);
            }
            else if (type == 5)
            {
                this.HandleServerErrors(response.StatusCode);
            }

            // if not otherwise handled, do default handling and wrap with DropboxApiException
            try
            {
                base.HandleError(requestUri, requestMethod, response);
            }
            catch (Exception ex)
            {
                throw new DropboxApiException("Error consuming Dropbox REST API.", ex);
            }
        }

        private void HandleClientErrors(HttpResponseMessage<byte[]> response) 
        {
		    JsonValue errorValue = this.ExtractErrorDetailsFromResponse(response);
		    if (errorValue == null) 
            {
			    return; // unexpected error body, can't be handled here
		    }
            string errorText = errorValue.ContainsName("error") 
                ? errorValue.GetValue<string>("error") 
                : response.StatusDescription;

            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    throw new DropboxApiException(errorText, DropboxApiError.BadParameter);
                case HttpStatusCode.Unauthorized:
                    throw new DropboxApiException(errorText, DropboxApiError.NotAuthorized);
                case HttpStatusCode.Forbidden:
                    throw new DropboxApiException(errorText, DropboxApiError.OperationNotPermitted);
                case HttpStatusCode.NotFound:
                    throw new DropboxApiException(errorText, DropboxApiError.PathNotFound);
                case HttpStatusCode.NotAcceptable:
                    throw new DropboxApiException(errorText, DropboxApiError.TooManyEntries);
                case HttpStatusCode.LengthRequired:
                    throw new DropboxApiException(errorText, DropboxApiError.ChunkedEncodingNotSupported);
                case HttpStatusCode.UnsupportedMediaType:
                    throw new DropboxApiException(errorText, DropboxApiError.ThumbnailNotSupported);
            }
	    }

	    private void HandleServerErrors(HttpStatusCode statusCode)
        {
            if (statusCode == HttpStatusCode.BadGateway)
            {
                throw new DropboxApiException("Dropbox is down or is being upgraded. Try again later.", DropboxApiError.ServerDown);
            }
            else if (statusCode == HttpStatusCode.ServiceUnavailable) 
            {
                throw new DropboxApiException(
                    "Dropbox is experiencing high load or the rate limit has been exceeded. Try again later.", 
                    DropboxApiError.ServerOverloadedOrRateLimitExceeded);
		    }
            else if (statusCode == (HttpStatusCode)507)
            {
                throw new DropboxApiException("The storage quota has been exceeded.", DropboxApiError.StorageQuotaExceeded);
            }
            else 
            {
                throw new DropboxApiException(
                    "Something is broken at Dropbox. Please check status page at http://status.dropbox.com/.", 
                    DropboxApiError.Server);
		    } 
	    }

        private JsonValue ExtractErrorDetailsFromResponse(HttpResponseMessage<byte[]> response) 
        {
            if (response.Body == null)
            {
                return null;
            }
            MediaType contentType = response.Headers.ContentType;
            Encoding charset = (contentType != null && contentType.CharSet != null) ? contentType.CharSet : DEFAULT_CHARSET;
            string errorDetails = charset.GetString(response.Body, 0, response.Body.Length);
		    
            JsonValue result;
            return JsonValue.TryParse(errorDetails, out result) ? result : null;
	    }
    }
}