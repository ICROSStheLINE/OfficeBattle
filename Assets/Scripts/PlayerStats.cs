using Unity.Netcode;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
	public float maxHealth = 5;
	public float health;
	[HideInInspector] public bool isRunning = false;
	[HideInInspector] public bool canMove = true;
	[HideInInspector] public bool canTurn = true;
	public int playerNumber;

	void Start()
	{
		health = maxHealth;
		if (IsOwner)
		{
			GameObject.Find("Crosshair").transform.GetComponent<CrosshairTarget>().SetPlayer(playerNumber);
			if (transform.GetComponent<Punching>())
			{ transform.GetComponent<Punching>().SetPlayer(playerNumber); }
		}
    }

    void Update()
    {
		
    }
	
	public void TakeDamage(float amount = default(float))
	{
		if (amount == default(float)) { amount = 1f; }
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

