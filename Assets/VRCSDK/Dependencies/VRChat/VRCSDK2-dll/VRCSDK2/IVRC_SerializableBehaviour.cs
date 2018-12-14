namespace VRCSDK2
{
	public interface IVRC_SerializableBehaviour
	{
		byte[] GetBytes();

		void SetBytes(byte[] stream);
	}
}
