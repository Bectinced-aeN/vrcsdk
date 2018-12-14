namespace VRCSDK2
{
	public interface IVRC_Destructible
	{
		byte[] GetState();

		void SetState(byte[] state);

		float GetMaxHealth();

		float GetCurrentHealth();

		void ApplyDamage(float damage);

		void ApplyHealing(float healing);
	}
}
