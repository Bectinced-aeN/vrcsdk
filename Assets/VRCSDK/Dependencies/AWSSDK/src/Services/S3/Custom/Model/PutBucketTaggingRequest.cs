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
using System.Xml.Serialization;
using System.Text;
using System.IO;

using Amazon.Runtime;
using Amazon.Runtime.Internal;

namespace Amazon.S3.Model
{
    /// <summary>
    /// Container for the parameters to the PutBucketTagging operation.
    /// <para>Sets the tags for a bucket.</para>
    /// </summary>
    public partial class PutBucketTaggingRequest : AmazonWebServiceRequest
    {
        private string bucketName;
        private List<Tag> tagSet = new List<Tag>();

        /// <summary>
        /// The name of the bucket to apply the tags to.
        /// </summary>
        public string BucketName
        {
            get { return this.bucketName; }
            set { this.bucketName = value; }
        }

        // Check to see if Bucket property is set
        internal bool IsSetBucketName()
        {
            return this.bucketName != null;
        }

        /// <summary>
        /// The collection of tags.
        /// </summary>
        public List<Tag> TagSet
        {
            get { return this.tagSet; }
            set { this.tagSet = value; }
        }

        // Check to see if TagSet property is set
        internal bool IsSetTagSet()
        {
            return this.tagSet.Count > 0;
        }

    }
}
    
