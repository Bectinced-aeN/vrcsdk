using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class WebsiteConfiguration
	{
		private string errorDocument;

		private string indexDocumentSuffix;

		private RoutingRuleRedirect redirectAllRequestsTo;

		private List<RoutingRule> routingRules = new List<RoutingRule>();

		public string ErrorDocument
		{
			get
			{
				return errorDocument;
			}
			set
			{
				errorDocument = value;
			}
		}

		public string IndexDocumentSuffix
		{
			get
			{
				return indexDocumentSuffix;
			}
			set
			{
				indexDocumentSuffix = value;
			}
		}

		public RoutingRuleRedirect RedirectAllRequestsTo
		{
			get
			{
				return redirectAllRequestsTo;
			}
			set
			{
				redirectAllRequestsTo = value;
			}
		}

		public List<RoutingRule> RoutingRules
		{
			get
			{
				return routingRules;
			}
			set
			{
				routingRules = value;
			}
		}

		internal bool IsSetErrorDocument()
		{
			return errorDocument != null;
		}

		internal bool IsSetIndexDocumentSuffix()
		{
			return indexDocumentSuffix != null;
		}

		internal bool IsSetRedirectAllRequestsTo()
		{
			return redirectAllRequestsTo != null;
		}

		internal bool IsSetRoutingRules()
		{
			return routingRules.Count > 0;
		}
	}
}
