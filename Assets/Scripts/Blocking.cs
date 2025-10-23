using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Blocking : NetworkBehaviour
{
	Animator anim;
	PlayerStats playerStats;

	[SerializeField] GameObject torsoAimTarget;
	[SerializeField] GameObject torsoTargetingRig;
	Rig torsoRig;
	[HideInInspector] public NetworkVariable<bool> isBlocking = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
	[HideInInspector] public bool midBlockPushback = false;
	[HideInInspector] public NetworkVariable<bool> isParrying = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
	bool alreadyParried = false;

	static readonly float blockPushbackAnimationDurationSpeedMultiplier = 1f;
    static readonly float blockPushbackAnimationDuration = 0.583f / blockPushbackAnimationDurationSpeedMultiplier;

	static readonly float parryAnimationDurationSpeedMultiplier = 0.2f;
	static readonly float parryAnimationDuration = 0.167f / parryAnimationDurationSpeedMultiplier;

    void Start()
    {
        anim = GetComponent<Animator>();
		playerStats = GetComponent<PlayerStats>();
		torsoRig = torsoTargetingRig.GetComponent<Rig>();
    }

    void Update()
    {
		if (!IsOwner) {return;}
		if (playerStats.isTargettedForAttack.Value == false) {alreadyParried = false;}

		if (Input.GetKeyDown("e") && !alreadyParried)
		{
			StartCoroutine("ParryWindow");
			alreadyParried = true;
		}

        if (Input.GetKey("e"))
		{
			isBlocking.Value = true;
			anim.SetBool("isBlocking", true);
		}
		else if (!Input.GetKey("e"))
		{
			StopCoroutine("ParryWindow");
			isBlocking.Value = false;
			anim.SetBool("isBlocking", false);
		}
    }

	[ServerRpc(RequireOwnership = false)]
	public void TriggerParryServerRpc(NetworkObjectReference parryTargetNetRef)
	{
		TriggerParryClientRpc(parryTargetNetRef);
	}

	[ClientRpc]
	private void TriggerParryClientRpc(NetworkObjectReference parryTargetNetRef)
	{
		if (parryTargetNetRef.TryGet(out NetworkObject netObj))
		{
			StartCoroutine(Parry(netObj.gameObject));
		}
	}

	[ServerRpc(RequireOwnership = false)]
	public void TriggerBlockPushbackServerRpc()
	{
		TriggerBlockPushbackClientRpc();
	}

	[ClientRpc]
	private void TriggerBlockPushbackClientRpc()
	{
		StartCoroutine("BlockPushback");
	}

	IEnumerator Parry(GameObject parryTarget)
	{
		if (IsOwner)
		{
			playerStats.canMove = false;
			playerStats.canTurn = false;
			anim.SetBool("parry", true);
		}
		torsoAimTarget.transform.position = parryTarget.transform.position;

		float parryAnimationDurationHalf = parryAnimationDuration / 2;
		float elapsed = 0f;
		while (elapsed < parryAnimationDurationHalf)
		{
			elapsed += Time.deltaTime;
			float t = Mathf.Clamp01(elapsed / parryAnimationDurationHalf);

			// SMOOTH INTERPOLATION PEAK
			float smoothT = Mathf.SmoothStep(0f, 1f, t);

			// Torso rigs
			torsoAimTarget.transform.position = parryTarget.transform.position;
			torsoRig.weight = smoothT;

			yield return null;
		}
		torsoRig.weight = 1f;
		yield return new WaitForSeconds(parryAnimationDurationHalf);
		if (IsOwner)
		{
			playerStats.canMove = true;
			playerStats.canTurn = true;
			anim.SetBool("parry", false);
		}
		torsoRig.weight = 0;
	}

	IEnumerator ParryWindow()
	{
		isParrying.Value = true;
		yield return new WaitForSeconds(0.5f);
		isParrying.Value = false;
	}

	IEnumerator BlockPushback()
	{
		if (IsOwner)
		{
			// Disable movement
			// Disable ability to turn camera
			// Disable ability to perform ANY actions
			playerStats.canMove = false;
			playerStats.canTurn = false;
			// Perform the pushback animation
			anim.SetBool("blockingPushback", true);
		}
		yield return new WaitForSeconds(blockPushbackAnimationDuration);
		if (IsOwner)
		{
			// Re-enable allat shi
			playerStats.canMove = true;
			playerStats.canTurn = true;
			anim.SetBool("blockingPushback", false);
		}
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