using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SQLite
{
	public class TableQuery<T> : BaseTableQuery, IEnumerable, IEnumerable<T>
	{
		private class CompileResult
		{
			public string CommandText
			{
				get;
				set;
			}

			public object Value
			{
				get;
				set;
			}
		}

		private Expression _where;

		private List<Ordering> _orderBys;

		private int? _limit;

		private int? _offset;

		private BaseTableQuery _joinInner;

		private Expression _joinInnerKeySelector;

		private BaseTableQuery _joinOuter;

		private Expression _joinOuterKeySelector;

		private Expression _joinSelector;

		private Expression _selector;

		private bool _deferred;

		public SQLiteConnection Connection
		{
			get;
			private set;
		}

		public TableMapping Table
		{
			get;
			private set;
		}

		private TableQuery(SQLiteConnection conn, TableMapping table)
		{
			Connection = conn;
			Table = table;
		}

		public TableQuery(SQLiteConnection conn)
		{
			Connection = conn;
			Table = Connection.GetMapping(typeof(T));
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public TableQuery<U> Clone<U>()
		{
			TableQuery<U> tableQuery = new TableQuery<U>(Connection, Table);
			tableQuery._where = _where;
			tableQuery._deferred = _deferred;
			if (_orderBys != null)
			{
				tableQuery._orderBys = new List<Ordering>(_orderBys);
			}
			tableQuery._limit = _limit;
			tableQuery._offset = _offset;
			tableQuery._joinInner = _joinInner;
			tableQuery._joinInnerKeySelector = _joinInnerKeySelector;
			tableQuery._joinOuter = _joinOuter;
			tableQuery._joinOuterKeySelector = _joinOuterKeySelector;
			tableQuery._joinSelector = _joinSelector;
			tableQuery._selector = _selector;
			return tableQuery;
		}

		public TableQuery<T> Where(Expression<Func<T, bool>> predExpr)
		{
			if (predExpr.NodeType == ExpressionType.Lambda)
			{
				Expression body = predExpr.Body;
				TableQuery<T> tableQuery = Clone<T>();
				tableQuery.AddWhere(body);
				return tableQuery;
			}
			throw new NotSupportedException("Must be a predicate");
		}

		public TableQuery<T> Take(int n)
		{
			TableQuery<T> tableQuery = Clone<T>();
			tableQuery._limit = n;
			return tableQuery;
		}

		public TableQuery<T> Skip(int n)
		{
			TableQuery<T> tableQuery = Clone<T>();
			tableQuery._offset = n;
			return tableQuery;
		}

		public T ElementAt(int index)
		{
			return Skip(index).Take(1).First();
		}

		public TableQuery<T> Deferred()
		{
			TableQuery<T> tableQuery = Clone<T>();
			tableQuery._deferred = true;
			return tableQuery;
		}

		public TableQuery<T> OrderBy<U>(Expression<Func<T, U>> orderExpr)
		{
			return AddOrderBy(orderExpr, asc: true);
		}

		public TableQuery<T> OrderByDescending<U>(Expression<Func<T, U>> orderExpr)
		{
			return AddOrderBy(orderExpr, asc: false);
		}

		public TableQuery<T> ThenBy<U>(Expression<Func<T, U>> orderExpr)
		{
			return AddOrderBy(orderExpr, asc: true);
		}

		public TableQuery<T> ThenByDescending<U>(Expression<Func<T, U>> orderExpr)
		{
			return AddOrderBy(orderExpr, asc: false);
		}

		private TableQuery<T> AddOrderBy<U>(Expression<Func<T, U>> orderExpr, bool asc)
		{
			if (orderExpr.NodeType == ExpressionType.Lambda)
			{
				MemberExpression memberExpression = null;
				UnaryExpression unaryExpression = orderExpr.Body as UnaryExpression;
				memberExpression = ((unaryExpression == null || unaryExpression.NodeType != ExpressionType.Convert) ? (orderExpr.Body as MemberExpression) : (unaryExpression.Operand as MemberExpression));
				if (memberExpression != null && memberExpression.Expression.NodeType == ExpressionType.Parameter)
				{
					TableQuery<T> tableQuery = Clone<T>();
					if (tableQuery._orderBys == null)
					{
						tableQuery._orderBys = new List<Ordering>();
					}
					tableQuery._orderBys.Add(new Ordering
					{
						ColumnName = Table.FindColumnWithPropertyName(memberExpression.Member.Name).Name,
						Ascending = asc
					});
					return tableQuery;
				}
				throw new NotSupportedException("Order By does not support: " + orderExpr);
			}
			throw new NotSupportedException("Must be a predicate");
		}

		private void AddWhere(Expression pred)
		{
			if (_where == null)
			{
				_where = pred;
			}
			else
			{
				_where = Expression.AndAlso(_where, pred);
			}
		}

		public TableQuery<TResult> Join<TInner, TKey, TResult>(TableQuery<TInner> inner, Expression<Func<T, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<T, TInner, TResult>> resultSelector)
		{
			TableQuery<TResult> tableQuery = new TableQuery<TResult>(Connection, Connection.GetMapping(typeof(TResult)));
			tableQuery._joinOuter = this;
			tableQuery._joinOuterKeySelector = outerKeySelector;
			tableQuery._joinInner = inner;
			tableQuery._joinInnerKeySelector = innerKeySelector;
			tableQuery._joinSelector = resultSelector;
			return tableQuery;
		}

		public TableQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
		{
			TableQuery<TResult> tableQuery = Clone<TResult>();
			tableQuery._selector = selector;
			return tableQuery;
		}

		private SQLiteCommand GenerateCommand(string selectionList)
		{
			if (_joinInner != null && _joinOuter != null)
			{
				throw new NotSupportedException("Joins are not supported.");
			}
			string text = "select " + selectionList + " from \"" + Table.TableName + "\"";
			List<object> list = new List<object>();
			if (_where != null)
			{
				CompileResult compileResult = CompileExpr(_where, list);
				text = text + " where " + compileResult.CommandText;
			}
			if (_orderBys != null && _orderBys.Count > 0)
			{
				string str = string.Join(", ", (from o in _orderBys
				select "\"" + o.ColumnName + "\"" + ((!o.Ascending) ? " desc" : string.Empty)).ToArray());
				text = text + " order by " + str;
			}
			if (_limit.HasValue)
			{
				text = text + " limit " + _limit.Value;
			}
			if (_offset.HasValue)
			{
				if (!_limit.HasValue)
				{
					text += " limit -1 ";
				}
				text = text + " offset " + _offset.Value;
			}
			return Connection.CreateCommand(text, list.ToArray());
		}

		private CompileResult CompileExpr(Expression expr, List<object> queryArgs)
		{
			if (expr == null)
			{
				throw new NotSupportedException("Expression is NULL");
			}
			if (expr is BinaryExpression)
			{
				BinaryExpression binaryExpression = (BinaryExpression)expr;
				CompileResult compileResult = CompileExpr(binaryExpression.Left, queryArgs);
				CompileResult compileResult2 = CompileExpr(binaryExpression.Right, queryArgs);
				string commandText = (compileResult.CommandText == "?" && compileResult.Value == null) ? CompileNullBinaryExpression(binaryExpression, compileResult2) : ((!(compileResult2.CommandText == "?") || compileResult2.Value != null) ? ("(" + compileResult.CommandText + " " + GetSqlName(binaryExpression) + " " + compileResult2.CommandText + ")") : CompileNullBinaryExpression(binaryExpression, compileResult));
				CompileResult compileResult3 = new CompileResult();
				compileResult3.CommandText = commandText;
				return compileResult3;
			}
			if (expr.NodeType == ExpressionType.Call)
			{
				MethodCallExpression methodCallExpression = (MethodCallExpression)expr;
				CompileResult[] array = new CompileResult[methodCallExpression.Arguments.Count];
				CompileResult compileResult4 = (methodCallExpression.Object == null) ? null : CompileExpr(methodCallExpression.Object, queryArgs);
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = CompileExpr(methodCallExpression.Arguments[i], queryArgs);
				}
				string empty = string.Empty;
				empty = ((methodCallExpression.Method.Name == "Like" && array.Length == 2) ? ("(" + array[0].CommandText + " like " + array[1].CommandText + ")") : ((methodCallExpression.Method.Name == "Contains" && array.Length == 2) ? ("(" + array[1].CommandText + " in " + array[0].CommandText + ")") : ((methodCallExpression.Method.Name == "Contains" && array.Length == 1) ? ((methodCallExpression.Object == null || methodCallExpression.Object.Type != typeof(string)) ? ("(" + array[0].CommandText + " in " + compileResult4.CommandText + ")") : ("(" + compileResult4.CommandText + " like ('%' || " + array[0].CommandText + " || '%'))")) : ((methodCallExpression.Method.Name == "StartsWith" && array.Length == 1) ? ("(" + compileResult4.CommandText + " like (" + array[0].CommandText + " || '%'))") : ((methodCallExpression.Method.Name == "EndsWith" && array.Length == 1) ? ("(" + compileResult4.CommandText + " like ('%' || " + array[0].CommandText + "))") : ((methodCallExpression.Method.Name == "Equals" && array.Length == 1) ? ("(" + compileResult4.CommandText + " = (" + array[0].CommandText + "))") : ((methodCallExpression.Method.Name == "ToLower") ? ("(lower(" + compileResult4.CommandText + "))") : ((!(methodCallExpression.Method.Name == "ToUpper")) ? (methodCallExpression.Method.Name.ToLower() + "(" + string.Join(",", (from a in array
				select a.CommandText).ToArray()) + ")") : ("(upper(" + compileResult4.CommandText + "))")))))))));
				CompileResult compileResult3 = new CompileResult();
				compileResult3.CommandText = empty;
				return compileResult3;
			}
			if (expr.NodeType == ExpressionType.Constant)
			{
				ConstantExpression constantExpression = (ConstantExpression)expr;
				queryArgs.Add(constantExpression.Value);
				CompileResult compileResult3 = new CompileResult();
				compileResult3.CommandText = "?";
				compileResult3.Value = constantExpression.Value;
				return compileResult3;
			}
			if (expr.NodeType == ExpressionType.Convert)
			{
				UnaryExpression unaryExpression = (UnaryExpression)expr;
				Type type = unaryExpression.Type;
				CompileResult compileResult5 = CompileExpr(unaryExpression.Operand, queryArgs);
				CompileResult compileResult3 = new CompileResult();
				compileResult3.CommandText = compileResult5.CommandText;
				compileResult3.Value = ((compileResult5.Value == null) ? null : ConvertTo(compileResult5.Value, type));
				return compileResult3;
			}
			if (expr.NodeType == ExpressionType.Not)
			{
				UnaryExpression unaryExpression2 = (UnaryExpression)expr;
				Type type2 = unaryExpression2.Type;
				CompileResult compileResult6 = CompileExpr(unaryExpression2.Operand, queryArgs);
				CompileResult compileResult3 = new CompileResult();
				compileResult3.CommandText = "NOT " + compileResult6.CommandText;
				compileResult3.Value = ((compileResult6.Value == null) ? null : compileResult6.Value);
				return compileResult3;
			}
			if (expr.NodeType == ExpressionType.MemberAccess)
			{
				MemberExpression memberExpression = (MemberExpression)expr;
				CompileResult compileResult3;
				if (memberExpression.Expression != null && memberExpression.Expression.NodeType == ExpressionType.Parameter)
				{
					string name = Table.FindColumnWithPropertyName(memberExpression.Member.Name).Name;
					compileResult3 = new CompileResult();
					compileResult3.CommandText = "\"" + name + "\"";
					return compileResult3;
				}
				object obj = null;
				if (memberExpression.Expression != null)
				{
					CompileResult compileResult7 = CompileExpr(memberExpression.Expression, queryArgs);
					if (compileResult7.Value == null)
					{
						throw new NotSupportedException("Member access failed to compile expression");
					}
					if (compileResult7.CommandText == "?")
					{
						queryArgs.RemoveAt(queryArgs.Count - 1);
					}
					obj = compileResult7.Value;
				}
				object obj2 = null;
				if (memberExpression.Member.MemberType == MemberTypes.Property)
				{
					PropertyInfo propertyInfo = (PropertyInfo)memberExpression.Member;
					obj2 = propertyInfo.GetGetMethod().Invoke(obj, null);
				}
				else
				{
					if (memberExpression.Member.MemberType != MemberTypes.Field)
					{
						throw new NotSupportedException("MemberExpr: " + memberExpression.Member.MemberType);
					}
					FieldInfo fieldInfo = (FieldInfo)memberExpression.Member;
					obj2 = fieldInfo.GetValue(obj);
				}
				if (obj2 != null && obj2 is IEnumerable && !(obj2 is string) && !(obj2 is IEnumerable<byte>))
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("(");
					string value = string.Empty;
					foreach (object item in (IEnumerable)obj2)
					{
						queryArgs.Add(item);
						stringBuilder.Append(value);
						stringBuilder.Append("?");
						value = ",";
					}
					stringBuilder.Append(")");
					compileResult3 = new CompileResult();
					compileResult3.CommandText = stringBuilder.ToString();
					compileResult3.Value = obj2;
					return compileResult3;
				}
				queryArgs.Add(obj2);
				compileResult3 = new CompileResult();
				compileResult3.CommandText = "?";
				compileResult3.Value = obj2;
				return compileResult3;
			}
			throw new NotSupportedException("Cannot compile: " + expr.NodeType.ToString());
		}

		private static object ConvertTo(object obj, Type t)
		{
			Type underlyingType = Nullable.GetUnderlyingType(t);
			if (underlyingType != null)
			{
				if (obj == null)
				{
					return null;
				}
				return Convert.ChangeType(obj, underlyingType);
			}
			return Convert.ChangeType(obj, t);
		}

		private string CompileNullBinaryExpression(BinaryExpression expression, CompileResult parameter)
		{
			if (expression.NodeType == ExpressionType.Equal)
			{
				return "(" + parameter.CommandText + " is ?)";
			}
			if (expression.NodeType == ExpressionType.NotEqual)
			{
				return "(" + parameter.CommandText + " is not ?)";
			}
			throw new NotSupportedException("Cannot compile Null-BinaryExpression with type " + expression.NodeType.ToString());
		}

		private string GetSqlName(Expression expr)
		{
			ExpressionType nodeType = expr.NodeType;
			switch (nodeType)
			{
			case ExpressionType.GreaterThan:
				return ">";
			case ExpressionType.GreaterThanOrEqual:
				return ">=";
			case ExpressionType.LessThan:
				return "<";
			case ExpressionType.LessThanOrEqual:
				return "<=";
			case ExpressionType.And:
				return "&";
			case ExpressionType.AndAlso:
				return "and";
			case ExpressionType.Or:
				return "|";
			case ExpressionType.OrElse:
				return "or";
			case ExpressionType.Equal:
				return "=";
			case ExpressionType.NotEqual:
				return "!=";
			default:
				throw new NotSupportedException("Cannot get SQL for: " + nodeType);
			}
		}

		public int Count()
		{
			return GenerateCommand("count(*)").ExecuteScalar<int>();
		}

		public int Count(Expression<Func<T, bool>> predExpr)
		{
			return Where(predExpr).Count();
		}

		public IEnumerator<T> GetEnumerator()
		{
			if (!_deferred)
			{
				return GenerateCommand("*").ExecuteQuery<T>().GetEnumerator();
			}
			return GenerateCommand("*").ExecuteDeferredQuery<T>().GetEnumerator();
		}

		public T First()
		{
			TableQuery<T> source = Take(1);
			return source.ToList().First();
		}

		public T FirstOrDefault()
		{
			TableQuery<T> source = Take(1);
			return source.ToList().FirstOrDefault();
		}
	}
}
