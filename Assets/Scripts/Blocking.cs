using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Blocking : NetworkBehaviour
{
	Animator anim;
	PlayerStats playerStats;
	
	[HideInInspector] public bool isBlocking = false;
	[HideInInspector] public bool midBlockPushback = false;
	
	static readonly float blockPushbackAnimationDurationSpeedMultiplier = 1f;
    static readonly float blockPushbackAnimationDuration = 0.583f / blockPushbackAnimationDurationSpeedMultiplier;
	
    void Start()
    {
        anim = GetComponent<Animator>();
		playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
		if (!IsOwner)
		{
			return;
		}
		
        if (Input.GetKey("e"))
		{
			isBlocking = true;
			anim.SetBool("isBlocking", true);
		}
		else
		{
			isBlocking = false;
			anim.SetBool("isBlocking", false);
		}
    }
	
	IEnumerator BlockPushback()
	{
		// Disable movement
		// Disable ability to turn camera
		// Disable ability to perform ANY actions
		playerStats.canMove = false;
		playerStats.canTurn = false;
		// Perform the pushback animation
		anim.SetBool("blockingPushback", true);
		yield return new WaitForSeconds(blockPushbackAnimationDuration);
		// Re-enable allat shi
		playerStats.canMove = true;
		playerStats.canTurn = true;
		anim.SetBool("blockingPushback", false);
	}
}

/* 
Chopped:

- Lebanese
- Turkish
- Egypt (50/50)
- Isreal (80/20)
- Iraq (Not Kurdish)
- Bahrain
- Oman (Beans n' Bites)
- Palestinian

Huzz:

- Kurdish
- Syria
- Iran (50/50)
- Jordan
- Saudi (50/50)
- Qatar (75/25) (i'm NOT broke)

*/