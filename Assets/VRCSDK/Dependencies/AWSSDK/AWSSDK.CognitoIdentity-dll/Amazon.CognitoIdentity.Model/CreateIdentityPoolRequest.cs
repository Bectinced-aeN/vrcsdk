using System.Collections.Generic;

namespace Amazon.CognitoIdentity.Model
{
	public class CreateIdentityPoolRequest : AmazonCognitoIdentityRequest
	{
		private bool? _allowUnauthenticatedIdentities;

		private List<CognitoIdentityProviderInfo> _cognitoIdentityProviders = new List<CognitoIdentityProviderInfo>();

		private string _developerProviderName;

		private string _identityPoolName;

		private List<string> _openIdConnectProviderARNs = new List<string>();

		private List<string> _samlProviderARNs = new List<string>();

		private Dictionary<string, string> _supportedLoginProviders = new Dictionary<string, string>();

		public bool AllowUnauthenticatedIdentities
		{
			get
			{
				return _allowUnauthenticatedIdentities.GetValueOrDefault();
			}
			set
			{
				_allowUnauthenticatedIdentities = value;
			}
		}

		public List<CognitoIdentityProviderInfo> CognitoIdentityProviders
		{
			get
			{
				return _cognitoIdentityProviders;
			}
			set
			{
				_cognitoIdentityProviders = value;
			}
		}

		public string DeveloperProviderName
		{
			get
			{
				return _developerProviderName;
			}
			set
			{
				_developerProviderName = value;
			}
		}

		public string IdentityPoolName
		{
			get
			{
				return _identityPoolName;
			}
			set
			{
				_identityPoolName = value;
			}
		}

		public List<string> OpenIdConnectProviderARNs
		{
			get
			{
				return _openIdConnectProviderARNs;
			}
			set
			{
				_openIdConnectProviderARNs = value;
			}
		}

		public List<string> SamlProviderARNs
		{
			get
			{
				return _samlProviderARNs;
			}
			set
			{
				_samlProviderARNs = value;
			}
		}

		public Dictionary<string, string> SupportedLoginProviders
		{
			get
			{
				return _supportedLoginProviders;
			}
			set
			{
				_supportedLoginProviders = value;
			}
		}

		internal bool IsSetAllowUnauthenticatedIdentities()
		{
			return _allowUnauthenticatedIdentities.HasValue;
		}

		internal bool IsSetCognitoIdentityProviders()
		{
			if (_cognitoIdentityProviders != null)
			{
				return _cognitoIdentityProviders.Count > 0;
			}
			return false;
		}

		internal bool IsSetDeveloperProviderName()
		{
			return _developerProviderName != null;
		}

		internal bool IsSetIdentityPoolName()
		{
			return _identityPoolName != null;
		}

		internal bool IsSetOpenIdConnectProviderARNs()
		{
			if (_openIdConnectProviderARNs != null)
			{
				return _openIdConnectProviderARNs.Count > 0;
			}
			return false;
		}

		internal bool IsSetSamlProviderARNs()
		{
			if (_samlProviderARNs != null)
			{
				return _samlProviderARNs.Count > 0;
			}
			return false;
		}

		internal bool IsSetSupportedLoginProviders()
		{
			if (_supportedLoginProviders != null)
			{
				return _supportedLoginProviders.Count > 0;
			}
			return false;
		}
	}
}
