using System;
using System.Globalization;

namespace ThirdParty.iOS4Unity
{
	[Serializable]
	public struct CGPoint
	{
		public static readonly CGPoint Empty;

		public float X;

		public float Y;

		public bool IsEmpty
		{
			get
			{
				if ((double)X == 0.0)
				{
					return (double)Y == 0.0;
				}
				return false;
			}
		}

		public CGPoint(float x, float y)
		{
			X = x;
			Y = y;
		}

		public static CGPoint Add(CGPoint pt, CGSize sz)
		{
			return new CGPoint(pt.X + sz.Width, pt.Y + sz.Height);
		}

		public static CGPoint Subtract(CGPoint pt, CGSize sz)
		{
			return new CGPoint(pt.X - sz.Width, pt.Y - sz.Height);
		}

		public override bool Equals(object obj)
		{
			if (obj is CGPoint)
			{
				return this == (CGPoint)obj;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)X ^ (int)Y;
		}

		public override string ToString()
		{
			return $"{{X={X.ToString(CultureInfo.CurrentCulture)}, Y={Y.ToString(CultureInfo.CurrentCulture)}}}";
		}

		public static CGPoint operator +(CGPoint pt, CGSize sz)
		{
			return new CGPoint(pt.X + sz.Width, pt.Y + sz.Height);
		}

		public static bool operator ==(CGPoint left, CGPoint right)
		{
			if (left.X == right.X)
			{
				return left.Y == right.Y;
			}
			return false;
		}

		public static bool operator !=(CGPoint left, CGPoint right)
		{
			if (left.X == right.X)
			{
				return left.Y != right.Y;
			}
			return true;
		}

		public static CGPoint operator -(CGPoint pt, CGSize sz)
		{
			return new CGPoint(pt.X - sz.Width, pt.Y - sz.Height);
		}
	}
}
