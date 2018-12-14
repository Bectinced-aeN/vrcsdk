//
// Copyright 2014-2015 Amazon.com, 
// Inc. or its affiliates. All Rights Reserved.
// 
// Licensed under the Amazon Software License (the "License"). 
// You may not use this file except in compliance with the 
// License. A copy of the License is located at
// 
//     http://aws.amazon.com/asl/
// 
// or in the "license" file accompanying this file. This file is 
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, express or implied. See the License 
// for the specific language governing permissions and 
// limitations under the License.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Util;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
    public class S3UnmarshallerContext : XmlUnmarshallerContext
    {
        bool _checkedForErrorResponse;

        /// <summary>
        /// Wrap an XmlTextReader with state for event-based parsing of an XML stream.
        /// </summary>
        /// <param name="responseStream"><c>Stream</c> with the XML from a service response.</param>
        /// <param name="maintainResponseBody"> If set to true, maintains a copy of the complete response body as the stream is being read.</param>
        /// <param name="responseData">Response data coming back from the request</param>
        public S3UnmarshallerContext(Stream responseStream, bool maintainResponseBody, IWebResponseData responseData)
            : base(responseStream, maintainResponseBody, responseData)
        {
        }

        /// <summary>
        /// Reads to the next node in the XML document, and updates the context accordingly.
        /// If node is RequestId, reads the contents and stores in RequestId property.
        /// </summary>
        /// <returns>
        /// True if a node was read, false if there are no more elements to read./
        /// </returns>
        public override bool Read()
        {
            bool result = base.Read();
            if (this.ResponseData.StatusCode == System.Net.HttpStatusCode.OK &&
                !_checkedForErrorResponse)
            {
                // Check for top level XML element "Error".
                // Few S3 operations like CopyObject, CopyPart and CompleteMultipartUpload
                // can return an HTTP 200 response with an error element.
                if (this.IsStartElement)
                {
                    if (this.TestExpression("Error", 1))
                    {
                        var errorResponse = new Amazon.S3.Model.Internal.MarshallTransformations.S3ErrorResponseUnmarshaller().Unmarshall(this);

                        throw new Amazon.S3.AmazonS3Exception(
                            errorResponse.Message, null, errorResponse.Type, errorResponse.Code,
                            errorResponse.RequestId, this.ResponseData.StatusCode, errorResponse.Id2);
                    }
                    _checkedForErrorResponse = true;
                }
            }
            return result;
        }
    }
}
