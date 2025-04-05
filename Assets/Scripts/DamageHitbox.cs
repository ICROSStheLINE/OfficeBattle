using UnityEngine;

public class DamageHitbox : MonoBehaviour
{
	public float damageNumber;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
	
	void OnTriggerEnter(Collider collision)
	{
		if ((collision.gameObject.tag == "Player") || collision.gameObject.tag == "Test Dummy")
		{
			// Reference the collided object's HP or stat script
			// Change the HP variable to have taken damage
			// That's it! GGEZ
		}
	}
}
