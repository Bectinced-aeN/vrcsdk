using UnityEngine;

namespace VRCSDK2
{
	public class VRC_TutorialAreaMarker : MonoBehaviour
	{
		public string Text;

		public float EnableDistance = 3f;

		public float MaxVisibleDistance = 15f;

		public bool DisableWhenPlayerInRange;

		private bool _showingLabel;

		private bool _enabled = true;

		private float _timeStarted = -1f;

		public VRC_TutorialAreaMarker()
			: this()
		{
		}

		public void Enable(bool enable)
		{
			_enabled = enable;
		}

		private void Update()
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			if (_enabled && Networking.LocalPlayer != null)
			{
				if (_timeStarted < 0f)
				{
					_timeStarted = Time.get_unscaledTime();
				}
				if (!(Time.get_unscaledTime() < _timeStarted + 1f))
				{
					Vector3 val = this.get_transform().get_position() - Networking.LocalPlayer.get_transform().get_position();
					float magnitude = val.get_magnitude();
					if (magnitude >= EnableDistance)
					{
						if (magnitude <= MaxVisibleDistance)
						{
							ShowLabel(show: true);
						}
						else
						{
							ShowLabel(show: false);
						}
					}
					else
					{
						ShowLabel(show: false);
						if (DisableWhenPlayerInRange)
						{
							Enable(enable: false);
						}
					}
				}
			}
			else
			{
				ShowLabel(show: false);
			}
		}

		private void ShowLabel(bool show)
		{
			if (show != _showingLabel)
			{
				if (show && _enabled && !string.IsNullOrEmpty(Text))
				{
					Tutorial.ActivateAreaMarkerLabel(this.get_transform(), Text);
					_showingLabel = true;
				}
				else
				{
					if (_showingLabel)
					{
						Tutorial.DeactivateObjectLabel(this.get_transform());
					}
					_showingLabel = false;
				}
			}
		}
	}
}
