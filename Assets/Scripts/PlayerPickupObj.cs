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
	Rig torsoRig;
	
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
		torsoAimTarget.transform.position = currentlyHeldObject.transform.position;
		torsoRig.weight = 0f;
		float smoothnessFactor = 20f;
		for (float i = 0; i < smoothnessFactor + 1; i += 1f)
		{
			yield return new WaitForSeconds(secondsUntilItemGrabbed / smoothnessFactor);
			torsoAimTarget.transform.position = currentlyHeldObject.transform.position;
			torsoRig.weight = 1f * (i / smoothnessFactor);
		}
		
		currentlyHeldObject.transform.SetParent(penContainer.transform);
		currentlyHeldObject.transform.localPosition = Vector3.zero;
		currentlyHeldObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
		currentlyHeldObject.transform.GetComponent<Rigidbody>().isKinematic = true;
		currentlyHeldObject.transform.GetComponent<BoxCollider>().isTrigger = true;
		
		for (float i = smoothnessFactor; i >= 0; i -= 1f)
		{
			yield return new WaitForSeconds(secondsBetweenItemGrabAndAnimationEnd / smoothnessFactor);
			if (torsoRig.weight > 0)
			{
				torsoRig.weight -= 0.15f;
			}
		}
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
