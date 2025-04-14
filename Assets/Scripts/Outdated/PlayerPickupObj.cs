using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerPickupObj : MonoBehaviour
{
    LayerMask layerMask;
	Transform playerPOV;
	GameObject pickupContainer;
	GameObject itemPickupRig;
	GameObject pickupPosition;
	MultiParentConstraint multiParentConstraint;
	
	public GameObject currentlyHeldObject = null;

	void Start()
	{
		layerMask = LayerMask.GetMask("Object");
		playerPOV = Camera.main.transform;
		itemPickupRig = gameObject.transform.Find("Item Pickup Rig").gameObject;
		multiParentConstraint = itemPickupRig.transform.Find("Pickup").gameObject.GetComponent<MultiParentConstraint>();
		pickupContainer = itemPickupRig.transform.Find("PickupContainer").gameObject;
		pickupPosition = itemPickupRig.transform.Find("PickupPosition").gameObject;
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
			pickupPosition.transform.rotation = hit.transform.rotation;
			pickupPosition.transform.position = hit.transform.position;
			SetWeightSlider(0, 1);
			SetWeightSlider(1, 0);
			hit.transform.SetParent(pickupContainer.transform);
			hit.transform.localPosition = Vector3.zero;
			hit.transform.localRotation = Quaternion.Euler(Vector3.zero);
			hit.transform.GetComponent<Rigidbody>().isKinematic = true;
			hit.transform.GetComponent<BoxCollider>().isTrigger = true;
			Debug.Log("Selected an object!! Yippee!! \n" + "Object name: " + hit.transform.gameObject.name + "\n" + "Object distance from player: " + hit.distance);
			StartCoroutine("IncreaseWeightSlider");
		}
	}
	
	void DropItem()
	{
		currentlyHeldObject.transform.SetParent(null);
		currentlyHeldObject.transform.GetComponent<Rigidbody>().isKinematic = false;
		currentlyHeldObject.transform.GetComponent<BoxCollider>().isTrigger = false;
		currentlyHeldObject = null;
	}
	
	IEnumerator IncreaseWeightSlider()
	{
		float pickupDuration = 1f; 
		float pickupFrames = 20f;

		for (int frame = 0; frame <= pickupFrames; frame++)
		{
			float t = frame / pickupFrames;
			SetWeightSlider(0, 1f - t);
			SetWeightSlider(1, t);
			yield return new WaitForSeconds(pickupDuration / pickupFrames);
		}
	}
	
	void SetWeightSlider(int item, float weight)
	{
		var sources = multiParentConstraint.data.sourceObjects;
		sources.SetWeight(item, weight);
		multiParentConstraint.data.sourceObjects = sources;
	}
}
