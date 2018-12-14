namespace Amazon.Runtime
{
	public interface IMetricsFormatter
	{
		string FormatMetrics(IRequestMetrics metrics);
	}
}
