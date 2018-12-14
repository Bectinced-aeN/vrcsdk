using Amazon.Auth.AccessControlPolicy.Internal;
using System.Collections.Generic;
using System.Linq;

namespace Amazon.Auth.AccessControlPolicy
{
	public class Policy
	{
		private const string DEFAULT_POLICY_VERSION = "2012-10-17";

		private string id;

		private string version = "2012-10-17";

		private IList<Statement> statements = new List<Statement>();

		public string Id
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		public string Version
		{
			get
			{
				return version;
			}
			set
			{
				version = value;
			}
		}

		public IList<Statement> Statements
		{
			get
			{
				return statements;
			}
			set
			{
				statements = value;
			}
		}

		public Policy()
		{
		}

		public Policy(string id)
		{
			this.id = id;
		}

		public Policy(string id, IList<Statement> statements)
		{
			this.id = id;
			this.statements = statements;
		}

		public Policy WithId(string id)
		{
			Id = id;
			return this;
		}

		public bool CheckIfStatementExists(Statement statement)
		{
			if (Statements == null)
			{
				return false;
			}
			foreach (Statement statement2 in Statements)
			{
				if (statement2.Effect == statement.Effect && StatementContainsResources(statement2, statement.Resources) && StatementContainsActions(statement2, statement.Actions) && StatementContainsConditions(statement2, statement.Conditions) && StatementContainsPrincipals(statement2, statement.Principals))
				{
					return true;
				}
			}
			return false;
		}

		private static bool StatementContainsResources(Statement statement, IList<Resource> resources)
		{
			foreach (Resource resource in resources)
			{
				if (statement.Resources.FirstOrDefault((Resource x) => string.Equals(x.Id, resource.Id)) == null)
				{
					return false;
				}
			}
			return true;
		}

		private static bool StatementContainsActions(Statement statement, IList<ActionIdentifier> actions)
		{
			foreach (ActionIdentifier action in actions)
			{
				if (statement.Actions.FirstOrDefault((ActionIdentifier x) => string.Equals(x.ActionName, action.ActionName)) == null)
				{
					return false;
				}
			}
			return true;
		}

		private static bool StatementContainsConditions(Statement statement, IList<Condition> conditions)
		{
			foreach (Condition condition in conditions)
			{
				if (statement.Conditions.FirstOrDefault(delegate(Condition x)
				{
					if (string.Equals(x.Type, condition.Type) && string.Equals(x.ConditionKey, condition.ConditionKey))
					{
						return x.Values.Intersect(condition.Values).Count() == condition.Values.Count();
					}
					return false;
				}) == null)
				{
					return false;
				}
			}
			return true;
		}

		private static bool StatementContainsPrincipals(Statement statement, IList<Principal> principals)
		{
			foreach (Principal principal in principals)
			{
				if (statement.Principals.FirstOrDefault(delegate(Principal x)
				{
					if (string.Equals(x.Id, principal.Id))
					{
						return string.Equals(x.Provider, principal.Provider);
					}
					return false;
				}) == null)
				{
					return false;
				}
			}
			return true;
		}

		public Policy WithStatements(params Statement[] statements)
		{
			if (Statements == null)
			{
				Statements = new List<Statement>();
			}
			foreach (Statement item in statements)
			{
				Statements.Add(item);
			}
			return this;
		}

		public string ToJson()
		{
			return ToJson(prettyPrint: true);
		}

		public string ToJson(bool prettyPrint)
		{
			return JsonPolicyWriter.WritePolicyToString(prettyPrint, this);
		}

		public static Policy FromJson(string json)
		{
			return JsonPolicyReader.ReadJsonStringToPolicy(json);
		}
	}
}
