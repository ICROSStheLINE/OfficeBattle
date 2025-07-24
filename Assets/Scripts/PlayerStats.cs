using UnityEngine;

public class PlayerStats : MonoBehaviour
{
	public float maxHealth = 5;
	public float health;
	[HideInInspector] public bool isRunning = false;
	
    void Start()
    {
		health = maxHealth;
    }

    void Update()
    {
		
    }
	
	public void TakeDamage(float amount)
	{
		health -= amount;
		if (health <= 0)
		{
			Die();
		}
	}

	void Die()
	{
		Debug.Log(gameObject.name + " died. RIP");
	}
}

