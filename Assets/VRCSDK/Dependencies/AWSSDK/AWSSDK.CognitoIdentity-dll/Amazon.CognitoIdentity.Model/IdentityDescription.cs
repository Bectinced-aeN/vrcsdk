using System;
using System.Collections.Generic;

namespace Amazon.CognitoIdentity.Model
{
	public class IdentityDescription
	{
		private DateTime? _creationDate;

		private string _identityId;

		private DateTime? _lastModifiedDate;

		private List<string> _logins = new List<string>();

		public DateTime CreationDate
		{
			get
			{
				return _creationDate.GetValueOrDefault();
			}
			set
			{
				_creationDate = value;
			}
		}

		public string IdentityId
		{
			get
			{
				return _identityId;
			}
			set
			{
				_identityId = value;
			}
		}

		public DateTime LastModifiedDate
		{
			get
			{
				return _lastModifiedDate.GetValueOrDefault();
			}
			set
			{
				_lastModifiedDate = value;
			}
		}

		public List<string> Logins
		{
			get
			{
				return _logins;
			}
			set
			{
				_logins = value;
			}
		}

		internal bool IsSetCreationDate()
		{
			return _creationDate.HasValue;
		}

		internal bool IsSetIdentityId()
		{
			return _identityId != null;
		}

		internal bool IsSetLastModifiedDate()
		{
			return _lastModifiedDate.HasValue;
		}

		internal bool IsSetLogins()
		{
			if (_logins != null)
			{
				return _logins.Count > 0;
			}
			return false;
		}
	}
}
