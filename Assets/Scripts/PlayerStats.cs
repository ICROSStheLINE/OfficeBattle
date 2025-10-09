using Unity.Netcode;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
	public float maxHealth = 5;
	
	public NetworkVariable<float> health = new NetworkVariable<float>(5f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
	
	[HideInInspector] public bool isRunning = false;
	[HideInInspector] public bool canMove = true;
	[HideInInspector] public bool canTurn = true;
	public int playerNumber;

	/*void Awake()
	{
		health.OnValueChanged += OnHealthChanged;
	}
	
	private void OnHealthChanged(float oldValue, float newValue)
	{
		
	}*/

	void Start()
	{
		health.Value = maxHealth;
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
	
	[ServerRpc(RequireOwnership = false)]
	public void TakeDamageServerRpc(float amount = default(float))
	{
		if (amount == default(float)) { amount = 1f; }
		
		if (health.Value <= 0) return;

		health.Value -= amount;
		if (health.Value <= 0)
		{
			Die();
		}
	}

	void Die()
	{
		Debug.Log(gameObject.name + " died. RIP");
	}
}

