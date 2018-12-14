using System.Collections;
using System.Collections.Generic;
using ThirdParty.Json.LitJson;

namespace Amazon.Auth.AccessControlPolicy.Internal
{
	internal static class JsonPolicyReader
	{
		public static Policy ReadJsonStringToPolicy(string jsonString)
		{
			Policy policy = new Policy();
			JsonData jsonData = JsonMapper.ToObject(jsonString);
			if (jsonData["Id"] != null && jsonData["Id"].IsString)
			{
				policy.Id = (string)jsonData["Id"];
			}
			JsonData jsonData2 = jsonData["Statement"];
			if (jsonData2 != null && jsonData2.IsArray)
			{
				foreach (JsonData item in (IEnumerable)jsonData2)
				{
					Statement statement = convertStatement(item);
					if (statement != null)
					{
						policy.Statements.Add(statement);
					}
				}
				return policy;
			}
			return policy;
		}

		private static Statement convertStatement(JsonData jStatement)
		{
			if (jStatement["Effect"] == null || !jStatement["Effect"].IsString)
			{
				return null;
			}
			string value = (string)jStatement["Effect"];
			Statement.StatementEffect effect = (!"Allow".Equals(value)) ? Statement.StatementEffect.Deny : Statement.StatementEffect.Allow;
			Statement statement = new Statement(effect);
			if (jStatement["Sid"] != null && jStatement["Sid"].IsString)
			{
				statement.Id = (string)jStatement["Sid"];
			}
			convertActions(statement, jStatement);
			convertResources(statement, jStatement);
			convertCondition(statement, jStatement);
			convertPrincipals(statement, jStatement);
			return statement;
		}

		private static void convertPrincipals(Statement statement, JsonData jStatement)
		{
			JsonData jsonData = jStatement["Principal"];
			if (jsonData != null)
			{
				if (jsonData.IsObject)
				{
					convertPrincipalRecord(statement, jsonData);
				}
				else if (jsonData.IsArray)
				{
					foreach (JsonData item in (IEnumerable)jsonData)
					{
						convertPrincipalRecord(statement, item);
					}
				}
				else if (jsonData.IsString && jsonData.Equals("*"))
				{
					statement.Principals.Add(Principal.Anonymous);
				}
			}
		}

		private static void convertPrincipalRecord(Statement statement, JsonData jPrincipal)
		{
			foreach (KeyValuePair<string, JsonData> item3 in (IEnumerable)jPrincipal)
			{
				if (item3.Value != null)
				{
					if (item3.Value.IsArray)
					{
						foreach (JsonData item4 in (IEnumerable)item3.Value)
						{
							if (item4.IsString)
							{
								Principal item = new Principal(item3.Key, (string)item4, stripHyphen: false);
								statement.Principals.Add(item);
							}
						}
					}
					else if (item3.Value.IsString)
					{
						Principal item2 = new Principal(item3.Key, (string)item3.Value, stripHyphen: false);
						statement.Principals.Add(item2);
					}
				}
			}
		}

		private static void convertActions(Statement statement, JsonData jStatement)
		{
			JsonData jsonData = jStatement["Action"];
			if (jsonData != null)
			{
				if (jsonData.IsString)
				{
					statement.Actions.Add(new ActionIdentifier((string)jsonData));
				}
				else if (jsonData.IsArray)
				{
					foreach (JsonData item in (IEnumerable)jsonData)
					{
						if (item.IsString)
						{
							statement.Actions.Add(new ActionIdentifier((string)item));
						}
					}
				}
			}
		}

		private static void convertResources(Statement statement, JsonData jStatement)
		{
			JsonData jsonData = jStatement["Resource"];
			if (jsonData != null)
			{
				if (jsonData.IsString)
				{
					statement.Resources.Add(new Resource((string)jsonData));
				}
				else if (jsonData.IsArray)
				{
					foreach (JsonData item in (IEnumerable)jsonData)
					{
						if (item.IsString)
						{
							statement.Resources.Add(new Resource((string)item));
						}
					}
				}
			}
		}

		private static void convertCondition(Statement statement, JsonData jStatement)
		{
			JsonData jsonData = jStatement["Condition"];
			if (jsonData != null)
			{
				if (jsonData.IsObject)
				{
					convertConditionRecord(statement, jsonData);
				}
				else if (jsonData.IsArray)
				{
					foreach (JsonData item in (IEnumerable)jsonData)
					{
						convertConditionRecord(statement, item);
					}
				}
			}
		}

		private static void convertConditionRecord(Statement statement, JsonData jCondition)
		{
			foreach (KeyValuePair<string, JsonData> item2 in (IEnumerable)jCondition)
			{
				string key = item2.Key;
				foreach (KeyValuePair<string, JsonData> item3 in (IEnumerable)item2.Value)
				{
					string key2 = item3.Key;
					List<string> list = new List<string>();
					if (item3.Value != null)
					{
						if (item3.Value.IsString)
						{
							list.Add((string)item3.Value);
						}
						else if (item3.Value.IsArray)
						{
							foreach (JsonData item4 in (IEnumerable)item3.Value)
							{
								if (item4.IsString)
								{
									list.Add((string)item4);
								}
							}
						}
					}
					Condition item = new Condition(key, key2, list.ToArray());
					statement.Conditions.Add(item);
				}
			}
		}
	}
}
