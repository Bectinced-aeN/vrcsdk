namespace Amazon.S3.Model.Internal
{
	internal interface IInventoryPredicateVisitor
	{
		void Visit(InventoryPrefixPredicate inventoryPrefixPredicate);
	}
}
