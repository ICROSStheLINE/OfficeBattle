using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKWalkingAnim : MonoBehaviour
{
	LayerMask layerMask;
	GameObject rayPositionGameObject;
	GameObject standingPositionGameObject;
	Vector3 currentPosition;

	float stepDistanceThreshhold = 1f;

    void Start()
    {
		rayPositionGameObject = transform.parent.gameObject.transform.Find("rayPosition").gameObject;
		standingPositionGameObject = transform.parent.gameObject.transform.Find("StandingPosition").gameObject;

        layerMask = LayerMask.GetMask("Terrain");

		Vector3 standingPosition = standingPositionGameObject.transform.position;
		PlaceFootOnRaycast(standingPosition + Vector3.up * 2, Vector3.down, 0);
    }

    void Update()
    {
		Vector3 rayPosition = rayPositionGameObject.transform.position;
		transform.position = currentPosition;
		PlaceFootOnRaycast(rayPosition + Vector3.up * 2, Vector3.down, stepDistanceThreshhold);
    }

	void PlaceFootOnRaycast(Vector3 rayStartPos, Vector3 normalizedDirection, float stepDistanceThreshhold_)
	{
		Ray ray = new Ray(rayStartPos, normalizedDirection);
		if (Physics.Raycast(ray, out RaycastHit hit, 15, layerMask))
		{
			if (Vector3.Distance(currentPosition, hit.point) > stepDistanceThreshhold_)
			{
				currentPosition = hit.point + new Vector3(0,0.2f,0);
			}
		}
	}
}
