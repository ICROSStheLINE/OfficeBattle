using UnityEngine;

public class DamageHitbox : MonoBehaviour
{
	[SerializeField] float damageAmount = 2;

    void Start()
    {
        
    }
	
	void OnTriggerEnter(Collider collision)
	{
		if ((collision.gameObject.tag == "Player") || collision.gameObject.tag == "Test Dummy")
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
}
