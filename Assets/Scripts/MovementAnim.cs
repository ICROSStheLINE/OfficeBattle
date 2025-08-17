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
    }

	void LateUpdate()
	{
		Vector3 localMove = playerMovement.GetLocalMovementDirectionNormalized();
		
		anim.SetBool("isRunning", playerStats.isRunning);
		
		anim.SetFloat("runDirectionX", localMove.x, 0.1f, Time.deltaTime); // for strafing left and right
		anim.SetFloat("runDirectionY", localMove.z, 0.1f, Time.deltaTime); // for running forward and backwards
    }
}
