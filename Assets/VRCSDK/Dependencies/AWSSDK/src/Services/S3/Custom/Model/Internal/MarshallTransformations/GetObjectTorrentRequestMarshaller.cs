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
using System.Globalization;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Get Object Torrent Request Marshaller
    /// </summary>       
    public class GetObjectTorrentRequestMarshaller : IMarshaller<IRequest, GetObjectTorrentRequest> ,IMarshaller<IRequest,Amazon.Runtime.AmazonWebServiceRequest>
	{
		public IRequest Marshall(Amazon.Runtime.AmazonWebServiceRequest input)
		{
			return this.Marshall((GetObjectTorrentRequest)input);
		}

        public IRequest Marshall(GetObjectTorrentRequest getObjectTorrentRequest)
        {
            IRequest request = new DefaultRequest(getObjectTorrentRequest, "AmazonS3");

            request.HttpMethod = "GET";

            request.ResourcePath = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}",
                                                 S3Transforms.ToStringValue(getObjectTorrentRequest.BucketName),
                                                 S3Transforms.ToStringValue(getObjectTorrentRequest.Key));

            request.AddSubResource("torrent");
            request.UseQueryString = true;
            
            return request;
        }
    }
}
    
