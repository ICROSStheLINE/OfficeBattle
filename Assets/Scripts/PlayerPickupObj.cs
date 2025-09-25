using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerPickupObj : NetworkBehaviour
{
	Animator anim;
	MovementAnim movementAnim;
	
    LayerMask layerMask;
	Transform playerPOV;
	[SerializeField] GameObject penContainer;
	[SerializeField] GameObject torsoAimTarget;
	[SerializeField] GameObject torsoTargetingRig;
	[SerializeField] GameObject handPosTarget;
	[SerializeField] GameObject handPosRig;
	Rig torsoRig;
	Rig handRig;

	// - Note -
	// This NetworkVariable should ideally be set to null, but 'NetworkObjectReference' variables cannot be set to null for some reason
	// Setting it to default is the next best thing, as (in this case) it is effectively the same as null.
	// The Unity Engine devs addressed this complaint and said they would fix it in version 1.9 or something, but I'm on like version 2.something and it's NOT FIXED YET
	// These devs are FOOLISH/LAZY???
	public NetworkVariable<NetworkObjectReference> currentlyHeldObject = new NetworkVariable<NetworkObjectReference>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
	float dogshitAimRadius = 0.5f;
	float pickUpDistance = 5f;
	
	static readonly float animationDurationSpeedMultiplier = 1f;
	static readonly float animationDuration = 1f / animationDurationSpeedMultiplier;
	static readonly float animationFrames = 24f;
	static readonly float itemGrabbedFrame = 8f;
	static readonly float secondsUntilItemGrabbed = (itemGrabbedFrame / animationFrames) * animationDuration;
	static readonly float secondsBetweenItemGrabAndAnimationEnd = animationDuration - secondsUntilItemGrabbed;
	

	void Start()
	{
		layerMask = LayerMask.GetMask("Object");
		playerPOV = Camera.main.transform;
		anim = GetComponent<Animator>();
		torsoRig = torsoTargetingRig.GetComponent<Rig>();
		handRig = handPosRig.GetComponent<Rig>();
		
		movementAnim = GetComponent<MovementAnim>();
	}
	
	void FixedUpdate()
	{
		if (!IsOwner)
		{
			return;
		}

		if (Input.GetKey(KeyCode.Mouse0))
		{
			if (currentlyHeldObject.Equals(default(NetworkObjectReference)))
				PickUpItem();
		}
		if (Input.GetKey(KeyCode.Mouse1))
		{
			if (!currentlyHeldObject.Equals(default(NetworkObjectReference)))
				DropItem();
		}
	}

	void PickUpItem()
	{
		Ray ray = new Ray(playerPOV.position, playerPOV.TransformDirection(Vector3.forward));
		RaycastHit hit;
		if (Physics.SphereCast(ray, dogshitAimRadius, out hit, pickUpDistance, layerMask))
		{
			currentlyHeldObject.Value = hit.transform.gameObject.GetComponent<NetworkObjectReference>();
			TriggerPickUpServerRpc(currentlyHeldObject.Value);
		}
	}

	// Think back to the image explaining Owner Authoritative Networking.
	// Here, the Owner is sending the information to the server, 
	// then in the ClientRpc the server is distributing that information to all the oher clients.
	[ServerRpc(RequireOwnership = false)]
	public void TriggerPickUpServerRpc(NetworkObjectReference itemRef)
	{
		Debug.Log("Flag 1");
		TriggerPickUpClientRpc(itemRef);
	}

	// Without this ClientRpc method, this stuff would only show for the server/host.
	[ClientRpc]
	private void TriggerPickUpClientRpc(NetworkObjectReference itemRef)
	{
		Debug.Log("Flag 2");

		// - Note -
		// variable.TryGet seems to be a built-in function that comes with all "NetworkObjectReference" variables.
		// Its sole function is to get the NetworkObject component that is being referenced by "NetworkObjectReference".
		// You might be thinking "Why don't we just use the NetworkObject reference in the first place instead of having to deal with this "NetworkObjectReference" variable?
		// NetworkObjectReference (in conjunction with .TryGet) is useful because it can tell if an object DOESN'T EXIST IN THE SCENE ANYMORE!
		// If we only referenced "NetworkObject", it would retain that reference in memory even AFTER the object has been removed from the scene (for some reason).
		// Crazy stuff.
		if (itemRef.TryGet(out NetworkObject netObj))
		{
			StartCoroutine(HandlePickUpAnimation(netObj.gameObject));
		}
	}

	IEnumerator HandlePickUpAnimation(GameObject itemRef)
	{
		Debug.Log("Flag 3");
		if (IsOwner)
		{
			anim.SetBool("isPickingUpObj", true);
		}

		// Hand and torso rig target objects
		handPosTarget.transform.position = itemRef.gameObject.transform.position;
		torsoAimTarget.transform.position = itemRef.gameObject.transform.position;

		// ───────────────────────────────
		// Phase 1: Reach for the item
		float elapsed = 0f;
		while (elapsed < secondsUntilItemGrabbed)
		{
			elapsed += Time.deltaTime;
			float t = Mathf.Clamp01(elapsed / secondsUntilItemGrabbed);

			// SMOOTH INTERPOLATION PEAK
			float smoothT = Mathf.SmoothStep(0f, 1f, t);

			// Hand and Torso rigs
			handPosTarget.transform.position = itemRef.gameObject.transform.position;
			handRig.weight = smoothT;
			torsoAimTarget.transform.position = itemRef.gameObject.transform.position;
			torsoRig.weight = smoothT;

			yield return null;
		}

		// Snap to the max rig weight just in case the math didn't add up earlier
		handRig.weight = 1f;
		torsoRig.weight = 1f;

		// ───────────────────────────────
		// Item goes in its container
		itemRef.gameObject.transform.SetParent(penContainer.transform); // For now only container is penContainer (lol)
		itemRef.gameObject.transform.localPosition = Vector3.zero;
		itemRef.gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
		itemRef.gameObject.transform.GetComponent<Rigidbody>().isKinematic = true;
		itemRef.gameObject.transform.GetComponent<BoxCollider>().isTrigger = true;

		// ───────────────────────────────
		// Phase 2: Return hand and torso to normal positions
		elapsed = 0f;
		while (elapsed < secondsBetweenItemGrabAndAnimationEnd)
		{
			elapsed += Time.deltaTime;
			float t = Mathf.Clamp01(elapsed / secondsBetweenItemGrabAndAnimationEnd);

			// Glorious smooth interpolation
			float smoothT = Mathf.SmoothStep(1f, 0f, t);

			handRig.weight = smoothT;
			torsoRig.weight = smoothT;

			yield return null;
		}

		// Reset rig weights back to 0 just in case math didn't add up earlier
		handRig.weight = 0f;
		torsoRig.weight = 0f;

		if (IsOwner)
		{
			anim.SetBool("isPickingUpObj", false);
		}
	}


	void DropItem()
	{
		if (currentlyHeldObject.Value.TryGet(out NetworkObject netObj))
		{
			netObj.gameObject.transform.SetParent(null);
			netObj.gameObject.transform.GetComponent<Rigidbody>().isKinematic = false;
			netObj.gameObject.transform.GetComponent<BoxCollider>().isTrigger = false;
		}
		currentlyHeldObject.Value = default(NetworkObjectReference);
	}
}
