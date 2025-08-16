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
		// anim.SetBool("isRunning", playerStats.isRunning);
		// anim.SetFloat("runDirection", playerMovement.GetMovementDirectionAngle());

		anim.SetBool("isRunning", true);
		anim.SetFloat("runDirection", -45f);
    }
	
	void LogRunDirection()
	{
		Debug.Log(Vector3.SignedAngle(transform.right, playerMovement.moveDirection, Vector3.up));
	}
}
