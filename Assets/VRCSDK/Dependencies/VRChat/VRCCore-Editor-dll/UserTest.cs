using UnityEngine;

public class UserTest : MonoBehaviour
{
	public UserTest()
		: this()
	{
	}

	private void Start()
	{
		this.Invoke("Run", 5f);
	}

	private void Run()
	{
	}
}
