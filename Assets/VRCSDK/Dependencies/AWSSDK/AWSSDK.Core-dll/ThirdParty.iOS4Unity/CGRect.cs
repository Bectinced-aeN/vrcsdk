using System;

namespace ThirdParty.iOS4Unity
{
	[Serializable]
	public struct CGRect
	{
		public static readonly CGRect Empty;

		public float X;

		public float Height;

		public float Width;

		public float Y;

		public float Bottom => Y + Height;

		public bool IsEmpty
		{
			get
			{
				if (!(Width <= 0f))
				{
					return Height <= 0f;
				}
				return true;
			}
		}

		public CGPoint Location
		{
			get
			{
				return new CGPoint(X, Y);
			}
			set
			{
				X = value.X;
				Y = value.Y;
			}
		}

		public float Right => X + Width;

		public CGSize Size
		{
			get
			{
				return new CGSize(Width, Height);
			}
			set
			{
				Width = value.Width;
				Height = value.Height;
			}
		}

		public CGRect(CGPoint location, CGSize size)
		{
			X = location.X;
			Y = location.Y;
			Width = size.Width;
			Height = size.Height;
		}

		public CGRect(float x, float y, float width, float height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		public static CGRect FromLTRB(float left, float top, float right, float bottom)
		{
			return new CGRect(left, top, right - left, bottom - top);
		}

		public static CGRect Inflate(CGRect rect, float x, float y)
		{
			CGRect result = new CGRect(rect.X, rect.Y, rect.Width, rect.Height);
			result.Inflate(x, y);
			return result;
		}

		public static CGRect Intersect(CGRect a, CGRect b)
		{
			if (!a.IntersectsWithInclusive(b))
			{
				return Empty;
			}
			return FromLTRB(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Min(a.Right, b.Right), Math.Min(a.Bottom, b.Bottom));
		}

		public static CGRect Union(CGRect a, CGRect b)
		{
			return FromLTRB(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Max(a.Right, b.Right), Math.Max(a.Bottom, b.Bottom));
		}

		public bool Contains(float x, float y)
		{
			if (x >= X && x < Right && y >= Y)
			{
				return y < Bottom;
			}
			return false;
		}

		public bool Contains(CGRect rect)
		{
			if (X <= rect.X && Right >= rect.Right && Y <= rect.Y)
			{
				return Bottom >= rect.Bottom;
			}
			return false;
		}

		public bool Contains(CGPoint pt)
		{
			return Contains(pt.X, pt.Y);
		}

		public override bool Equals(object obj)
		{
			if (obj is CGRect)
			{
				return this == (CGRect)obj;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)(X + Y + Width + Height);
		}

		public void Inflate(CGSize size)
		{
			X -= size.Width;
			Y -= size.Height;
			Width += size.Width * 2f;
			Height += size.Height * 2f;
		}

		public void Inflate(float x, float y)
		{
			Inflate(new CGSize(x, y));
		}

		public void Intersect(CGRect rect)
		{
			this = Intersect(this, rect);
		}

		public bool IntersectsWith(CGRect rect)
		{
			if (X < rect.Right && Right > rect.X && Y < rect.Bottom)
			{
				return Bottom > rect.Y;
			}
			return false;
		}

		private bool IntersectsWithInclusive(CGRect r)
		{
			if (X <= r.Right && Right >= r.X && Y <= r.Bottom)
			{
				return Bottom >= r.Y;
			}
			return false;
		}

		public void Offset(float x, float y)
		{
			X += x;
			Y += y;
		}

		public void Offset(CGPoint pos)
		{
			Offset(pos.X, pos.Y);
		}

		public override string ToString()
		{
			return $"{{X={X},Y={Y},Width={Width},Height={Height}}}";
		}

		public static bool operator ==(CGRect left, CGRect right)
		{
			if (left.X == right.X && left.Y == right.Y && left.Width == right.Width)
			{
				return left.Height == right.Height;
			}
			return false;
		}

		public static bool operator !=(CGRect left, CGRect right)
		{
			if (left.X == right.X && left.Y == right.Y && left.Width == right.Width)
			{
				return left.Height != right.Height;
			}
			return true;
		}
	}
}
