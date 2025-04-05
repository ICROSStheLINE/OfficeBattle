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
	
	bool isMidAttack = false;
	
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
        if (Input.GetKeyDown("f") && !isMidAttack && (penContainer.transform.childCount > 0))
		{
			StartCoroutine("AttackSwing");
		}
    }
	
	IEnumerator AttackSwing()
	{
		Animator penAnim = penContainer.GetComponent<Animator>();
		BoxCollider damageHitbox = penContainer.GetComponent<BoxCollider>();
		isMidAttack = true;
		penAnim.SetBool("isAttacking", true);
		yield return new WaitForSeconds(secondsUntilDamageStart);
		damageHitbox.enabled = true;
		yield return new WaitForSeconds(secondsBetweenDamageStartAndDamageEnd);
		damageHitbox.enabled = false;;
		yield return new WaitForSeconds(secondsBetweenDamageEndAndAnimationEnd);
		isMidAttack = false;
		penAnim.SetBool("isAttacking", false);
	}
}
