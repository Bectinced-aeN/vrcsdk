using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;

namespace SQLite
{
	public class SQLiteConnection : IDisposable
	{
		private struct IndexedColumn
		{
			public int Order;

			public string ColumnName;
		}

		private struct IndexInfo
		{
			public string IndexName;

			public string TableName;

			public bool Unique;

			public List<IndexedColumn> Columns;
		}

		public class ColumnInfo
		{
			[Column("name")]
			public string Name
			{
				get;
				set;
			}

			public int notnull
			{
				get;
				set;
			}

			public override string ToString()
			{
				return Name;
			}
		}

		public delegate void TraceHandler(string message);

		public delegate void TimeExecutionHandler(TimeSpan executionTime, TimeSpan totalExecutionTime);

		private bool _open;

		private TimeSpan _busyTimeout;

		private Dictionary<string, TableMapping> _mappings;

		private Dictionary<string, TableMapping> _tables;

		private Stopwatch _sw;

		private TimeSpan _elapsed = default(TimeSpan);

		private int _transactionDepth;

		private Random _rand = new Random();

		internal static readonly IntPtr NullHandle;

		private static Dictionary<string, object> syncObjects;

		private static bool _preserveDuringLinkMagic;

		public IntPtr Handle
		{
			get;
			private set;
		}

		public string DatabasePath
		{
			get;
			private set;
		}

		public bool Trace
		{
			get;
			set;
		}

		public bool TimeExecution
		{
			get;
			set;
		}

		public bool StoreDateTimeAsTicks
		{
			get;
			private set;
		}

		public object SyncObject => syncObjects[DatabasePath];

		public TimeSpan BusyTimeout
		{
			get
			{
				return _busyTimeout;
			}
			set
			{
				_busyTimeout = value;
				if (Handle != NullHandle)
				{
					SQLite3.BusyTimeout(Handle, (int)_busyTimeout.TotalMilliseconds);
				}
			}
		}

		public IEnumerable<TableMapping> TableMappings
		{
			get
			{
				object result;
				if (_tables != null)
				{
					IEnumerable<TableMapping> values = _tables.Values;
					result = values;
				}
				else
				{
					result = Enumerable.Empty<TableMapping>();
				}
				return (IEnumerable<TableMapping>)result;
			}
		}

		public bool IsInTransaction => _transactionDepth > 0;

		public event TraceHandler TraceEvent;

		public event TimeExecutionHandler TimeExecutionEvent;

		public SQLiteConnection(string databasePath, bool storeDateTimeAsTicks = false)
			: this(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, storeDateTimeAsTicks)
		{
		}

		public SQLiteConnection(string databasePath, SQLiteOpenFlags openFlags, bool storeDateTimeAsTicks = false)
		{
			if (string.IsNullOrEmpty(databasePath))
			{
				throw new ArgumentException("Must be specified", "databasePath");
			}
			DatabasePath = databasePath;
			mayCreateSyncObject(databasePath);
			byte[] nullTerminatedUtf = GetNullTerminatedUtf8(DatabasePath);
			IntPtr db;
			SQLite3.Result result = SQLite3.Open(nullTerminatedUtf, out db, (int)openFlags, IntPtr.Zero);
			Handle = db;
			if (result != 0)
			{
				throw SQLiteException.New(result, $"Could not open database file: {DatabasePath} ({result})");
			}
			_open = true;
			StoreDateTimeAsTicks = storeDateTimeAsTicks;
			BusyTimeout = TimeSpan.FromSeconds(0.1);
		}

		static SQLiteConnection()
		{
			NullHandle = default(IntPtr);
			syncObjects = new Dictionary<string, object>();
			if (_preserveDuringLinkMagic)
			{
				ColumnInfo columnInfo = new ColumnInfo
				{
					Name = "magic"
				};
			}
		}

		internal void InvokeTrace(string message)
		{
			if (this.TraceEvent != null)
			{
				this.TraceEvent(message);
			}
		}

		internal void InvokeTimeExecution(TimeSpan executionTime, TimeSpan totalExecutionTime)
		{
			if (this.TimeExecutionEvent != null)
			{
				this.TimeExecutionEvent(executionTime, totalExecutionTime);
			}
		}

		private void mayCreateSyncObject(string databasePath)
		{
			if (!syncObjects.ContainsKey(databasePath))
			{
				syncObjects[databasePath] = new object();
			}
		}

		public void EnableLoadExtension(int onoff)
		{
			SQLite3.Result result = SQLite3.EnableLoadExtension(Handle, onoff);
			if (result != 0)
			{
				string errmsg = SQLite3.GetErrmsg(Handle);
				throw SQLiteException.New(result, errmsg);
			}
		}

		private static byte[] GetNullTerminatedUtf8(string s)
		{
			int byteCount = Encoding.UTF8.GetByteCount(s);
			byte[] array = new byte[byteCount + 1];
			byteCount = Encoding.UTF8.GetBytes(s, 0, s.Length, array, 0);
			return array;
		}

		public TableMapping GetMapping(Type type, CreateFlags createFlags = CreateFlags.None)
		{
			if (_mappings == null)
			{
				_mappings = new Dictionary<string, TableMapping>();
			}
			if (!_mappings.TryGetValue(type.FullName, out TableMapping value))
			{
				value = new TableMapping(type, createFlags);
				_mappings[type.FullName] = value;
			}
			return value;
		}

		public TableMapping GetMapping<T>()
		{
			return GetMapping(typeof(T));
		}

		public int DropTable<T>()
		{
			TableMapping mapping = GetMapping(typeof(T));
			string query = $"drop table if exists \"{mapping.TableName}\"";
			return Execute(query);
		}

		public int CreateTable<T>(CreateFlags createFlags = CreateFlags.None)
		{
			return CreateTable(typeof(T), createFlags);
		}

		public int CreateTable(Type ty, CreateFlags createFlags = CreateFlags.None)
		{
			if (_tables == null)
			{
				_tables = new Dictionary<string, TableMapping>();
			}
			if (!_tables.TryGetValue(ty.FullName, out TableMapping value))
			{
				value = GetMapping(ty, createFlags);
				_tables.Add(ty.FullName, value);
			}
			string str = "create table if not exists \"" + value.TableName + "\"(\n";
			IEnumerable<string> source = from p in value.Columns
			select Orm.SqlDecl(p, StoreDateTimeAsTicks);
			string str2 = string.Join(",\n", source.ToArray());
			str += str2;
			str += ")";
			int num = Execute(str);
			if (num == 0)
			{
				MigrateTable(value);
			}
			Dictionary<string, IndexInfo> dictionary = new Dictionary<string, IndexInfo>();
			TableMapping.Column[] columns = value.Columns;
			foreach (TableMapping.Column column in columns)
			{
				foreach (IndexedAttribute index in column.Indices)
				{
					string text = index.Name ?? (value.TableName + "_" + column.Name);
					if (!dictionary.TryGetValue(text, out IndexInfo value2))
					{
						value2 = default(IndexInfo);
						IndexInfo indexInfo = value2;
						indexInfo.IndexName = text;
						indexInfo.TableName = value.TableName;
						indexInfo.Unique = index.Unique;
						indexInfo.Columns = new List<IndexedColumn>();
						value2 = indexInfo;
						dictionary.Add(text, value2);
					}
					if (index.Unique != value2.Unique)
					{
						throw new Exception("All the columns in an index must have the same value for their Unique property");
					}
					value2.Columns.Add(new IndexedColumn
					{
						Order = index.Order,
						ColumnName = column.Name
					});
				}
			}
			foreach (string key in dictionary.Keys)
			{
				IndexInfo indexInfo2 = dictionary[key];
				string[] array = new string[indexInfo2.Columns.Count];
				if (indexInfo2.Columns.Count == 1)
				{
					string[] array2 = array;
					IndexedColumn indexedColumn = indexInfo2.Columns[0];
					array2[0] = indexedColumn.ColumnName;
				}
				else
				{
					indexInfo2.Columns.Sort((IndexedColumn lhs, IndexedColumn rhs) => lhs.Order - rhs.Order);
					int j = 0;
					for (int count = indexInfo2.Columns.Count; j < count; j++)
					{
						string[] array3 = array;
						int num2 = j;
						IndexedColumn indexedColumn2 = indexInfo2.Columns[j];
						array3[num2] = indexedColumn2.ColumnName;
					}
				}
				num += CreateIndex(key, indexInfo2.TableName, array, indexInfo2.Unique);
			}
			return num;
		}

		public int CreateIndex(string indexName, string tableName, string[] columnNames, bool unique = false)
		{
			string query = string.Format("create {2} index if not exists \"{3}\" on \"{0}\"(\"{1}\")", tableName, string.Join("\", \"", columnNames), (!unique) ? string.Empty : "unique", indexName);
			return Execute(query);
		}

		public int CreateIndex(string indexName, string tableName, string columnName, bool unique = false)
		{
			return CreateIndex(indexName, tableName, new string[1]
			{
				columnName
			}, unique);
		}

		public int CreateIndex(string tableName, string columnName, bool unique = false)
		{
			return CreateIndex(tableName + "_" + columnName, tableName, columnName, unique);
		}

		public int CreateIndex(string tableName, string[] columnNames, bool unique = false)
		{
			return CreateIndex(tableName + "_" + string.Join("_", columnNames), tableName, columnNames, unique);
		}

		public void CreateIndex<T>(Expression<Func<T, object>> property, bool unique = false)
		{
			MemberExpression memberExpression = (property.Body.NodeType != ExpressionType.Convert) ? (property.Body as MemberExpression) : (((UnaryExpression)property.Body).Operand as MemberExpression);
			PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
			if (propertyInfo == null)
			{
				throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
			}
			string name = propertyInfo.Name;
			TableMapping mapping = GetMapping<T>();
			string name2 = mapping.FindColumnWithPropertyName(name).Name;
			CreateIndex(mapping.TableName, name2, unique);
		}

		public List<ColumnInfo> GetTableInfo(string tableName)
		{
			string query = "pragma table_info(\"" + tableName + "\")";
			return Query<ColumnInfo>(query, new object[0]);
		}

		private void MigrateTable(TableMapping map)
		{
			List<ColumnInfo> tableInfo = GetTableInfo(map.TableName);
			List<TableMapping.Column> list = new List<TableMapping.Column>();
			TableMapping.Column[] columns = map.Columns;
			foreach (TableMapping.Column column in columns)
			{
				bool flag = false;
				foreach (ColumnInfo item in tableInfo)
				{
					flag = (string.Compare(column.Name, item.Name, StringComparison.OrdinalIgnoreCase) == 0);
					if (flag)
					{
						break;
					}
				}
				if (!flag)
				{
					list.Add(column);
				}
			}
			foreach (TableMapping.Column item2 in list)
			{
				string query = "alter table \"" + map.TableName + "\" add column " + Orm.SqlDecl(item2, StoreDateTimeAsTicks);
				Execute(query);
			}
		}

		protected virtual SQLiteCommand NewCommand()
		{
			return new SQLiteCommand(this);
		}

		public SQLiteCommand CreateCommand(string cmdText, params object[] ps)
		{
			if (!_open)
			{
				throw SQLiteException.New(SQLite3.Result.Error, "Cannot create commands from unopened database");
			}
			SQLiteCommand sQLiteCommand = NewCommand();
			sQLiteCommand.CommandText = cmdText;
			foreach (object val in ps)
			{
				sQLiteCommand.Bind(val);
			}
			return sQLiteCommand;
		}

		public int Execute(string query, params object[] args)
		{
			SQLiteCommand sQLiteCommand = CreateCommand(query, args);
			if (TimeExecution)
			{
				if (_sw == null)
				{
					_sw = new Stopwatch();
				}
				_sw.Reset();
				_sw.Start();
			}
			int result = sQLiteCommand.ExecuteNonQuery();
			if (TimeExecution)
			{
				_sw.Stop();
				_elapsed += _sw.Elapsed;
				InvokeTimeExecution(_sw.Elapsed, _elapsed);
			}
			return result;
		}

		public T ExecuteScalar<T>(string query, params object[] args)
		{
			SQLiteCommand sQLiteCommand = CreateCommand(query, args);
			if (TimeExecution)
			{
				if (_sw == null)
				{
					_sw = new Stopwatch();
				}
				_sw.Reset();
				_sw.Start();
			}
			T result = sQLiteCommand.ExecuteScalar<T>();
			if (TimeExecution)
			{
				_sw.Stop();
				_elapsed += _sw.Elapsed;
				InvokeTimeExecution(_sw.Elapsed, _elapsed);
			}
			return result;
		}

		public List<T> Query<T>(string query, params object[] args) where T : new()
		{
			SQLiteCommand sQLiteCommand = CreateCommand(query, args);
			return sQLiteCommand.ExecuteQuery<T>();
		}

		public IEnumerable<T> DeferredQuery<T>(string query, params object[] args) where T : new()
		{
			SQLiteCommand sQLiteCommand = CreateCommand(query, args);
			return sQLiteCommand.ExecuteDeferredQuery<T>();
		}

		public List<object> Query(TableMapping map, string query, params object[] args)
		{
			SQLiteCommand sQLiteCommand = CreateCommand(query, args);
			return sQLiteCommand.ExecuteQuery<object>(map);
		}

		public IEnumerable<object> DeferredQuery(TableMapping map, string query, params object[] args)
		{
			SQLiteCommand sQLiteCommand = CreateCommand(query, args);
			return sQLiteCommand.ExecuteDeferredQuery<object>(map);
		}

		public TableQuery<T> Table<T>() where T : new()
		{
			return new TableQuery<T>(this);
		}

		public T Get<T>(object pk) where T : new()
		{
			TableMapping mapping = GetMapping(typeof(T));
			return Query<T>(mapping.GetByPrimaryKeySql, new object[1]
			{
				pk
			}).First();
		}

		public T Get<T>(Expression<Func<T, bool>> predicate) where T : new()
		{
			return Table<T>().Where(predicate).First();
		}

		public T Find<T>(object pk) where T : new()
		{
			TableMapping mapping = GetMapping(typeof(T));
			return Query<T>(mapping.GetByPrimaryKeySql, new object[1]
			{
				pk
			}).FirstOrDefault();
		}

		public object Find(object pk, TableMapping map)
		{
			return Query(map, map.GetByPrimaryKeySql, pk).FirstOrDefault();
		}

		public T Find<T>(Expression<Func<T, bool>> predicate) where T : new()
		{
			return Table<T>().Where(predicate).FirstOrDefault();
		}

		public void BeginTransaction()
		{
			if (Interlocked.CompareExchange(ref _transactionDepth, 1, 0) != 0)
			{
				throw new InvalidOperationException("Cannot begin a transaction while already in a transaction.");
			}
			try
			{
				Execute("begin transaction");
			}
			catch (Exception ex)
			{
				SQLiteException ex2 = ex as SQLiteException;
				if (ex2 != null)
				{
					switch (ex2.Result)
					{
					case SQLite3.Result.Busy:
					case SQLite3.Result.NoMem:
					case SQLite3.Result.Interrupt:
					case SQLite3.Result.IOError:
					case SQLite3.Result.Full:
						RollbackTo(null, noThrow: true);
						break;
					}
				}
				else
				{
					Interlocked.Decrement(ref _transactionDepth);
				}
				throw;
				IL_008f:;
			}
		}

		public string SaveTransactionPoint()
		{
			int num = Interlocked.Increment(ref _transactionDepth) - 1;
			string text = "S" + _rand.Next(32767) + "D" + num;
			try
			{
				Execute("savepoint " + text);
				return text;
			}
			catch (Exception ex)
			{
				SQLiteException ex2 = ex as SQLiteException;
				if (ex2 != null)
				{
					switch (ex2.Result)
					{
					case SQLite3.Result.Busy:
					case SQLite3.Result.NoMem:
					case SQLite3.Result.Interrupt:
					case SQLite3.Result.IOError:
					case SQLite3.Result.Full:
						RollbackTo(null, noThrow: true);
						break;
					}
				}
				else
				{
					Interlocked.Decrement(ref _transactionDepth);
				}
				throw;
				IL_00d0:
				return text;
			}
		}

		public void Rollback()
		{
			RollbackTo(null, noThrow: false);
		}

		public void RollbackTo(string savepoint)
		{
			RollbackTo(savepoint, noThrow: false);
		}

		private void RollbackTo(string savepoint, bool noThrow)
		{
			try
			{
				if (string.IsNullOrEmpty(savepoint))
				{
					if (Interlocked.Exchange(ref _transactionDepth, 0) > 0)
					{
						Execute("rollback");
					}
				}
				else
				{
					DoSavePointExecute(savepoint, "rollback to ");
				}
			}
			catch (SQLiteException)
			{
				if (!noThrow)
				{
					throw;
				}
			}
		}

		public void Release(string savepoint)
		{
			DoSavePointExecute(savepoint, "release ");
		}

		private void DoSavePointExecute(string savepoint, string cmd)
		{
			int num = savepoint.IndexOf('D');
			int result;
			if (num >= 2 && savepoint.Length > num + 1 && int.TryParse(savepoint.Substring(num + 1), out result) && 0 <= result && result < _transactionDepth)
			{
				Thread.VolatileWrite(ref _transactionDepth, result);
				Execute(cmd + savepoint);
				return;
			}
			throw new ArgumentException("savePoint is not valid, and should be the result of a call to SaveTransactionPoint.", "savePoint");
		}

		public void Commit()
		{
			if (Interlocked.Exchange(ref _transactionDepth, 0) != 0)
			{
				Execute("commit");
			}
		}

		public void RunInTransaction(Action action)
		{
			try
			{
				lock (syncObjects[DatabasePath])
				{
					string savepoint = SaveTransactionPoint();
					action();
					Release(savepoint);
				}
			}
			catch (Exception)
			{
				Rollback();
				throw;
				IL_0045:;
			}
		}

		public void RunInDatabaseLock(Action action)
		{
			lock (syncObjects[DatabasePath])
			{
				action();
			}
		}

		public int InsertAll(IEnumerable objects)
		{
			int c = 0;
			RunInTransaction(delegate
			{
				foreach (object @object in objects)
				{
					c += Insert(@object);
				}
			});
			return c;
		}

		public int InsertAll(IEnumerable objects, string extra)
		{
			int c = 0;
			RunInTransaction(delegate
			{
				foreach (object @object in objects)
				{
					c += Insert(@object, extra);
				}
			});
			return c;
		}

		public int InsertAll(IEnumerable objects, Type objType)
		{
			int c = 0;
			RunInTransaction(delegate
			{
				foreach (object @object in objects)
				{
					c += Insert(@object, objType);
				}
			});
			return c;
		}

		public int Insert(object obj)
		{
			if (obj == null)
			{
				return 0;
			}
			return Insert(obj, string.Empty, obj.GetType());
		}

		public int InsertOrReplace(object obj)
		{
			if (obj == null)
			{
				return 0;
			}
			return Insert(obj, "OR REPLACE", obj.GetType());
		}

		public int Insert(object obj, Type objType)
		{
			return Insert(obj, string.Empty, objType);
		}

		public int InsertOrReplace(object obj, Type objType)
		{
			return Insert(obj, "OR REPLACE", objType);
		}

		public int Insert(object obj, string extra)
		{
			if (obj == null)
			{
				return 0;
			}
			return Insert(obj, extra, obj.GetType());
		}

		public int Insert(object obj, string extra, Type objType)
		{
			if (obj == null || objType == null)
			{
				return 0;
			}
			TableMapping mapping = GetMapping(objType);
			if (mapping.PK != null && mapping.PK.IsAutoGuid)
			{
				PropertyInfo property = objType.GetProperty(mapping.PK.PropertyName);
				if (property != null && property.GetGetMethod().Invoke(obj, null).Equals(Guid.Empty))
				{
					property.SetValue(obj, Guid.NewGuid(), null);
				}
			}
			TableMapping.Column[] array = (string.Compare(extra, "OR REPLACE", StringComparison.OrdinalIgnoreCase) != 0) ? mapping.InsertColumns : mapping.InsertOrReplaceColumns;
			object[] array2 = new object[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = array[i].GetValue(obj);
			}
			PreparedSqlLiteInsertCommand insertCommand = mapping.GetInsertCommand(this, extra);
			int result;
			try
			{
				result = insertCommand.ExecuteNonQuery(array2);
			}
			catch (SQLiteException ex)
			{
				if (SQLite3.ExtendedErrCode(Handle) == SQLite3.ExtendedResult.ConstraintNotNull)
				{
					throw NotNullConstraintViolationException.New(ex.Result, ex.Message, mapping, obj);
				}
				throw;
				IL_0120:;
			}
			if (mapping.HasAutoIncPK)
			{
				long id = SQLite3.LastInsertRowid(Handle);
				mapping.SetAutoIncPK(obj, id);
			}
			return result;
		}

		public int Update(object obj)
		{
			if (obj == null)
			{
				return 0;
			}
			return Update(obj, obj.GetType());
		}

		public int Update(object obj, Type objType)
		{
			int num = 0;
			if (obj == null || objType == null)
			{
				return 0;
			}
			TableMapping mapping = GetMapping(objType);
			TableMapping.Column pk = mapping.PK;
			if (pk != null)
			{
				IEnumerable<TableMapping.Column> source = from p in mapping.Columns
				where p != pk
				select p;
				IEnumerable<object> collection = from c in source
				select c.GetValue(obj);
				List<object> list = new List<object>(collection);
				list.Add(pk.GetValue(obj));
				string query = string.Format("update \"{0}\" set {1} where {2} = ? ", mapping.TableName, string.Join(",", (from c in source
				select "\"" + c.Name + "\" = ? ").ToArray()), pk.Name);
				try
				{
					return Execute(query, list.ToArray());
				}
				catch (SQLiteException ex)
				{
					if (ex.Result == SQLite3.Result.Constraint && SQLite3.ExtendedErrCode(Handle) == SQLite3.ExtendedResult.ConstraintNotNull)
					{
						throw NotNullConstraintViolationException.New(ex, mapping, obj);
					}
					throw ex;
					IL_014e:
					return num;
				}
			}
			throw new NotSupportedException("Cannot update " + mapping.TableName + ": it has no PK");
		}

		public int UpdateAll(IEnumerable objects)
		{
			int c = 0;
			RunInTransaction(delegate
			{
				foreach (object @object in objects)
				{
					c += Update(@object);
				}
			});
			return c;
		}

		public int Delete(object objectToDelete)
		{
			TableMapping mapping = GetMapping(objectToDelete.GetType());
			TableMapping.Column pK = mapping.PK;
			if (pK == null)
			{
				throw new NotSupportedException("Cannot delete " + mapping.TableName + ": it has no PK");
			}
			string query = $"delete from \"{mapping.TableName}\" where \"{pK.Name}\" = ?";
			return Execute(query, pK.GetValue(objectToDelete));
		}

		public int Delete<T>(object primaryKey)
		{
			TableMapping mapping = GetMapping(typeof(T));
			TableMapping.Column pK = mapping.PK;
			if (pK == null)
			{
				throw new NotSupportedException("Cannot delete " + mapping.TableName + ": it has no PK");
			}
			string query = $"delete from \"{mapping.TableName}\" where \"{pK.Name}\" = ?";
			return Execute(query, primaryKey);
		}

		public int DeleteAll<T>()
		{
			TableMapping mapping = GetMapping(typeof(T));
			string query = $"delete from \"{mapping.TableName}\"";
			return Execute(query);
		}

		~SQLiteConnection()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			Close();
		}

		public void Close()
		{
			if (_open && Handle != NullHandle)
			{
				try
				{
					if (_mappings != null)
					{
						foreach (TableMapping value in _mappings.Values)
						{
							value.Dispose();
						}
					}
					SQLite3.Result result = SQLite3.Close(Handle);
					if (result != 0)
					{
						string errmsg = SQLite3.GetErrmsg(Handle);
						throw SQLiteException.New(result, errmsg);
					}
				}
				finally
				{
					Handle = NullHandle;
					_open = false;
				}
			}
		}
	}
}
