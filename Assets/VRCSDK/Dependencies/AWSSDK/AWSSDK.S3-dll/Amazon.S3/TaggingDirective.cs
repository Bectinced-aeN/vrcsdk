using Amazon.Runtime;

namespace Amazon.S3
{
	internal sealed class TaggingDirective : ConstantClass
	{
		public static readonly TaggingDirective COPY = new TaggingDirective("COPY");

		public static readonly TaggingDirective REPLACE = new TaggingDirective("REPLACE");

		public TaggingDirective(string value)
			: this(value)
		{
		}

		public static TaggingDirective FindValue(string value)
		{
			return ConstantClass.FindValue<TaggingDirective>(value);
		}

		public static implicit operator TaggingDirective(string value)
		{
			return ConstantClass.FindValue<TaggingDirective>(value);
		}
	}
}
