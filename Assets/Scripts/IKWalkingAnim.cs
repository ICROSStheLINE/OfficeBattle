using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKWalkingAnim : MonoBehaviour
{
	LayerMask layerMask;
	GameObject rayPositionGameObject;
	Vector3 currentPosition;

    void Start()
    {
		rayPositionGameObject = transform.parent.gameObject.transform.Find("rayPosition").gameObject;
		
        layerMask = LayerMask.GetMask("Terrain");
    }

    void Update()
    {
		Vector3 rayPosition = rayPositionGameObject.transform.position;
		transform.position = currentPosition;
		Ray ray = new Ray(rayPosition + Vector3.up * 2, Vector3.down);
		if (Physics.Raycast(ray, out RaycastHit hit, 15, layerMask))
		{
			if (Vector3.Distance(currentPosition, hit.point) > 1f)
			{
				currentPosition = hit.point + new Vector3(0,0.2f,0);
			}
		}
    }
}
