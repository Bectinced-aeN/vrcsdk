using UnityEngine;

public class VRC_VisualDamage : MonoBehaviour
{
	[Tooltip("the scale of the damage indicator when at full damage")]
	public float fullDamageScale = 1f;

	[Tooltip("the scale of the damage indicator when at minimum damage")]
	public float minDamageScale = 2f;

	[Tooltip("the distance in front of the player's head for the damage indicator")]
	public float offset = 0.5f;

	private AnimationCurve curve = new AnimationCurve();

	private float currentTime;

	private Renderer renderer;

	private Vector3 initScale;

	public VRC_VisualDamage()
		: this()
	{
	}//IL_0022: Unknown result type (might be due to invalid IL or missing references)
	//IL_002c: Expected O, but got Unknown


	private void Awake()
	{
		renderer = this.GetComponent<Renderer>();
	}

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		initScale = this.get_transform().get_localScale();
		curve.AddKey(new Keyframe(0f, fullDamageScale));
		curve.AddKey(new Keyframe(1f, minDamageScale));
	}

	private void OnSceneWasLoaded()
	{
		Object.Destroy(this.get_gameObject());
	}

	private void Update()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (renderer != null && curve != null && renderer.get_enabled())
		{
			if (currentTime < 1f)
			{
				currentTime += Time.get_deltaTime();
			}
			this.get_transform().set_localScale(initScale * curve.Evaluate(currentTime));
		}
	}

	private void HideDamageIndicator()
	{
		if (renderer != null)
		{
			renderer.set_enabled(false);
		}
	}

	private void ShowDamageIndicator()
	{
		if (renderer != null)
		{
			renderer.set_enabled(true);
		}
	}

	public void SetDamagePercent(float damagePercent)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (damagePercent > 0f)
		{
			ShowDamageIndicator();
			currentTime = 0f;
			float num = fullDamageScale - minDamageScale;
			float num2 = minDamageScale + num * damagePercent;
			curve.MoveKey(1, new Keyframe(1f, num2));
		}
		else
		{
			HideDamageIndicator();
		}
	}
}
