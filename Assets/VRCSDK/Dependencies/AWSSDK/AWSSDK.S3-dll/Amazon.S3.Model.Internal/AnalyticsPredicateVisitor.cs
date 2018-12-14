using Amazon.S3.Model.Internal.MarshallTransformations;
using System.Xml;

namespace Amazon.S3.Model.Internal
{
	internal class AnalyticsPredicateVisitor : IAnalyticsPredicateVisitor
	{
		private readonly XmlWriter xmlWriter;

		public AnalyticsPredicateVisitor(XmlWriter xmlWriter)
		{
			this.xmlWriter = xmlWriter;
		}

		public void Visit(AnalyticsPrefixPredicate analyticsPrefixPredicate)
		{
			if (analyticsPrefixPredicate != null)
			{
				xmlWriter.WriteElementString("Prefix", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(analyticsPrefixPredicate.Prefix));
			}
		}

		public void visit(AnalyticsTagPredicate analyticsTagPredicate)
		{
			if (analyticsTagPredicate != null)
			{
				xmlWriter.WriteStartElement("Tag", "http://s3.amazonaws.com/doc/2006-03-01/");
				if (analyticsTagPredicate.Tag.IsSetKey())
				{
					xmlWriter.WriteElementString("Key", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(analyticsTagPredicate.Tag.Key));
				}
				if (analyticsTagPredicate.Tag.IsSetValue())
				{
					xmlWriter.WriteElementString("Value", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(analyticsTagPredicate.Tag.Value));
				}
				xmlWriter.WriteEndElement();
			}
		}

		public void visit(AnalyticsAndOperator analyticsAndOperatorPredicate)
		{
			if (analyticsAndOperatorPredicate != null)
			{
				xmlWriter.WriteStartElement("And", "http://s3.amazonaws.com/doc/2006-03-01/");
				foreach (AnalyticsFilterPredicate operand in analyticsAndOperatorPredicate.Operands)
				{
					operand?.Accept(this);
				}
				xmlWriter.WriteEndElement();
			}
		}
	}
}
