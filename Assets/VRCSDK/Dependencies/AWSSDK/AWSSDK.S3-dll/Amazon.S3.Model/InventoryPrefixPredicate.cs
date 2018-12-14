using Amazon.S3.Model.Internal;

namespace Amazon.S3.Model
{
	public sealed class InventoryPrefixPredicate : InventoryFilterPredicate
	{
		private readonly string prefix;

		public string Prefix => prefix;

		public InventoryPrefixPredicate(string prefix)
		{
			this.prefix = prefix;
		}

		internal override void Accept(IInventoryPredicateVisitor inventoryPredicateVisitor)
		{
			inventoryPredicateVisitor.Visit(this);
		}
	}
}
