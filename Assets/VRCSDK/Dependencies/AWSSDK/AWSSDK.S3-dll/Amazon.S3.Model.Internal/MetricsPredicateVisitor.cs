using Amazon.S3.Model.Internal.MarshallTransformations;
using System.Xml;

namespace Amazon.S3.Model.Internal
{
	internal class MetricsPredicateVisitor : IMetricsPredicateVisitor
	{
		private readonly XmlWriter xmlWriter;

		public MetricsPredicateVisitor(XmlWriter xmlWriter)
		{
			this.xmlWriter = xmlWriter;
		}

		public void Visit(MetricsPrefixPredicate metricsPrefixPredicate)
		{
			if (metricsPrefixPredicate != null)
			{
				xmlWriter.WriteElementString("Prefix", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(metricsPrefixPredicate.Prefix));
			}
		}

		public void visit(MetricsTagPredicate metricsTagPredicate)
		{
			if (metricsTagPredicate != null)
			{
				xmlWriter.WriteStartElement("Tag", "http://s3.amazonaws.com/doc/2006-03-01/");
				if (metricsTagPredicate.Tag.IsSetKey())
				{
					xmlWriter.WriteElementString("Key", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(metricsTagPredicate.Tag.Key));
				}
				if (metricsTagPredicate.Tag.IsSetValue())
				{
					xmlWriter.WriteElementString("Value", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(metricsTagPredicate.Tag.Value));
				}
				xmlWriter.WriteEndElement();
			}
		}

		public void visit(MetricsAndOperator metricsAndOperatorPredicate)
		{
			if (metricsAndOperatorPredicate != null)
			{
				xmlWriter.WriteStartElement("And", "http://s3.amazonaws.com/doc/2006-03-01/");
				foreach (MetricsFilterPredicate operand in metricsAndOperatorPredicate.Operands)
				{
					operand?.Accept(this);
				}
				xmlWriter.WriteEndElement();
			}
		}
	}
}
