using Amazon.S3.Model.Internal.MarshallTransformations;
using System.Xml;

namespace Amazon.S3.Model.Internal
{
	internal class InventoryPredicateVisitor : IInventoryPredicateVisitor
	{
		private readonly XmlWriter xmlWriter;

		public InventoryPredicateVisitor(XmlWriter xmlWriter)
		{
			this.xmlWriter = xmlWriter;
		}

		public void Visit(InventoryPrefixPredicate inventoryPrefixPredicate)
		{
			if (inventoryPrefixPredicate != null)
			{
				xmlWriter.WriteElementString("Prefix", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(inventoryPrefixPredicate.Prefix));
			}
		}
	}
}
