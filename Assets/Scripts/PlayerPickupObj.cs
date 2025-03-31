using UnityEngine;

public class PlayerPickupObj : MonoBehaviour
{
    LayerMask layerMask;
	Transform playerPOV;

	void Start()
	{
		layerMask = LayerMask.GetMask("Object");
		playerPOV = Camera.main.transform;
	}
	
	void FixedUpdate()
	{
		if (Input.GetKey("g"))
		{
			PickUpItem();
		}
	}

	void PickUpItem()
	{
		RaycastHit hit;
		if (Physics.Raycast(playerPOV.position, playerPOV.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
		{
			Debug.Log("Selected an object!! Yippee!! \n" + "Object name: " + hit.transform.gameObject.name + "\n" + "Object distance from player: " + hit.distance);
		}
	}
}
