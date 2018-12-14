using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace AmplitudeSDKWrapper
{
	internal class DatabaseHelper
	{
		private static string DB_PATH = Path.Combine(Application.get_temporaryCachePath(), "amplitude.sqlite");

		public DatabaseHelper()
		{
			using (SQLiteConnection sQLiteConnection = new SQLiteConnection(GetConnectionString()))
			{
				sQLiteConnection.CreateTable<Event>();
			}
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
			using (SQLiteConnection sQLiteConnection = GetConnection())
			{
				try
				{
					Event @event = new Event();
					@event.Text = evt;
					Event event2 = @event;
					if (sQLiteConnection.Insert(event2) == 0)
					{
						Debug.Log((object)"AmplitudeAPI: DatabaseHelper.AddEvent: Insert failed");
						return -1;
					}
					return event2.Id;
					IL_0046:;
				}
				catch (SQLiteException ex)
				{
					Debug.Log((object)"AmplitudeAPI: DatabaseHelper.AddEvent failed");
					Debug.LogException((Exception)ex);
				}
			}
			return -1;
		}

		public int GetEventCount()
		{
			using (SQLiteConnection sQLiteConnection = GetConnection())
			{
				try
				{
					return sQLiteConnection.Table<Event>().Count();
					IL_0018:;
				}
				catch (SQLiteException ex)
				{
					Debug.Log((object)"AmplitudeAPI: DatabaseHelper.GetEventCount: GetNumberRows failed");
					Debug.LogException((Exception)ex);
				}
			}
			return 0;
		}

		public KeyValuePair<int, IEnumerable<Event>> GetEvents(int lessThanId, int limit)
		{
			using (SQLiteConnection sQLiteConnection = GetConnection())
			{
				try
				{
					ParameterExpression parameterExpression = Expression.Parameter(typeof(Event), "e");
					TableQuery<Event> tableQuery = sQLiteConnection.Table<Event>();
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
						return new KeyValuePair<int, IEnumerable<Event>>(list.Last().Id, list);
					}
				}
				catch (SQLiteException ex)
				{
					Debug.Log((object)"AmplitudeAPI: DatabaseHelper.GetEvents failed");
					Debug.LogException((Exception)ex);
				}
			}
			return new KeyValuePair<int, IEnumerable<Event>>(-1, new List<Event>());
		}

		public int GetNthEventId(int n)
		{
			using (SQLiteConnection sQLiteConnection = GetConnection())
			{
				try
				{
					Event @event = (from i in sQLiteConnection.Table<Event>()
					orderby i.Id
					select i).Skip(n - 1).Take(1).First();
					return @event.Id;
					IL_006c:;
				}
				catch (SQLiteException ex)
				{
					Debug.Log((object)"AmplitudeAPI: DatabaseHelper.GetNthEventId failed");
					Debug.LogException((Exception)ex);
				}
			}
			return -1;
		}

		public void RemoveEvents(int maxId)
		{
			using (SQLiteConnection sQLiteConnection = GetConnection())
			{
				try
				{
					sQLiteConnection.Execute("DELETE FROM Event WHERE Id <= ?", maxId);
				}
				catch (SQLiteException ex)
				{
					Debug.Log((object)"AmplitudeAPI: DatabaseHelper.RemoveEvents failed");
					Debug.LogException((Exception)ex);
				}
			}
		}

		public void RemoveEvent(int id)
		{
			using (SQLiteConnection sQLiteConnection = GetConnection())
			{
				try
				{
					sQLiteConnection.Execute("DELETE FROM Event WHERE Id = ?", id);
				}
				catch (SQLiteException ex)
				{
					Debug.Log((object)"AmplitudeAPI: DatabaseHelper.RemoveEvent failed");
					Debug.LogException((Exception)ex);
				}
			}
		}
	}
}
