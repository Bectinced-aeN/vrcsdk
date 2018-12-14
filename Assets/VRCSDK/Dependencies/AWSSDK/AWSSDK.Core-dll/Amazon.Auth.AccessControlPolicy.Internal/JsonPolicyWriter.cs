using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ThirdParty.Json.LitJson;

namespace Amazon.Auth.AccessControlPolicy.Internal
{
	internal static class JsonPolicyWriter
	{
		public static string WritePolicyToString(bool prettyPrint, Policy policy)
		{
			if (policy != null)
			{
				StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
				try
				{
					JsonWriter jsonWriter = new JsonWriter(stringWriter);
					jsonWriter.IndentValue = 4;
					jsonWriter.PrettyPrint = prettyPrint;
					writePolicy(policy, jsonWriter);
					return stringWriter.ToString().Trim();
				}
				catch (Exception ex)
				{
					throw new ArgumentException("Unable to serialize policy to JSON string: " + ex.Message, ex);
				}
			}
			throw new ArgumentNullException("policy");
		}

		private static void writePolicy(Policy policy, JsonWriter generator)
		{
			generator.WriteObjectStart();
			writePropertyValue(generator, "Version", policy.Version);
			if (policy.Id != null)
			{
				writePropertyValue(generator, "Id", policy.Id);
			}
			generator.WritePropertyName("Statement");
			generator.WriteArrayStart();
			foreach (Statement statement in policy.Statements)
			{
				generator.WriteObjectStart();
				if (statement.Id != null)
				{
					writePropertyValue(generator, "Sid", statement.Id);
				}
				writePropertyValue(generator, "Effect", statement.Effect.ToString());
				writePrincipals(statement, generator);
				writeActions(statement, generator);
				writeResources(statement, generator);
				writeConditions(statement, generator);
				generator.WriteObjectEnd();
			}
			generator.WriteArrayEnd();
			generator.WriteObjectEnd();
		}

		private static void writePrincipals(Statement statement, JsonWriter generator)
		{
			IList<Principal> principals = statement.Principals;
			if (principals != null && principals.Count != 0)
			{
				generator.WritePropertyName("Principal");
				if (principals.Count == 1 && principals[0] != null && principals[0].Provider.Equals("__ANONYMOUS__", StringComparison.Ordinal))
				{
					generator.Write("*");
				}
				else
				{
					generator.WriteObjectStart();
					Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
					foreach (Principal item in principals)
					{
						if (!dictionary.TryGetValue(item.Provider, out List<string> value))
						{
							value = new List<string>();
							dictionary[item.Provider] = value;
						}
						value.Add(item.Id);
					}
					foreach (string key in dictionary.Keys)
					{
						generator.WritePropertyName(key);
						if (dictionary[key].Count > 1)
						{
							generator.WriteArrayStart();
						}
						foreach (string item2 in dictionary[key])
						{
							generator.Write(item2);
						}
						if (dictionary[key].Count > 1)
						{
							generator.WriteArrayEnd();
						}
					}
					generator.WriteObjectEnd();
				}
			}
		}

		private static void writeActions(Statement statement, JsonWriter generator)
		{
			IList<ActionIdentifier> actions = statement.Actions;
			if (actions != null && actions.Count != 0)
			{
				generator.WritePropertyName("Action");
				if (actions.Count > 1)
				{
					generator.WriteArrayStart();
				}
				foreach (ActionIdentifier item in actions)
				{
					generator.Write(item.ActionName);
				}
				if (actions.Count > 1)
				{
					generator.WriteArrayEnd();
				}
			}
		}

		private static void writeResources(Statement statement, JsonWriter generator)
		{
			IList<Resource> resources = statement.Resources;
			if (resources != null && resources.Count != 0)
			{
				generator.WritePropertyName("Resource");
				if (resources.Count > 1)
				{
					generator.WriteArrayStart();
				}
				foreach (Resource item in resources)
				{
					generator.Write(item.Id);
				}
				if (resources.Count > 1)
				{
					generator.WriteArrayEnd();
				}
			}
		}

		private static void writeConditions(Statement statement, JsonWriter generator)
		{
			IList<Condition> conditions = statement.Conditions;
			if (conditions != null && conditions.Count != 0)
			{
				Dictionary<string, Dictionary<string, List<string>>> dictionary = sortConditionsByTypeAndKey(conditions);
				generator.WritePropertyName("Condition");
				generator.WriteObjectStart();
				foreach (KeyValuePair<string, Dictionary<string, List<string>>> item in dictionary)
				{
					generator.WritePropertyName(item.Key);
					generator.WriteObjectStart();
					foreach (KeyValuePair<string, List<string>> item2 in item.Value)
					{
						IList<string> value = item2.Value;
						if (value.Count != 0)
						{
							generator.WritePropertyName(item2.Key);
							if (value.Count > 1)
							{
								generator.WriteArrayStart();
							}
							if (value != null && value.Count != 0)
							{
								foreach (string item3 in value)
								{
									generator.Write(item3);
								}
							}
							if (value.Count > 1)
							{
								generator.WriteArrayEnd();
							}
						}
					}
					generator.WriteObjectEnd();
				}
				generator.WriteObjectEnd();
			}
		}

		private static Dictionary<string, Dictionary<string, List<string>>> sortConditionsByTypeAndKey(IList<Condition> conditions)
		{
			Dictionary<string, Dictionary<string, List<string>>> dictionary = new Dictionary<string, Dictionary<string, List<string>>>();
			foreach (Condition condition in conditions)
			{
				string type = condition.Type;
				string conditionKey = condition.ConditionKey;
				if (!dictionary.TryGetValue(type, out Dictionary<string, List<string>> value))
				{
					value = (dictionary[type] = new Dictionary<string, List<string>>());
				}
				if (!value.TryGetValue(conditionKey, out List<string> value2))
				{
					value2 = (value[conditionKey] = new List<string>());
				}
				if (condition.Values != null)
				{
					string[] values = condition.Values;
					foreach (string item in values)
					{
						value2.Add(item);
					}
				}
			}
			return dictionary;
		}

		private static void writePropertyValue(JsonWriter generator, string propertyName, string value)
		{
			generator.WritePropertyName(propertyName);
			generator.Write(value);
		}
	}
}
