using System;

namespace Org.BouncyCastle.Asn1.X509
{
	internal class Time : Asn1Encodable, IAsn1Choice
	{
		internal Asn1Object time;

		public Time(Asn1Object time)
		{
			if (time == null)
			{
				throw new ArgumentNullException("time");
			}
			if (!(time is DerUtcTime) && !(time is DerGeneralizedTime))
			{
				throw new ArgumentException("unknown object passed to Time");
			}
			this.time = time;
		}

		public Time(DateTime date)
		{
			string text = date.ToString("yyyyMMddHHmmss") + "Z";
			int num = int.Parse(text.Substring(0, 4));
			if (num < 1950 || num > 2049)
			{
				time = new DerGeneralizedTime(text);
			}
			else
			{
				time = new DerUtcTime(text.Substring(2));
			}
		}

		public static Time GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return GetInstance(obj.GetObject());
		}

		public static Time GetInstance(object obj)
		{
			if (obj == null || obj is Time)
			{
				return (Time)obj;
			}
			if (obj is DerUtcTime)
			{
				return new Time((DerUtcTime)obj);
			}
			if (obj is DerGeneralizedTime)
			{
				return new Time((DerGeneralizedTime)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public string GetTime()
		{
			if (time is DerUtcTime)
			{
				return ((DerUtcTime)time).AdjustedTimeString;
			}
			return ((DerGeneralizedTime)time).GetTime();
		}

		public DateTime ToDateTime()
		{
			try
			{
				if (time is DerUtcTime)
				{
					return ((DerUtcTime)time).ToAdjustedDateTime();
				}
				return ((DerGeneralizedTime)time).ToDateTime();
				IL_003c:
				DateTime result;
				return result;
			}
			catch (FormatException ex)
			{
				throw new InvalidOperationException("invalid date string: " + ex.Message);
				IL_0058:
				DateTime result;
				return result;
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			return time;
		}

		public override string ToString()
		{
			return GetTime();
		}
	}
}
