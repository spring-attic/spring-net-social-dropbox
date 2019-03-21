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
    /// Defines the Dropbox image sizes of thumbnails.
    /// </summary>
    /// <author>Bruno Baia</author>
    public enum ThumbnailSize
    {
        /// <summary>
        /// Fits within 32x32.
        /// </summary>
        Mini,

        /// <summary>
        /// Fits within 64x64.
        /// </summary>
        Small,

        /// <summary>
        /// Fits within 128x128.
        /// </summary>
        Medium,

        /// <summary>
        /// Fits within 640x480.
        /// </summary>
        Large,

        /// <summary>
        /// Fits within 1024x768.
        /// </summary>
        ExtraLarge
    }
}
