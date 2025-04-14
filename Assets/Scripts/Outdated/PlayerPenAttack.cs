using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPenAttack : MonoBehaviour
{
	PlayerPickupObj playerPickupObj;
	Animator penContainerAnim;
	
    GameObject penContainer;
	static readonly float animationDurationSpeedMultiplier = 1f;
	static readonly float animationDuration = 0.917f / animationDurationSpeedMultiplier;
	static readonly float animationFrames = 55f;
	static readonly float damageHitboxStartFrame = 15f;
	static readonly float damageHitboxEndFrame = 25f;
	static readonly float secondsUntilDamageStart = (damageHitboxStartFrame / animationFrames) * animationDuration;
	static readonly float secondsUntilDamageEnd = (damageHitboxEndFrame / animationFrames) * animationDuration;
	static readonly float secondsBetweenDamageStartAndDamageEnd = secondsUntilDamageEnd - secondsUntilDamageStart;
	static readonly float secondsBetweenDamageEndAndAnimationEnd = animationDuration - secondsUntilDamageEnd;
	
	static readonly float throwWindupDurationSpeedMultiplier = 1f;
	static readonly float throwWindupDuration = 0.667f / throwWindupDurationSpeedMultiplier;
	
	bool isMidAttack = false;
	bool isMidThrow = false;
	
    void Start()
    {
		playerPickupObj = GetComponent<PlayerPickupObj>();
		
        penContainer = Camera.main.transform.Find("PenContainer").gameObject;
		penContainerAnim = penContainer.GetComponent<Animator>();
    }

    
    void Update()
    {
        if (Input.GetKeyDown("f") && !isMidAttack && !isMidThrow && (penContainer.transform.childCount > 0))
		{
			StartCoroutine("PenSwipe");
		}
		if (Input.GetKeyDown("e") && !isMidAttack && !isMidThrow && (penContainer.transform.childCount > 0))
		{
			StartCoroutine("PenThrow");
		}
    }
	
	IEnumerator PenSwipe()
	{
		DamageHitbox damageHitbox = penContainer.transform.GetChild(0).gameObject.GetComponent<DamageHitbox>();
		isMidAttack = true;
		
		penContainerAnim.SetBool("isAttacking", true);
		yield return new WaitForSeconds(secondsUntilDamageStart);
		damageHitbox.damageActive = true;
		yield return new WaitForSeconds(secondsBetweenDamageStartAndDamageEnd);
		damageHitbox.damageActive = false;
		yield return new WaitForSeconds(secondsBetweenDamageEndAndAnimationEnd);
		isMidAttack = false;
		penContainerAnim.SetBool("isAttacking", false);
	}
	
	IEnumerator PenThrow()
	{
		GameObject pen = penContainer.transform.GetChild(0).gameObject;
		DamageHitbox damageHitbox = pen.GetComponent<DamageHitbox>();
		BoxCollider penCollider = pen.GetComponent<BoxCollider>();
		Rigidbody penRb = pen.transform.GetComponent<Rigidbody>();
		isMidThrow = true;
		
		penContainerAnim.SetBool("isThrowing", true);
		yield return new WaitForSeconds(throwWindupDuration);
		penContainerAnim.SetBool("isThrowing", false);
		
		pen.transform.SetParent(null);
		penRb.isKinematic = false;
		damageHitbox.damageActive = true;
		penCollider.isTrigger = false;
		
		penRb.linearVelocity = Camera.main.transform.forward.normalized * 10;
		
		playerPickupObj.currentlyHeldObject = null;
		isMidThrow = false;
	}
}
