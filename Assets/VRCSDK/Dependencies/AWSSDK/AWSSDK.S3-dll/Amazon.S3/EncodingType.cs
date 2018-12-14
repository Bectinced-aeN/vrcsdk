using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class EncodingType : ConstantClass
	{
		public static readonly EncodingType Url = new EncodingType("Url");

		public EncodingType(string value)
			: this(value)
		{
		}

		public static EncodingType FindValue(string value)
		{
			return ConstantClass.FindValue<EncodingType>(value);
		}

		public static implicit operator EncodingType(string value)
		{
			return FindValue(value);
		}
	}
}
