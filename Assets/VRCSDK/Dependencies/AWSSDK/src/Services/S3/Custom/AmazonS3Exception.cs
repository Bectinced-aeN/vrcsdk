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
using System.Net;
using System.Text;

using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3
{
    public class AmazonS3Exception : AmazonServiceException
    {

        public AmazonS3Exception()
            : base(ResponseUnmarshaller.GetDefaultErrorMessage<AmazonS3Exception>())
        {
        }

        public AmazonS3Exception(string message)
            : base(message)
        {
        }

        public AmazonS3Exception(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public AmazonS3Exception(Exception innerException)
            : base(innerException.Message, innerException)
        {
        }

        public AmazonS3Exception(string message, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, errorType, errorCode, requestId, statusCode)
        {
        }

        public AmazonS3Exception(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode)
            : base(message, innerException, errorType, errorCode, requestId, statusCode)
        {
        }

        public AmazonS3Exception(string message, Exception innerException, ErrorType errorType, string errorCode, string requestId, HttpStatusCode statusCode, string amazonId2)
            : base(message, innerException, errorType, errorCode, requestId, statusCode)
        {
            this.AmazonId2 = amazonId2;
        }

        /// <summary>
        /// A special token that helps AWS troubleshoot problems.
        /// </summary>
        public string AmazonId2 { get; protected set; }

        /// <summary>
        /// The entire response body for this exception, if available.
        /// </summary>
        public string ResponseBody { get; internal set; }

        #region Overrides

        public override string Message
        {
            get
            {
                if (string.IsNullOrEmpty(ResponseBody))
                    return base.Message;
                else
                    return base.Message + " " + "Response Body: " + ResponseBody;
            }
        }

        #endregion
    }
}
