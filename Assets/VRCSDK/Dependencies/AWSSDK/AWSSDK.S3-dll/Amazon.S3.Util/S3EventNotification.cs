using Amazon.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using ThirdParty.Json.LitJson;

namespace Amazon.S3.Util
{
	public class S3EventNotification
	{
		public class UserIdentityEntity
		{
			public string PrincipalId
			{
				get;
				set;
			}
		}

		public class S3BucketEntity
		{
			public string Name
			{
				get;
				set;
			}

			public UserIdentityEntity OwnerIdentity
			{
				get;
				set;
			}

			public string Arn
			{
				get;
				set;
			}
		}

		public class S3ObjectEntity
		{
			public string Key
			{
				get;
				set;
			}

			public long Size
			{
				get;
				set;
			}

			public string ETag
			{
				get;
				set;
			}

			public string VersionId
			{
				get;
				set;
			}
		}

		public class S3Entity
		{
			public string ConfigurationId
			{
				get;
				set;
			}

			public S3BucketEntity Bucket
			{
				get;
				set;
			}

			public S3ObjectEntity Object
			{
				get;
				set;
			}

			public string S3SchemaVersion
			{
				get;
				set;
			}
		}

		public class RequestParametersEntity
		{
			public string SourceIPAddress
			{
				get;
				set;
			}
		}

		public class ResponseElementsEntity
		{
			public string XAmzId2
			{
				get;
				set;
			}

			public string XAmzRequestId
			{
				get;
				set;
			}
		}

		public class S3EventNotificationRecord
		{
			public string AwsRegion
			{
				get;
				set;
			}

			public EventType EventName
			{
				get;
				set;
			}

			public string EventSource
			{
				get;
				set;
			}

			public DateTime EventTime
			{
				get;
				set;
			}

			public string EventVersion
			{
				get;
				set;
			}

			public RequestParametersEntity RequestParameters
			{
				get;
				set;
			}

			public ResponseElementsEntity ResponseElements
			{
				get;
				set;
			}

			public S3Entity S3
			{
				get;
				set;
			}

			public UserIdentityEntity UserIdentity
			{
				get;
				set;
			}
		}

		public List<S3EventNotificationRecord> Records
		{
			get;
			set;
		}

		public static S3EventNotification ParseJson(string json)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Expected O, but got Unknown
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				JsonData val = JsonMapper.ToObject(json);
				S3EventNotification s3EventNotification = new S3EventNotification
				{
					Records = new List<S3EventNotificationRecord>()
				};
				if (val.get_Item("Records") != null)
				{
					foreach (JsonData item in (IEnumerable)val.get_Item("Records"))
					{
						JsonData val2 = item;
						S3EventNotificationRecord s3EventNotificationRecord = new S3EventNotificationRecord();
						s3EventNotificationRecord.EventVersion = GetValueAsString(val2, "eventVersion");
						s3EventNotificationRecord.EventSource = GetValueAsString(val2, "eventSource");
						s3EventNotificationRecord.AwsRegion = GetValueAsString(val2, "awsRegion");
						s3EventNotificationRecord.EventVersion = GetValueAsString(val2, "eventVersion");
						if (val2.get_Item("eventTime") != null)
						{
							s3EventNotificationRecord.EventTime = DateTime.Parse((string)val2.get_Item("eventTime"), CultureInfo.InvariantCulture);
						}
						if (val2.get_Item("eventName") != null)
						{
							string text = (string)val2.get_Item("eventName");
							if (!text.StartsWith("s3:", StringComparison.OrdinalIgnoreCase))
							{
								text = "s3:" + text;
							}
							s3EventNotificationRecord.EventName = EventType.FindValue(text);
						}
						if (val2.get_Item("userIdentity") != null)
						{
							JsonData data = val2.get_Item("userIdentity");
							s3EventNotificationRecord.UserIdentity = new UserIdentityEntity();
							s3EventNotificationRecord.UserIdentity.PrincipalId = GetValueAsString(data, "principalId");
						}
						if (val2.get_Item("requestParameters") != null)
						{
							JsonData data2 = val2.get_Item("requestParameters");
							s3EventNotificationRecord.RequestParameters = new RequestParametersEntity();
							s3EventNotificationRecord.RequestParameters.SourceIPAddress = GetValueAsString(data2, "sourceIPAddress");
						}
						if (val2.get_Item("responseElements") != null)
						{
							JsonData data3 = val2.get_Item("responseElements");
							s3EventNotificationRecord.ResponseElements = new ResponseElementsEntity();
							s3EventNotificationRecord.ResponseElements.XAmzRequestId = GetValueAsString(data3, "x-amz-request-id");
							s3EventNotificationRecord.ResponseElements.XAmzId2 = GetValueAsString(data3, "x-amz-id-2");
						}
						if (val2.get_Item("s3") != null)
						{
							JsonData val3 = val2.get_Item("s3");
							s3EventNotificationRecord.S3 = new S3Entity();
							s3EventNotificationRecord.S3.S3SchemaVersion = GetValueAsString(val3, "s3SchemaVersion");
							s3EventNotificationRecord.S3.ConfigurationId = GetValueAsString(val3, "configurationId");
							if (val3.get_Item("bucket") != null)
							{
								JsonData val4 = val3.get_Item("bucket");
								s3EventNotificationRecord.S3.Bucket = new S3BucketEntity();
								s3EventNotificationRecord.S3.Bucket.Name = GetValueAsString(val4, "name");
								s3EventNotificationRecord.S3.Bucket.Arn = GetValueAsString(val4, "arn");
								if (val4.get_Item("ownerIdentity") != null)
								{
									JsonData data4 = val4.get_Item("ownerIdentity");
									s3EventNotificationRecord.S3.Bucket.OwnerIdentity = new UserIdentityEntity();
									s3EventNotificationRecord.S3.Bucket.OwnerIdentity.PrincipalId = GetValueAsString(data4, "principalId");
								}
							}
							if (val3.get_Item("object") != null)
							{
								JsonData data5 = val3.get_Item("object");
								s3EventNotificationRecord.S3.Object = new S3ObjectEntity();
								s3EventNotificationRecord.S3.Object.Key = GetValueAsString(data5, "key");
								s3EventNotificationRecord.S3.Object.Size = GetValueAsLong(data5, "size");
								s3EventNotificationRecord.S3.Object.ETag = GetValueAsString(data5, "eTag");
								s3EventNotificationRecord.S3.Object.VersionId = GetValueAsString(data5, "versionId");
							}
						}
						s3EventNotification.Records.Add(s3EventNotificationRecord);
					}
				}
				return s3EventNotification;
			}
			catch (Exception ex)
			{
				throw new AmazonClientException("Failed to parse json string: " + ex.Message, ex);
			}
		}

		private static string GetValueAsString(JsonData data, string key)
		{
			if (data.get_Item(key) != null)
			{
				return (string)data.get_Item(key);
			}
			return null;
		}

		private static long GetValueAsLong(JsonData data, string key)
		{
			if (data.get_Item(key) != null)
			{
				if (data.get_Item(key).get_IsInt())
				{
					return (int)data.get_Item(key);
				}
				return (long)data.get_Item(key);
			}
			return 0L;
		}
	}
}
