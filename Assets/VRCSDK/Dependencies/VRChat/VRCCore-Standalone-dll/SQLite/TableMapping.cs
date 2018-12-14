using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SQLite
{
	public class TableMapping
	{
		public class Column
		{
			private PropertyInfo _prop;

			public string Name
			{
				get;
				private set;
			}

			public string PropertyName => _prop.Name;

			public Type ColumnType
			{
				get;
				private set;
			}

			public string Collation
			{
				get;
				private set;
			}

			public bool IsAutoInc
			{
				get;
				private set;
			}

			public bool IsAutoGuid
			{
				get;
				private set;
			}

			public bool IsPK
			{
				get;
				private set;
			}

			public IEnumerable<IndexedAttribute> Indices
			{
				get;
				set;
			}

			public bool IsNullable
			{
				get;
				private set;
			}

			public int? MaxStringLength
			{
				get;
				private set;
			}

			public Column(PropertyInfo prop, CreateFlags createFlags = CreateFlags.None)
			{
				ColumnAttribute columnAttribute = (ColumnAttribute)prop.GetCustomAttributes(typeof(ColumnAttribute), inherit: true).FirstOrDefault();
				_prop = prop;
				Name = ((columnAttribute != null) ? columnAttribute.Name : prop.Name);
				ColumnType = (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
				Collation = Orm.Collation(prop);
				IsPK = (Orm.IsPK(prop) || ((createFlags & CreateFlags.ImplicitPK) == CreateFlags.ImplicitPK && string.Compare(prop.Name, "Id", StringComparison.OrdinalIgnoreCase) == 0));
				bool flag = Orm.IsAutoInc(prop) || (IsPK && (createFlags & CreateFlags.AutoIncPK) == CreateFlags.AutoIncPK);
				IsAutoGuid = (flag && ColumnType == typeof(Guid));
				IsAutoInc = (flag && !IsAutoGuid);
				Indices = Orm.GetIndices(prop);
				if (!Indices.Any() && !IsPK && (createFlags & CreateFlags.ImplicitIndex) == CreateFlags.ImplicitIndex && Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
				{
					Indices = new IndexedAttribute[1]
					{
						new IndexedAttribute()
					};
				}
				IsNullable = (!IsPK && !Orm.IsMarkedNotNull(prop));
				MaxStringLength = Orm.MaxStringLength(prop);
			}

			public void SetValue(object obj, object val)
			{
				_prop.SetValue(obj, val, null);
			}

			public object GetValue(object obj)
			{
				return _prop.GetGetMethod().Invoke(obj, null);
			}
		}

		private Column _autoPk;

		private Column[] _insertColumns;

		private Column[] _insertOrReplaceColumns;

		private PreparedSqlLiteInsertCommand _insertCommand;

		private string _insertCommandExtra;

		public Type MappedType
		{
			get;
			private set;
		}

		public string TableName
		{
			get;
			private set;
		}

		public Column[] Columns
		{
			get;
			private set;
		}

		public Column PK
		{
			get;
			private set;
		}

		public string GetByPrimaryKeySql
		{
			get;
			private set;
		}

		public bool HasAutoIncPK
		{
			get;
			private set;
		}

		public Column[] InsertColumns
		{
			get
			{
				if (_insertColumns == null)
				{
					_insertColumns = (from c in Columns
					where !c.IsAutoInc
					select c).ToArray();
				}
				return _insertColumns;
			}
		}

		public Column[] InsertOrReplaceColumns
		{
			get
			{
				if (_insertOrReplaceColumns == null)
				{
					_insertOrReplaceColumns = Columns.ToArray();
				}
				return _insertOrReplaceColumns;
			}
		}

		public TableMapping(Type type, CreateFlags createFlags = CreateFlags.None)
		{
			MappedType = type;
			TableAttribute tableAttribute = (TableAttribute)type.GetCustomAttributes(typeof(TableAttribute), inherit: true).FirstOrDefault();
			TableName = ((tableAttribute == null) ? MappedType.Name : tableAttribute.Name);
			PropertyInfo[] properties = MappedType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
			List<Column> list = new List<Column>();
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				bool flag = propertyInfo.GetCustomAttributes(typeof(IgnoreAttribute), inherit: true).Length > 0;
				if (propertyInfo.CanWrite && !flag)
				{
					list.Add(new Column(propertyInfo, createFlags));
				}
			}
			Columns = list.ToArray();
			Column[] columns = Columns;
			foreach (Column column in columns)
			{
				if (column.IsAutoInc && column.IsPK)
				{
					_autoPk = column;
				}
				if (column.IsPK)
				{
					PK = column;
				}
			}
			HasAutoIncPK = (_autoPk != null);
			if (PK != null)
			{
				GetByPrimaryKeySql = $"select * from \"{TableName}\" where \"{PK.Name}\" = ?";
			}
			else
			{
				GetByPrimaryKeySql = $"select * from \"{TableName}\" limit 1";
			}
		}

		public void SetAutoIncPK(object obj, long id)
		{
			if (_autoPk != null)
			{
				_autoPk.SetValue(obj, Convert.ChangeType(id, _autoPk.ColumnType, null));
			}
		}

		public Column FindColumnWithPropertyName(string propertyName)
		{
			return Columns.FirstOrDefault((Column c) => c.PropertyName == propertyName);
		}

		public Column FindColumn(string columnName)
		{
			return Columns.FirstOrDefault((Column c) => c.Name == columnName);
		}

		public PreparedSqlLiteInsertCommand GetInsertCommand(SQLiteConnection conn, string extra)
		{
			if (_insertCommand == null)
			{
				_insertCommand = CreateInsertCommand(conn, extra);
				_insertCommandExtra = extra;
			}
			else if (_insertCommandExtra != extra)
			{
				_insertCommand.Dispose();
				_insertCommand = CreateInsertCommand(conn, extra);
				_insertCommandExtra = extra;
			}
			return _insertCommand;
		}

		private PreparedSqlLiteInsertCommand CreateInsertCommand(SQLiteConnection conn, string extra)
		{
			Column[] source = InsertColumns;
			string commandText;
			if (!source.Any() && Columns.Count() == 1 && Columns[0].IsAutoInc)
			{
				commandText = string.Format("insert {1} into \"{0}\" default values", TableName, extra);
			}
			else
			{
				if (string.Compare(extra, "OR REPLACE", StringComparison.OrdinalIgnoreCase) == 0)
				{
					source = InsertOrReplaceColumns;
				}
				commandText = string.Format("insert {3} into \"{0}\"({1}) values ({2})", TableName, string.Join(",", (from c in source
				select "\"" + c.Name + "\"").ToArray()), string.Join(",", (from c in source
				select "?").ToArray()), extra);
			}
			PreparedSqlLiteInsertCommand preparedSqlLiteInsertCommand = new PreparedSqlLiteInsertCommand(conn);
			preparedSqlLiteInsertCommand.CommandText = commandText;
			return preparedSqlLiteInsertCommand;
		}

		protected internal void Dispose()
		{
			if (_insertCommand != null)
			{
				_insertCommand.Dispose();
				_insertCommand = null;
			}
		}
	}
}
