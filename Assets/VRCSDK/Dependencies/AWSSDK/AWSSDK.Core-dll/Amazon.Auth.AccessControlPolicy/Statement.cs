using System;
using System.Collections.Generic;

namespace Amazon.Auth.AccessControlPolicy
{
	public class Statement
	{
		public enum StatementEffect
		{
			Allow,
			Deny
		}

		private string id;

		private StatementEffect effect;

		private IList<Principal> principals = new List<Principal>();

		private IList<ActionIdentifier> actions = new List<ActionIdentifier>();

		private IList<Resource> resources = new List<Resource>();

		private IList<Condition> conditions = new List<Condition>();

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

		public StatementEffect Effect
		{
			get
			{
				return effect;
			}
			set
			{
				effect = value;
			}
		}

		public IList<ActionIdentifier> Actions
		{
			get
			{
				return actions;
			}
			set
			{
				actions = value;
			}
		}

		public IList<Resource> Resources
		{
			get
			{
				return resources;
			}
			set
			{
				resources = value;
			}
		}

		public IList<Condition> Conditions
		{
			get
			{
				return conditions;
			}
			set
			{
				conditions = value;
			}
		}

		public IList<Principal> Principals
		{
			get
			{
				return principals;
			}
			set
			{
				principals = value;
			}
		}

		public Statement(StatementEffect effect)
		{
			this.effect = effect;
			id = Guid.NewGuid().ToString().Replace("-", "");
		}

		public Statement WithId(string id)
		{
			Id = id;
			return this;
		}

		public Statement WithActionIdentifiers(params ActionIdentifier[] actions)
		{
			if (this.actions == null)
			{
				this.actions = new List<ActionIdentifier>();
			}
			foreach (ActionIdentifier item in actions)
			{
				this.actions.Add(item);
			}
			return this;
		}

		public Statement WithResources(params Resource[] resources)
		{
			if (this.resources == null)
			{
				this.resources = new List<Resource>();
			}
			foreach (Resource item in resources)
			{
				this.resources.Add(item);
			}
			return this;
		}

		public Statement WithConditions(params Condition[] conditions)
		{
			if (Conditions == null)
			{
				Conditions = new List<Condition>();
			}
			foreach (Condition item in conditions)
			{
				Conditions.Add(item);
			}
			return this;
		}

		public Statement WithPrincipals(params Principal[] principals)
		{
			if (this.principals == null)
			{
				this.principals = new List<Principal>();
			}
			foreach (Principal item in principals)
			{
				this.principals.Add(item);
			}
			return this;
		}
	}
}
