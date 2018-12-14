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

using System.IO;
using System.Xml;
using System.Text;
using Amazon.S3.Util;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Put Bucket Notification Request Marshaller
    /// </summary>       
    public class PutBucketNotificationRequestMarshaller : IMarshaller<IRequest, PutBucketNotificationRequest> ,IMarshaller<IRequest,Amazon.Runtime.AmazonWebServiceRequest>
	{
		public IRequest Marshall(Amazon.Runtime.AmazonWebServiceRequest input)
		{
			return this.Marshall((PutBucketNotificationRequest)input);
		}

        public IRequest Marshall(PutBucketNotificationRequest putBucketNotificationRequest)
        {
            IRequest request = new DefaultRequest(putBucketNotificationRequest, "AmazonS3");

            request.HttpMethod = "PUT";

            request.ResourcePath = string.Concat("/", S3Transforms.ToStringValue(putBucketNotificationRequest.BucketName));

            request.AddSubResource("notification");

            var stringWriter = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);
            using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings() { Encoding = System.Text.Encoding.UTF8, OmitXmlDeclaration = true }))
            {
                xmlWriter.WriteStartElement("NotificationConfiguration", "");

                if (putBucketNotificationRequest.IsSetTopicConfigurations())
                {
                    foreach (var topicConfiguartion in putBucketNotificationRequest.TopicConfigurations)
                    {
                        if (topicConfiguartion != null)
                        {
                            xmlWriter.WriteStartElement("TopicConfiguration", "");
                            if (topicConfiguartion.IsSetId())
                            {
                                xmlWriter.WriteElementString("Id", "", S3Transforms.ToXmlStringValue(topicConfiguartion.Id));
                            }
                            if (topicConfiguartion.IsSetEvents())
                            {
                                foreach(var evnt in topicConfiguartion.Events)
                                {
                                    xmlWriter.WriteElementString("Event", "", S3Transforms.ToXmlStringValue(evnt));
                                }
                            }
                            if (topicConfiguartion.IsSetTopic())
                            {
                                xmlWriter.WriteElementString("Topic", "", S3Transforms.ToXmlStringValue(topicConfiguartion.Topic));
                            }
                            xmlWriter.WriteEndElement();
                        }
                    }
                }

                if (putBucketNotificationRequest.IsSetQueueConfigurations())
                {
                    foreach (var queueConfiguartion in putBucketNotificationRequest.QueueConfigurations)
                    {
                        if (queueConfiguartion != null)
                        {
                            xmlWriter.WriteStartElement("QueueConfiguration", "");
                            if (queueConfiguartion.IsSetId())
                            {
                                xmlWriter.WriteElementString("Id", "", S3Transforms.ToXmlStringValue(queueConfiguartion.Id));
                            }
                            if (queueConfiguartion.IsSetEvents())
                            {
                                foreach (var evnt in queueConfiguartion.Events)
                                {
                                    xmlWriter.WriteElementString("Event", "", S3Transforms.ToXmlStringValue(evnt));
                                }
                            }
                            if (queueConfiguartion.IsSetQueue())
                            {
                                xmlWriter.WriteElementString("Queue", "", S3Transforms.ToXmlStringValue(queueConfiguartion.Queue));
                            }
                            xmlWriter.WriteEndElement();
                        }
                    }
                }

                if (putBucketNotificationRequest.IsSetCloudFunctionConfigurations())
                {
                    foreach (var cloudFunctionConfiguartion in putBucketNotificationRequest.CloudFunctionConfigurations)
                    {
                        if (cloudFunctionConfiguartion != null)
                        {
                            xmlWriter.WriteStartElement("CloudFunctionConfiguration", "");
                            if (cloudFunctionConfiguartion.IsSetId())
                            {
                                xmlWriter.WriteElementString("Id", "", S3Transforms.ToXmlStringValue(cloudFunctionConfiguartion.Id));
                            }
                            if (cloudFunctionConfiguartion.IsSetEvents())
                            {
                                foreach (var evnt in cloudFunctionConfiguartion.Events)
                                {
                                    xmlWriter.WriteElementString("Event", "", S3Transforms.ToXmlStringValue(evnt));
                                }
                            }
                            if (cloudFunctionConfiguartion.IsSetCloudFunction())
                            {
                                xmlWriter.WriteElementString("CloudFunction", "", S3Transforms.ToXmlStringValue(cloudFunctionConfiguartion.CloudFunction));
                            }
                            if (cloudFunctionConfiguartion.IsSetInvocationRole())
                            {
                                xmlWriter.WriteElementString("InvocationRole", "", S3Transforms.ToXmlStringValue(cloudFunctionConfiguartion.InvocationRole));
                            }
                            xmlWriter.WriteEndElement();
                        }
                    }
                }

                if (putBucketNotificationRequest.IsSetLambdaFunctionConfigurations())
                {
                    foreach (var lambdaFunctionConfiguartion in putBucketNotificationRequest.LambdaFunctionConfigurations)
                    {
                        if (lambdaFunctionConfiguartion != null)
                        {
                            xmlWriter.WriteStartElement("CloudFunctionConfiguration", "");
                            if (lambdaFunctionConfiguartion.IsSetId())
                            {
                                xmlWriter.WriteElementString("Id", "", S3Transforms.ToXmlStringValue(lambdaFunctionConfiguartion.Id));
                            }
                            if (lambdaFunctionConfiguartion.IsSetEvents())
                            {
                                foreach (var evnt in lambdaFunctionConfiguartion.Events)
                                {
                                    xmlWriter.WriteElementString("Event", "", S3Transforms.ToXmlStringValue(evnt));
                                }
                            }
                            if (lambdaFunctionConfiguartion.IsSetFunctionArn())
                            {
                                xmlWriter.WriteElementString("CloudFunction", "", S3Transforms.ToXmlStringValue(lambdaFunctionConfiguartion.FunctionArn));
                            }
                            xmlWriter.WriteEndElement();
                        }
                    }
                }

                xmlWriter.WriteEndElement();
            }
    
            try 
            {
                var content = stringWriter.ToString();
                request.Content = System.Text.Encoding.UTF8.GetBytes(content);
                request.Headers[HeaderKeys.ContentTypeHeader] = "application/xml";
                
                var checksum = AmazonS3Util.GenerateChecksumForContent(content, true);
                request.Headers[HeaderKeys.ContentMD5Header] = checksum;
                
            } 
            catch (EncoderFallbackException e) 
            {
                throw new AmazonServiceException("Unable to marshall request to XML", e);
            }
        
            return request;
        }
    }
}
    
