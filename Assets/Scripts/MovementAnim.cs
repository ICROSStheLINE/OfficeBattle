using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnim : MonoBehaviour
{
    PlayerStats playerStats;
    PlayerMovement playerMovement;
    Animator anim;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerStats = GetComponent<PlayerStats>();
        anim = GetComponent<Animator>();
		
		//InvokeRepeating("LogRunDirection", 3f, 1f);
    }

    void LateUpdate()
    {
        anim.SetBool("isRunning", playerStats.isRunning);
		anim.SetFloat("runDirection", Vector3.SignedAngle(transform.right, playerMovement.moveDirection, Vector3.up));
		// ^^^ If this equals -90 then the player is running straight forward.
		// ^^^ If this equals 0 then the player is running right
		// ^^^ If this equals 180 then the player is running left
		// ^^^ If this equals 90 then the player is running straight backwards
		// ^^^ If this equals -45 then the player is running forward-right
		// ^^^ If this equals -135 then the player is running forward-left
		// ^^^ If this equals 45 then the player is running backwards-right
		// ^^^ If this equals 135 then the player is running backwards-left
    }
	
	void LogRunDirection()
	{
		Debug.Log(Vector3.SignedAngle(transform.right, playerMovement.moveDirection, Vector3.up));
	}
}
