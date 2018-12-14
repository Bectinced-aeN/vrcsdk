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

using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// List Multipart Uploads Request Marshaller
    /// </summary>       
    public class ListMultipartUploadsRequestMarshaller : IMarshaller<IRequest, ListMultipartUploadsRequest> ,IMarshaller<IRequest,Amazon.Runtime.AmazonWebServiceRequest>
	{
		public IRequest Marshall(Amazon.Runtime.AmazonWebServiceRequest input)
		{
			return this.Marshall((ListMultipartUploadsRequest)input);
		}

        public IRequest Marshall(ListMultipartUploadsRequest listMultipartUploadsRequest)
        {
            IRequest request = new DefaultRequest(listMultipartUploadsRequest, "AmazonS3");

            request.HttpMethod = "GET";

            request.ResourcePath = string.Concat("/", S3Transforms.ToStringValue(listMultipartUploadsRequest.BucketName));

            request.AddSubResource("uploads");

            if (listMultipartUploadsRequest.IsSetDelimiter())
                request.Parameters.Add("delimiter", S3Transforms.ToStringValue(listMultipartUploadsRequest.Delimiter));
            if (listMultipartUploadsRequest.IsSetKeyMarker())
                request.Parameters.Add("key-marker", S3Transforms.ToStringValue(listMultipartUploadsRequest.KeyMarker));
            if (listMultipartUploadsRequest.IsSetMaxUploads())
                request.Parameters.Add("max-uploads", S3Transforms.ToStringValue(listMultipartUploadsRequest.MaxUploads));
            if (listMultipartUploadsRequest.IsSetPrefix())
                request.Parameters.Add("prefix", S3Transforms.ToStringValue(listMultipartUploadsRequest.Prefix));
            if (listMultipartUploadsRequest.IsSetUploadIdMarker())
                request.Parameters.Add("upload-id-marker", S3Transforms.ToStringValue(listMultipartUploadsRequest.UploadIdMarker));
            if (listMultipartUploadsRequest.IsSetEncoding())
                request.Parameters.Add("encoding-type", S3Transforms.ToStringValue(listMultipartUploadsRequest.Encoding));

            request.UseQueryString = true;
            
            return request;
        }
    }
}
    
