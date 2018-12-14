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
    /// Put Bucket Lifecycle Request Marshaller
    /// </summary>       
    public class PutLifecycleConfigurationRequestMarshaller : IMarshaller<IRequest, PutLifecycleConfigurationRequest> ,IMarshaller<IRequest,Amazon.Runtime.AmazonWebServiceRequest>
	{
		public IRequest Marshall(Amazon.Runtime.AmazonWebServiceRequest input)
		{
			return this.Marshall((PutLifecycleConfigurationRequest)input);
		}

        public IRequest Marshall(PutLifecycleConfigurationRequest putLifecycleConfigurationRequest)
        {
            IRequest request = new DefaultRequest(putLifecycleConfigurationRequest, "AmazonS3");

            request.HttpMethod = "PUT";

            request.ResourcePath = string.Concat("/", S3Transforms.ToStringValue(putLifecycleConfigurationRequest.BucketName));

            request.AddSubResource("lifecycle");

            var stringWriter = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);
            using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings(){Encoding = Encoding.UTF8, OmitXmlDeclaration = true}))
            {
                var lifecycleConfigurationLifecycleConfiguration = putLifecycleConfigurationRequest.Configuration;
                if (lifecycleConfigurationLifecycleConfiguration != null)
                {
                    xmlWriter.WriteStartElement("LifecycleConfiguration", "");

                    if (lifecycleConfigurationLifecycleConfiguration != null)
                    {
                        var lifecycleConfigurationLifecycleConfigurationrulesList = lifecycleConfigurationLifecycleConfiguration.Rules;
                        if (lifecycleConfigurationLifecycleConfigurationrulesList != null && lifecycleConfigurationLifecycleConfigurationrulesList.Count > 0)
                        {
                            foreach (var lifecycleConfigurationLifecycleConfigurationrulesListValue in lifecycleConfigurationLifecycleConfigurationrulesList)
                            {
                                xmlWriter.WriteStartElement("Rule", "");
                                if (lifecycleConfigurationLifecycleConfigurationrulesListValue != null)
                                {
                                    var expiration = lifecycleConfigurationLifecycleConfigurationrulesListValue.Expiration;
                                    if (expiration != null)
                                    {
                                        xmlWriter.WriteStartElement("Expiration", "");
                                        if (expiration.IsSetDate())
                                        {
                                            xmlWriter.WriteElementString("Date", "", S3Transforms.ToXmlStringValue(expiration.Date));
                                        }
                                        if (expiration.IsSetDays())
                                        {
                                            xmlWriter.WriteElementString("Days", "", S3Transforms.ToXmlStringValue(expiration.Days));
                                        }
                                        xmlWriter.WriteEndElement();
                                    }

                                    var transition = lifecycleConfigurationLifecycleConfigurationrulesListValue.Transition;
                                    if (transition != null)
                                    {
                                        xmlWriter.WriteStartElement("Transition", "");
                                        if (transition.IsSetDate())
                                        {
                                            xmlWriter.WriteElementString("Date", "", S3Transforms.ToXmlStringValue(transition.Date));
                                        }
                                        if (transition.IsSetDays())
                                        {
                                            xmlWriter.WriteElementString("Days", "", S3Transforms.ToXmlStringValue(transition.Days));
                                        }
                                        if (transition.IsSetStorageClass())
                                        {
                                            xmlWriter.WriteElementString("StorageClass", "", S3Transforms.ToXmlStringValue(transition.StorageClass));
                                        }
                                        xmlWriter.WriteEndElement();
                                    }

                                    var noncurrentVersionExpiration = lifecycleConfigurationLifecycleConfigurationrulesListValue.NoncurrentVersionExpiration;
                                    if (noncurrentVersionExpiration != null)
                                    {
                                        xmlWriter.WriteStartElement("NoncurrentVersionExpiration", "");
                                        if (noncurrentVersionExpiration.IsSetNoncurrentDays())
                                        {
                                            xmlWriter.WriteElementString("NoncurrentDays", "", S3Transforms.ToXmlStringValue(noncurrentVersionExpiration.NoncurrentDays));
                                        }
                                        xmlWriter.WriteEndElement();
                                    }

                                    var noncurrentVersionTransition = lifecycleConfigurationLifecycleConfigurationrulesListValue.NoncurrentVersionTransition;
                                    if (noncurrentVersionTransition != null)
                                    {
                                        xmlWriter.WriteStartElement("NoncurrentVersionTransition", "");
                                        if (noncurrentVersionTransition.IsSetNoncurrentDays())
                                        {
                                            xmlWriter.WriteElementString("NoncurrentDays", "", S3Transforms.ToXmlStringValue(noncurrentVersionTransition.NoncurrentDays));
                                        }
                                        if (noncurrentVersionTransition.IsSetStorageClass())
                                        {
                                            xmlWriter.WriteElementString("StorageClass", "", S3Transforms.ToXmlStringValue(noncurrentVersionTransition.StorageClass));
                                        }
                                        xmlWriter.WriteEndElement();
                                    }
                                }
                                if (lifecycleConfigurationLifecycleConfigurationrulesListValue.IsSetId())
                                {
                                    xmlWriter.WriteElementString("ID", "", S3Transforms.ToXmlStringValue(lifecycleConfigurationLifecycleConfigurationrulesListValue.Id));
                                }
                                if (lifecycleConfigurationLifecycleConfigurationrulesListValue.IsSetPrefix())
                                {
                                    xmlWriter.WriteElementString("Prefix", "", S3Transforms.ToXmlStringValue(lifecycleConfigurationLifecycleConfigurationrulesListValue.Prefix));
                                }
                                if (lifecycleConfigurationLifecycleConfigurationrulesListValue.IsSetStatus())
                                {
                                    xmlWriter.WriteElementString("Status", "", S3Transforms.ToXmlStringValue(lifecycleConfigurationLifecycleConfigurationrulesListValue.Status));
                                }
                                else
                                {
                                    xmlWriter.WriteElementString("Status", "", "Disabled");
                                }
                                xmlWriter.WriteEndElement();
                            }
                        }
                    }
                    xmlWriter.WriteEndElement();
                }
            }

            try
            {
                var content = stringWriter.ToString();
                request.Content = Encoding.UTF8.GetBytes(content);
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

