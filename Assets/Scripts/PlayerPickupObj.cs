using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerPickupObj : MonoBehaviour
{
	Animator anim;
	
    LayerMask layerMask;
	Transform playerPOV;
	[SerializeField] GameObject penContainer;
	public GameObject currentlyHeldObject = null;
	[SerializeField] GameObject torsoAimTarget;
	[SerializeField] GameObject torsoTargetingRig;
	[SerializeField] GameObject handPosTarget;
	[SerializeField] GameObject handPosRig;
	Rig torsoRig;
	Rig handRig;
	
	static readonly float animationDurationSpeedMultiplier = 1f;
	static readonly float animationDuration = 1.083f / animationDurationSpeedMultiplier;
	static readonly float animationFrames = 65f;
	static readonly float itemGrabbedFrame = 23f;
	static readonly float secondsUntilItemGrabbed = (itemGrabbedFrame / animationFrames) * animationDuration;
	static readonly float secondsBetweenItemGrabAndAnimationEnd = animationDuration - secondsUntilItemGrabbed;

	void Start()
	{
		layerMask = LayerMask.GetMask("Object");
		playerPOV = Camera.main.transform;
		anim = GetComponent<Animator>();
		torsoRig = torsoTargetingRig.GetComponent<Rig>();
		handRig = handPosRig.GetComponent<Rig>();
	}
	
	void FixedUpdate()
	{
		if (Input.GetKey(KeyCode.Mouse0))
		{
			if (currentlyHeldObject == null)
				PickUpItem();
		}
		if (Input.GetKey(KeyCode.Mouse1))
		{
			if (currentlyHeldObject != null)
				DropItem();
		}
	}

	void PickUpItem()
	{
		RaycastHit hit;
		if (Physics.Raycast(playerPOV.position, playerPOV.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
		{
			currentlyHeldObject = hit.transform.gameObject;
			StartCoroutine("HandlePickUpAnimation");
			//Debug.Log("Selected an object!! Yippee!! \n" + "Object name: " + hit.transform.gameObject.name + "\n" + "Object distance from player: " + hit.distance);
		}
	}
	
	IEnumerator HandlePickUpAnimation()
	{
		anim.SetBool("isPickingUpObj", true);

		// Hand and torso rig target objects
		handPosTarget.transform.position = currentlyHeldObject.transform.position;
		torsoAimTarget.transform.position = currentlyHeldObject.transform.position;

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
			handPosTarget.transform.position = currentlyHeldObject.transform.position;
			handRig.weight = smoothT;
			torsoAimTarget.transform.position = currentlyHeldObject.transform.position;
			torsoRig.weight = smoothT;

			yield return null;
		}

		// Snap to the max rig weight just in case the math didn't add up earlier
		handRig.weight = 1f;
		torsoRig.weight = 1f;

		// ───────────────────────────────
		// Item goes in its container
		currentlyHeldObject.transform.SetParent(penContainer.transform); // For now only container is penContainer (lol)
		currentlyHeldObject.transform.localPosition = Vector3.zero;
		currentlyHeldObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
		currentlyHeldObject.GetComponent<Rigidbody>().isKinematic = true;
		currentlyHeldObject.GetComponent<BoxCollider>().isTrigger = true;

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

		anim.SetBool("isPickingUpObj", false);
	}

	
	void DropItem()
	{
		currentlyHeldObject.transform.SetParent(null);
		currentlyHeldObject.transform.GetComponent<Rigidbody>().isKinematic = false;
		currentlyHeldObject.transform.GetComponent<BoxCollider>().isTrigger = false;
		currentlyHeldObject = null;
	}
}
