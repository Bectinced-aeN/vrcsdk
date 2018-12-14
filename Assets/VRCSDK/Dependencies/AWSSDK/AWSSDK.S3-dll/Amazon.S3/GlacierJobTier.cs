using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class GlacierJobTier : ConstantClass
	{
		public static readonly GlacierJobTier Standard = new GlacierJobTier("Standard");

		public static readonly GlacierJobTier Bulk = new GlacierJobTier("Bulk");

		public static readonly GlacierJobTier Expedited = new GlacierJobTier("Expedited");

		private GlacierJobTier(string value)
			: this(value)
		{
		}

		public static GlacierJobTier FindValue(string value)
		{
			return ConstantClass.FindValue<GlacierJobTier>(value);
		}

		public static implicit operator GlacierJobTier(string value)
		{
			return ConstantClass.FindValue<GlacierJobTier>(value);
		}
	}
}
