namespace Amazon.Auth.AccessControlPolicy.ActionIdentifiers
{
	public static class ElasticTranscoderActionIdentifiers
	{
		public static readonly ActionIdentifier AllElasticTranscoderActions = new ActionIdentifier("elastictranscoder:*");

		public static readonly ActionIdentifier CancelJob = new ActionIdentifier("elastictranscoder:CancelJob");

		public static readonly ActionIdentifier CreateJob = new ActionIdentifier("elastictranscoder:CreateJob");

		public static readonly ActionIdentifier CreatePipeline = new ActionIdentifier("elastictranscoder:CreatePipeline");

		public static readonly ActionIdentifier CreatePreset = new ActionIdentifier("elastictranscoder:CreatePreset");

		public static readonly ActionIdentifier DeletePipeline = new ActionIdentifier("elastictranscoder:DeletePipeline");

		public static readonly ActionIdentifier DeletePreset = new ActionIdentifier("elastictranscoder:DeletePreset");

		public static readonly ActionIdentifier ListJobsByPipeline = new ActionIdentifier("elastictranscoder:ListJobsByPipeline");

		public static readonly ActionIdentifier ListJobsByStatus = new ActionIdentifier("elastictranscoder:ListJobsByStatus");

		public static readonly ActionIdentifier ListPipelines = new ActionIdentifier("elastictranscoder:ListPipelines");

		public static readonly ActionIdentifier ListPresets = new ActionIdentifier("elastictranscoder:ListPresets");

		public static readonly ActionIdentifier ReadJob = new ActionIdentifier("elastictranscoder:ReadJob");

		public static readonly ActionIdentifier ReadPipeline = new ActionIdentifier("elastictranscoder:ReadPipeline");

		public static readonly ActionIdentifier ReadPreset = new ActionIdentifier("elastictranscoder:ReadPreset");

		public static readonly ActionIdentifier TestRole = new ActionIdentifier("elastictranscoder:TestRole");

		public static readonly ActionIdentifier UpdatePipeline = new ActionIdentifier("elastictranscoder:UpdatePipeline");

		public static readonly ActionIdentifier UpdatePipelineNotifications = new ActionIdentifier("elastictranscoder:UpdatePipelineNotifications");

		public static readonly ActionIdentifier UpdatePipelineStatus = new ActionIdentifier("elastictranscoder:UpdatePipelineStatus");
	}
}
