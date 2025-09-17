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
	
	LayerMask humanTouchLayerMask;
	
	public GameObject currentlyHeldObject = null;
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
		humanTouchLayerMask = LayerMask.GetMask("DummyTrigger");
		
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
		Ray ray = new Ray(playerPOV.position, playerPOV.TransformDirection(Vector3.forward));
		RaycastHit hit;
		if (Physics.SphereCast(ray, dogshitAimRadius, out hit, pickUpDistance, layerMask))
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
