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

using Spring.Util;

using NUnit.Framework;

namespace Spring.Social.Dropbox.Api
{
    /// <summary>
    /// Unit tests for the DropboxApiException class.
    /// </summary>
    /// <author>Bruno Baia</author>
    [TestFixture]
    public class DropboxApiExceptionTests
    {
        [Test]
        public void BinarySerialization()
        {
            string message = "Error message";
            DropboxApiError error = DropboxApiError.OperationNotPermitted;

            DropboxApiException exBefore = new DropboxApiException(message, error);

            DropboxApiException exAfter = SerializationTestUtils.BinarySerializeAndDeserialize(exBefore) as DropboxApiException;

            Assert.IsNotNull(exAfter);
            Assert.AreEqual(message, exAfter.Message, "Invalid message");
            Assert.AreEqual(error, exAfter.Error, "Invalid error");
        }
    }
}
