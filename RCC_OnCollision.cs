using UnityEngine;

public class RCC_OnCollision : MonoBehaviour
{
	public delegate void onCollisionTake();

	public static event onCollisionTake OnCollisionTake;

	private void OnCollisionEnter(Collision col)
	{
		if (!(col.relativeVelocity.magnitude < 5f) && RCC_OnCollision.OnCollisionTake != null)
		{
			RCC_OnCollision.OnCollisionTake();
		}
	}
}
