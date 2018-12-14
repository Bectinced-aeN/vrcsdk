using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security;

namespace Amazon.S3.Model
{
	[Serializable]
	public class DeleteObjectsResponse : AmazonWebServiceResponse, ISerializable
	{
		private List<DeletedObject> deleted = new List<DeletedObject>();

		private List<DeleteError> errors = new List<DeleteError>();

		private RequestCharged requestCharged;

		public List<DeletedObject> DeletedObjects
		{
			get
			{
				return deleted;
			}
			set
			{
				deleted = value;
			}
		}

		public List<DeleteError> DeleteErrors
		{
			get
			{
				return errors;
			}
			set
			{
				errors = value;
			}
		}

		public RequestCharged RequestCharged
		{
			get
			{
				return requestCharged;
			}
			set
			{
				requestCharged = value;
			}
		}

		public DeleteObjectsResponse()
			: this()
		{
		}

		internal bool IsSetDeletedObjects()
		{
			return deleted.Count > 0;
		}

		internal bool IsSetDeleteErrors()
		{
			return errors.Count > 0;
		}

		internal bool IsSetRequestCharged()
		{
			return requestCharged != null;
		}

		protected DeleteObjectsResponse(SerializationInfo info, StreamingContext context)
			: this()
		{
			if (info != null)
			{
				deleted = (List<DeletedObject>)info.GetValue("deleted", typeof(List<DeletedObject>));
				errors = (List<DeleteError>)info.GetValue("errors", typeof(List<DeleteError>));
				requestCharged = RequestCharged.FindValue((string)info.GetValue("requestCharged", typeof(string)));
			}
		}

		[SecurityCritical]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info != null)
			{
				info.AddValue("deleted", deleted);
				info.AddValue("errors", errors);
				info.AddValue("requestCharged", ConstantClass.op_Implicit(requestCharged));
			}
		}
	}
}
