using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerBodyRigController : MonoBehaviour
{
	[SerializeField] GameObject lowerBodyAimTarget;
	float targetDistance = 5f;
	
    void Start()
    {
		
    }

    void Update()
    {
		Vector3 targetPosition = Vector3.zero;
		if (Input.GetKey(KeyCode.W))
		{
            targetPosition += transform.forward;
		}
        if (Input.GetKey(KeyCode.A))
		{
            targetPosition += transform.right * -1;
		}
        if (Input.GetKey(KeyCode.D))
		{
            targetPosition += transform.right;
		}
        if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
		{
			targetPosition = targetPosition * -1;
            targetPosition += transform.forward; // The y value is 1 and not -1 because the player will have a backwards walking animation (ty shi)
		}
		if (targetPosition == Vector3.zero)
		{
			targetPosition = transform.forward;
		}
		targetPosition = targetPosition.normalized;
		
		lowerBodyAimTarget.transform.position = transform.position + new Vector3(targetPosition.x*targetDistance, 2, targetPosition.z*targetDistance);
    }
}
