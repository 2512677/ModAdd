using UnityEngine;
using UnityEngine.SceneManagement;

public class RCC_ResetScene : MonoBehaviour
{
	private void Update()
	{
		if (UnityEngine.Input.GetKeyUp(KeyCode.R))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}
