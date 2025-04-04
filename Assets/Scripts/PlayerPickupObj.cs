using UnityEngine;

public class PlayerPickupObj : MonoBehaviour
{
    LayerMask layerMask;
	Transform playerPOV;
	[SerializeField] GameObject penContainer;

	void Start()
	{
		layerMask = LayerMask.GetMask("Object");
		playerPOV = Camera.main.transform;
		if (penContainer == null)
		{
			try {penContainer = transform.Find("PenContainer").gameObject;}
			catch {Debug.Log("PlayerPickupObj.cs: Couldn't find the PenContainer gameObject!");}
		}
	}
	
	void FixedUpdate()
	{
		if (Input.GetKey(KeyCode.Mouse0))
		{
			PickUpItem();
		}
	}

	void PickUpItem()
	{
		RaycastHit hit;
		if (Physics.Raycast(playerPOV.position, playerPOV.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
		{
			hit.transform.SetParent(penContainer.transform);
			hit.transform.localPosition = Vector3.zero;
			hit.transform.localRotation = Quaternion.Euler(Vector3.zero);
			hit.transform.GetComponent<Rigidbody>().isKinematic = true;
			hit.transform.GetComponent<BoxCollider>().isTrigger = true;
			Debug.Log("Selected an object!! Yippee!! \n" + "Object name: " + hit.transform.gameObject.name + "\n" + "Object distance from player: " + hit.distance);
		}
	}
}
