using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPenAttack : MonoBehaviour
{
    [SerializeField] GameObject penContainer;
	static readonly float animationDurationSpeedMultiplier = 1f;
	static readonly float animationDuration = 0.917f / animationDurationSpeedMultiplier;
	static readonly float animationFrames = 55f;
	static readonly float damageHitboxStartFrame = 15f;
	static readonly float damageHitboxEndFrame = 25f;
	static readonly float secondsUntilDamageStart = (damageHitboxStartFrame / animationFrames) * animationDuration;
	static readonly float secondsUntilDamageEnd = (damageHitboxEndFrame / animationFrames) * animationDuration;
	static readonly float secondsBetweenDamageStartAndDamageEnd = secondsUntilDamageEnd - secondsUntilDamageStart;
	static readonly float secondsBetweenDamageEndAndAnimationEnd = animationDuration - secondsUntilDamageEnd;
	
    void Start()
    {
        if (penContainer == null)
		{
			try {penContainer = transform.Find("PenContainer").gameObject;}
			catch {Debug.Log("PlayerPenAttack.cs: Couldn't find the PenContainer gameObject!");}
		}
    }

    
    void Update()
    {
        if (Input.GetKeyDown("f"))
		{
			StartCoroutine("AttackSwing");
		}
    }
	
	IEnumerator AttackSwing()
	{
		Animator penAnim = penContainer.GetComponent<Animator>();
		penAnim.enabled = true;
		/*
		yield return new WaitForSeconds(secondsUntilDamageStart);
		// Activate Damage Hitbox
		yield return new WaitForSeconds(secondsBetweenDamageStartAndDamageEnd);
		// Deactivate Damage Hitbox
		yield return new WaitForSeconds(secondsBetweenDamageEndAndAnimationEnd);
		*/
		yield return new WaitForSeconds(animationDuration); // TODO: Remove this later when the damage hitbox is implemented
		penAnim.enabled = false;
	}
}
