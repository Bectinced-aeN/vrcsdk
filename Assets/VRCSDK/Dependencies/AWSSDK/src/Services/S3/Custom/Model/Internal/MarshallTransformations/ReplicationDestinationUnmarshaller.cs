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

using System.Collections.Generic;

using Amazon.S3.Model;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
    internal class ReplicationDestinationUnmarshaller : IUnmarshaller<ReplicationDestination, XmlUnmarshallerContext>, IUnmarshaller<ReplicationDestination, JsonUnmarshallerContext>
    {

        public ReplicationDestination Unmarshall(XmlUnmarshallerContext context)
        {
            ReplicationDestination destination = new ReplicationDestination();
            int originalDepth = context.CurrentDepth;
            int targetDepth = originalDepth + 1;

            if (context.IsStartOfDocument)
                targetDepth += 2;

            while (context.Read())
            {
                if (context.IsStartElement || context.IsAttribute)
                {
                    if (context.TestExpression("Bucket", targetDepth))
                    {
                        destination.BucketArn = StringUnmarshaller.GetInstance().Unmarshall(context);

                        continue;
                    }
                }
                else if (context.IsEndElement && context.CurrentDepth < originalDepth)
                {
                    return destination;
                }
            }

            return destination;
        }

        public ReplicationDestination Unmarshall(JsonUnmarshallerContext context)
        {
            return null;
        }

        private static ReplicationDestinationUnmarshaller _instance;

        public static ReplicationDestinationUnmarshaller Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ReplicationDestinationUnmarshaller();
                }
                return _instance;
            }
        }
    }
}
