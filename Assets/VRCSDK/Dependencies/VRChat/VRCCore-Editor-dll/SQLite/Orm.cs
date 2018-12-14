using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SQLite
{
	public static class Orm
	{
		public const int DefaultMaxStringLength = 140;

		public const string ImplicitPkName = "Id";

		public const string ImplicitIndexSuffix = "Id";

		public static string SqlDecl(TableMapping.Column p, bool storeDateTimeAsTicks)
		{
			string text = "\"" + p.Name + "\" " + SqlType(p, storeDateTimeAsTicks) + " ";
			if (p.IsPK)
			{
				text += "primary key ";
			}
			if (p.IsAutoInc)
			{
				text += "autoincrement ";
			}
			if (!p.IsNullable)
			{
				text += "not null ";
			}
			if (!string.IsNullOrEmpty(p.Collation))
			{
				text = text + "collate " + p.Collation + " ";
			}
			return text;
		}

		public static string SqlType(TableMapping.Column p, bool storeDateTimeAsTicks)
		{
			Type columnType = p.ColumnType;
			if (columnType == typeof(bool) || columnType == typeof(byte) || columnType == typeof(ushort) || columnType == typeof(sbyte) || columnType == typeof(short) || columnType == typeof(int))
			{
				return "integer";
			}
			if (columnType == typeof(uint) || columnType == typeof(long))
			{
				return "bigint";
			}
			if (columnType == typeof(float) || columnType == typeof(double) || columnType == typeof(decimal))
			{
				return "float";
			}
			if (columnType == typeof(string))
			{
				int? maxStringLength = p.MaxStringLength;
				if (maxStringLength.HasValue)
				{
					return "varchar(" + maxStringLength.Value + ")";
				}
				return "varchar";
			}
			if (columnType == typeof(TimeSpan))
			{
				return "bigint";
			}
			if (columnType == typeof(DateTime))
			{
				return (!storeDateTimeAsTicks) ? "datetime" : "bigint";
			}
			if (columnType == typeof(DateTimeOffset))
			{
				return "bigint";
			}
			if (columnType.IsEnum)
			{
				return "integer";
			}
			if (columnType == typeof(byte[]))
			{
				return "blob";
			}
			if (columnType == typeof(Guid))
			{
				return "varchar(36)";
			}
			throw new NotSupportedException("Don't know about " + columnType);
		}

		public static bool IsPK(MemberInfo p)
		{
			object[] customAttributes = p.GetCustomAttributes(typeof(PrimaryKeyAttribute), inherit: true);
			return customAttributes.Length > 0;
		}

		public static string Collation(MemberInfo p)
		{
			object[] customAttributes = p.GetCustomAttributes(typeof(CollationAttribute), inherit: true);
			if (customAttributes.Length > 0)
			{
				return ((CollationAttribute)customAttributes[0]).Value;
			}
			return string.Empty;
		}

		public static bool IsAutoInc(MemberInfo p)
		{
			object[] customAttributes = p.GetCustomAttributes(typeof(AutoIncrementAttribute), inherit: true);
			return customAttributes.Length > 0;
		}

		public static IEnumerable<IndexedAttribute> GetIndices(MemberInfo p)
		{
			object[] customAttributes = p.GetCustomAttributes(typeof(IndexedAttribute), inherit: true);
			return customAttributes.Cast<IndexedAttribute>();
		}

		public static int? MaxStringLength(PropertyInfo p)
		{
			object[] customAttributes = p.GetCustomAttributes(typeof(MaxLengthAttribute), inherit: true);
			if (customAttributes.Length > 0)
			{
				return ((MaxLengthAttribute)customAttributes[0]).Value;
			}
			return null;
		}

		public static bool IsMarkedNotNull(MemberInfo p)
		{
			object[] customAttributes = p.GetCustomAttributes(typeof(NotNullAttribute), inherit: true);
			return customAttributes.Length > 0;
		}
	}
}
