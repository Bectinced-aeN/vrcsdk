namespace Amazon.Auth.AccessControlPolicy
{
	public class ActionIdentifier
	{
		private string actionName;

		public string ActionName
		{
			get
			{
				return actionName;
			}
			set
			{
				actionName = value;
			}
		}

		public ActionIdentifier(string actionName)
		{
			this.actionName = actionName;
		}
	}
}
