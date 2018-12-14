using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace AmplitudeSDKWrapper
{
	internal class DatabaseHelper
	{
		private static string DB_PATH = Path.Combine(Application.get_temporaryCachePath(), "amplitude.sqlite");

		private bool _initialized;

		public DatabaseHelper()
		{
			CheckInit();
		}

		public string GetConnectionString()
		{
			return DB_PATH;
		}

		public SQLiteConnection GetConnection()
		{
			return new SQLiteConnection(GetConnectionString());
		}

		public int AddEvent(string evt)
		{
			CheckInit();
			int ret = -1;
			CheckedExecute(delegate(SQLiteConnection db)
			{
				Event @event = new Event
				{
					Text = evt
				};
				if (db.Insert(@event) == 0)
				{
					Debug.Log((object)"AmplitudeAPI: DatabaseHelper.AddEvent: Insert failed");
				}
				else
				{
					ret = @event.Id;
				}
			}, delegate(Exception e)
			{
				Debug.Log((object)"AmplitudeAPI: DatabaseHelper.AddEvent failed");
				Debug.LogException(e);
			});
			return ret;
		}

		public int GetEventCount()
		{
			CheckInit();
			int ret = -1;
			CheckedExecute(delegate(SQLiteConnection db)
			{
				ret = db.Table<Event>().Count();
			}, delegate(Exception e)
			{
				Debug.Log((object)"AmplitudeAPI: DatabaseHelper.GetEventCount: GetNumberRows failed");
				Debug.LogException(e);
			});
			return ret;
		}

		public KeyValuePair<int, IEnumerable<Event>> GetEvents(int lessThanId, int limit)
		{
			CheckInit();
			KeyValuePair<int, IEnumerable<Event>> ret = new KeyValuePair<int, IEnumerable<Event>>(-1, new List<Event>());
			CheckedExecute(delegate(SQLiteConnection db)
			{
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Event), "e");
				TableQuery<Event> tableQuery = db.Table<Event>();
				if (lessThanId >= 0)
				{
					tableQuery = from e in tableQuery
					where e.Id < lessThanId
					select e;
				}
				tableQuery = tableQuery.OrderBy(Expression.Lambda<Func<Event, int>>(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/)), new ParameterExpression[1]
				{
					parameterExpression
				}));
				if (limit >= 0)
				{
					tableQuery = tableQuery.Take(limit);
				}
				List<Event> list = tableQuery.ToList();
				if (list.Count() > 0)
				{
					ret = new KeyValuePair<int, IEnumerable<Event>>(list.Last().Id, list);
				}
			}, delegate(Exception e)
			{
				Debug.Log((object)"AmplitudeAPI: DatabaseHelper.GetEvents failed");
				Debug.LogException(e);
			});
			return ret;
		}

		public int GetNthEventId(int n)
		{
			CheckInit();
			int retId = -1;
			CheckedExecute(delegate(SQLiteConnection db)
			{
				Event @event = (from i in db.Table<Event>()
				orderby i.Id
				select i).Skip(n - 1).Take(1).First();
				retId = @event.Id;
			}, delegate(Exception e)
			{
				Debug.Log((object)"AmplitudeAPI: DatabaseHelper.GetNthEventId failed");
				Debug.LogException(e);
			});
			return retId;
		}

		public void RemoveEvents(int maxId)
		{
			CheckInit();
			CheckedExecute(delegate(SQLiteConnection db)
			{
				db.Execute("DELETE FROM Event WHERE Id <= ?", maxId);
			}, delegate(Exception e)
			{
				Debug.Log((object)"AmplitudeAPI: DatabaseHelper.RemoveEvents failed");
				Debug.LogException(e);
			});
		}

		public void RemoveEvent(int id)
		{
			CheckInit();
			CheckedExecute(delegate(SQLiteConnection db)
			{
				db.Execute("DELETE FROM Event WHERE Id = ?", id);
			}, delegate(Exception e)
			{
				Debug.Log((object)"AmplitudeAPI: DatabaseHelper.RemoveEvent failed");
				Debug.LogException(e);
			});
		}

		private void CheckInit()
		{
			if (!_initialized)
			{
				CheckedExecute(delegate(SQLiteConnection db)
				{
					db.CreateTable<Event>();
					_initialized = true;
				}, delegate(Exception e)
				{
					Debug.Log((object)"AmplitudeAPI: DatabaseHelper: failed to initialize database");
					Debug.LogException(e);
				}, handleException: false);
			}
		}

		private void CheckedExecute(Action<SQLiteConnection> fn, Action<Exception> onError, bool handleException = true)
		{
			try
			{
				using (SQLiteConnection sQLiteConnection = GetConnection())
				{
					if (sQLiteConnection == null)
					{
						throw new Exception("No database connection");
					}
					fn?.Invoke(sQLiteConnection);
				}
			}
			catch (SQLiteException ex)
			{
				if (handleException && (ex.Result == SQLite3.Result.Corrupt || ex.Result == SQLite3.Result.SchemaChngd || ex.Result == SQLite3.Result.NonDBFile || ex.Result == SQLite3.Result.Full))
				{
					Debug.LogError((object)"AmplitudeAPI: DatabaseHelper: trying to delete corrupt database");
					DeleteFileAndWait(DB_PATH);
					_initialized = false;
					CheckInit();
					CheckedExecute(fn, onError, handleException: false);
				}
				else
				{
					onError?.Invoke(ex);
				}
			}
			catch (Exception obj)
			{
				onError?.Invoke(obj);
			}
		}

		private void DeleteFileAndWait(string fileToDelete)
		{
			try
			{
				FileInfo fileInfo = new FileInfo(fileToDelete);
				if (fileInfo.Exists)
				{
					fileInfo.Delete();
					fileInfo.Refresh();
					for (int i = 0; i < 5; i++)
					{
						if (!fileInfo.Exists)
						{
							break;
						}
						Thread.Sleep(100);
						fileInfo.Refresh();
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError((object)("DeleteFileAndWait exception: " + fileToDelete));
				Debug.LogException(ex);
			}
		}
	}
}
