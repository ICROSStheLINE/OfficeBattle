using UnityEngine;

public class DamageHitbox : MonoBehaviour
{
	[SerializeField] float damageAmount = 2;
	[SerializeField] float heavyDamageAmount = 5;
	[HideInInspector] public bool damageActive = false;
	
	void OnTriggerEnter(Collider collision)
	{
		if (damageActive && ((collision.gameObject.tag == "Player") || collision.gameObject.tag == "Test Dummy"))
		{
			// Reference the collided object's HP or stat script
			PlayerStats collidedPlayerStats = collision.GetComponent<PlayerStats>();
			// Change the HP variable to have taken damage
			if (collidedPlayerStats != null)
			{
				collidedPlayerStats.TakeDamage(damageAmount);
			}
			// That's it! GGEZ
		}
	}
	
	void OnCollisionEnter(Collision collision)
	{
		if (damageActive && ((collision.gameObject.tag == "Player") || collision.gameObject.tag == "Test Dummy"))
		{
			PlayerStats collidedPlayerStats = collision.gameObject.GetComponent<PlayerStats>();
			if (collidedPlayerStats != null)
			{
				collidedPlayerStats.TakeDamage(heavyDamageAmount);
			}
		}
		damageActive = false;
	}
}
