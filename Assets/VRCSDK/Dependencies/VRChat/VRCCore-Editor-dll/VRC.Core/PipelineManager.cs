using System;
using UnityEngine;

namespace VRC.Core
{
	public class PipelineManager : MonoBehaviour
	{
		public enum ContentType
		{
			avatar,
			world
		}

		private enum Ownership
		{
			Uninitialized,
			Querried,
			Owned,
			Unowned
		}

		[HideInInspector]
		public bool launchedFromSDKPipeline;

		[HideInInspector]
		public bool completedSDKPipeline;

		[HideInInspector]
		public string blueprintId;

		[HideInInspector]
		public APIUser user;

		public ContentType contentType;

		[Obsolete("Property is not used.")]
		[HideInInspector]
		public string assetBundleUnityVersion;

		private Ownership owned;

		public PipelineManager()
			: this()
		{
		}

		public bool IsMyContent(string testId)
		{
			switch (owned)
			{
			case Ownership.Uninitialized:
				owned = Ownership.Querried;
				if (contentType == ContentType.world)
				{
					API.Fetch<ApiWorld>(testId, delegate(ApiContainer c)
					{
						ApiWorld apiWorld = c.Model as ApiWorld;
						if (apiWorld.authorId == user.id)
						{
							owned = Ownership.Owned;
						}
						else
						{
							owned = Ownership.Unowned;
						}
					}, delegate
					{
						owned = Ownership.Unowned;
					});
				}
				else
				{
					API.Fetch<ApiAvatar>(testId, delegate(ApiContainer c)
					{
						ApiAvatar apiAvatar = c.Model as ApiAvatar;
						if (apiAvatar.authorId == user.id)
						{
							owned = Ownership.Owned;
						}
						else
						{
							owned = Ownership.Unowned;
						}
					}, delegate
					{
						owned = Ownership.Unowned;
					});
				}
				return false;
			case Ownership.Querried:
				return false;
			case Ownership.Owned:
				return true;
			case Ownership.Unowned:
				return false;
			default:
				return false;
			}
		}

		protected void Start()
		{
			if (Application.get_isEditor() && this.GetComponent<PipelineSaver>() != null)
			{
				GameObject val = null;
				switch (contentType)
				{
				case ContentType.world:
				{
					Object val3 = Resources.Load("VRCSDKWorld", typeof(GameObject));
					if (!(val3 != null))
					{
						Debug.LogError((object)"Could not load VRCSDKWorld");
						return;
					}
					val = (Object.Instantiate(val3) as GameObject);
					break;
				}
				case ContentType.avatar:
				{
					Object val2 = Resources.Load("VRCSDKAvatar", typeof(GameObject));
					if (!(val2 != null))
					{
						Debug.LogError((object)"Could not load VRCSDKAvatar");
						return;
					}
					val = (Object.Instantiate(val2) as GameObject);
					break;
				}
				default:
					Debug.LogError((object)"Pipeline Manager is not set to supported content type (avatar or world)");
					break;
				}
				val.set_name("VRCSDK");
				Object val4 = Resources.Load("VRCCam", typeof(GameObject));
				if (val4 != null)
				{
					GameObject val5 = Object.Instantiate(val4) as GameObject;
					val5.set_name("VRCCam");
				}
				else
				{
					Debug.LogError((object)"Could not load VRCCam");
				}
			}
		}

		public void Cleanup()
		{
			Debug.Log((object)"PipelineManager.Cleanup");
			Object.DestroyImmediate(GameObject.Find("VRCSDK"));
			Object.DestroyImmediate(GameObject.Find("VRCCam"));
			Object.DestroyImmediate(GameObject.Find("UploadManager"));
			launchedFromSDKPipeline = false;
		}

		public void AssignId()
		{
			if (contentType == ContentType.avatar)
			{
				blueprintId = "avtr_" + Guid.NewGuid().ToString();
			}
			else if (contentType == ContentType.world)
			{
				blueprintId = "wrld_" + Guid.NewGuid().ToString();
			}
			else
			{
				Debug.LogError((object)"Assigning PipelineManager asset ID without content type");
			}
			completedSDKPipeline = false;
		}
	}
}
