using Amazon.S3.Model.Internal;

namespace Amazon.S3.Model
{
	public abstract class InventoryFilterPredicate
	{
		internal abstract void Accept(IInventoryPredicateVisitor inventoryPredicateVisitor);
	}
}
