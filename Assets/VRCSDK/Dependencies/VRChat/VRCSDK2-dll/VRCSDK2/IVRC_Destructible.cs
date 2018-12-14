namespace VRCSDK2
{
	public interface IVRC_Destructible
	{
		object[] GetState();

		void SetState(object[] state);

		float GetMaxHealth();

		float GetCurrentHealth();

		void ApplyDamage(float damage);

		void ApplyHealing(float healing);
	}
}
