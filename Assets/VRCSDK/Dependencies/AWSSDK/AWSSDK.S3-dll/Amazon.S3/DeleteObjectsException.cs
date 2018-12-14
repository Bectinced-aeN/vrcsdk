using Amazon.S3.Model;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security;

namespace Amazon.S3
{
	[Serializable]
	public class DeleteObjectsException : AmazonS3Exception
	{
		private DeleteObjectsResponse response;

		public DeleteObjectsResponse Response
		{
			get
			{
				return response;
			}
			set
			{
				response = value;
			}
		}

		public DeleteObjectsException(DeleteObjectsResponse response)
			: base(CreateMessage(response))
		{
			this.response = response;
		}

		private static string CreateMessage(DeleteObjectsResponse response)
		{
			if (response == null)
			{
				throw new ArgumentNullException("response");
			}
			return string.Format(CultureInfo.InvariantCulture, "Error deleting objects. Deleted objects: {0}. Delete errors: {1}", (response.DeletedObjects != null) ? response.DeletedObjects.Count : 0, (response.DeleteErrors != null) ? response.DeleteErrors.Count : 0);
		}

		protected DeleteObjectsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info != null)
			{
				Response = (info.GetValue("Response", typeof(DeleteObjectsResponse)) as DeleteObjectsResponse);
			}
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info?.AddValue("Response", Response);
		}
	}
}
