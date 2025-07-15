using UnityEngine;

public class PlayerPickupObj : MonoBehaviour
{
    LayerMask layerMask;
	Transform playerPOV;
	GameObject penContainer;
	public GameObject currentlyHeldObject = null;
	//GameObject rightArm;

	void Start()
	{
		layerMask = LayerMask.GetMask("Object");
		playerPOV = Camera.main.transform;
		penContainer = Camera.main.transform.Find("PenContainer").gameObject;
		//rightArm = Camera.main.transform.Find("RightArm").gameObject;
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
			hit.transform.SetParent(penContainer.transform);
			hit.transform.localPosition = Vector3.zero;
			hit.transform.localRotation = Quaternion.Euler(Vector3.zero);
			hit.transform.GetComponent<Rigidbody>().isKinematic = true;
			hit.transform.GetComponent<BoxCollider>().isTrigger = true;
			Debug.Log("Selected an object!! Yippee!! \n" + "Object name: " + hit.transform.gameObject.name + "\n" + "Object distance from player: " + hit.distance);
		}
	}
	
	void DropItem()
	{
		currentlyHeldObject.transform.SetParent(null);
		currentlyHeldObject.transform.GetComponent<Rigidbody>().isKinematic = false;
		currentlyHeldObject.transform.GetComponent<BoxCollider>().isTrigger = false;
		currentlyHeldObject = null;
	}
}
