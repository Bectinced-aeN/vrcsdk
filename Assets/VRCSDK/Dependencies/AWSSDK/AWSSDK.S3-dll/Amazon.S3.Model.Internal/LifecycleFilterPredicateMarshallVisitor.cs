using Amazon.S3.Model.Internal.MarshallTransformations;
using System.Xml;

namespace Amazon.S3.Model.Internal
{
	internal class LifecycleFilterPredicateMarshallVisitor : ILifecyclePredicateVisitor
	{
		private XmlWriter xmlWriter;

		public LifecycleFilterPredicateMarshallVisitor(XmlWriter xmlWriter)
		{
			this.xmlWriter = xmlWriter;
		}

		public void Visit(LifecyclePrefixPredicate lifecyclePrefixPredicate)
		{
			xmlWriter.WriteElementString("Prefix", "", S3Transforms.ToXmlStringValue(lifecyclePrefixPredicate.Prefix));
		}

		public void Visit(LifecycleTagPredicate lifecycleTagPredicate)
		{
			xmlWriter.WriteStartElement("Tag", "");
			if (lifecycleTagPredicate.IsSetTag())
			{
				xmlWriter.WriteElementString("Key", "", S3Transforms.ToXmlStringValue(lifecycleTagPredicate.Tag.Key));
				xmlWriter.WriteElementString("Value", "", S3Transforms.ToXmlStringValue(lifecycleTagPredicate.Tag.Value));
			}
			xmlWriter.WriteEndElement();
		}

		public void Visit(LifecycleAndOperator lifecycleAndOperator)
		{
			xmlWriter.WriteStartElement("And", "");
			if (lifecycleAndOperator.IsSetOperands())
			{
				foreach (LifecycleFilterPredicate operand in lifecycleAndOperator.Operands)
				{
					operand?.Accept(this);
				}
			}
			xmlWriter.WriteEndElement();
		}
	}
}
