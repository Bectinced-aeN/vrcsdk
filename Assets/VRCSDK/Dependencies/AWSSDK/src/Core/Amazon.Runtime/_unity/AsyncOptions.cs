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
namespace Amazon.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    public class AsyncOptions
    {
        public AsyncOptions()
        {
            this.ExecuteCallbackOnMainThread = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ExecuteCallbackOnMainThread { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public object State { get; set; }
    }
}
