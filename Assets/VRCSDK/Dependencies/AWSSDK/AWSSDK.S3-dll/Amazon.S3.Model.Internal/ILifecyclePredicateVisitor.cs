namespace Amazon.S3.Model.Internal
{
	internal interface ILifecyclePredicateVisitor
	{
		void Visit(LifecyclePrefixPredicate lifecyclePrefixPredicate);

		void Visit(LifecycleTagPredicate lifecycleTagPredicate);

		void Visit(LifecycleAndOperator lifecycleAndOperator);
	}
}
