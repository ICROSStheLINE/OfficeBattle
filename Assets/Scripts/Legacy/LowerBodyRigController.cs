using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerBodyRigController : MonoBehaviour
{
    [SerializeField] GameObject lowerBodyAimTarget;
    float targetDistance = 5f;
    float smoothSpeed = 7.5f; // Higher = snappier, Lower = smoother

    void Update()
    {
        Vector3 inputDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            inputDirection += transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputDirection += -transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputDirection += transform.right;
        }
        if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            inputDirection = -inputDirection;
            inputDirection += transform.forward; // The value is forward and not backwards because the player will have a backwards walking animation (ty shi)
        }

        if (inputDirection == Vector3.zero) // If idling look forward ty shi
        {
            inputDirection = transform.forward;
        }

        inputDirection = inputDirection.normalized;

        // Compute the new target world position!
        Vector3 desiredPosition = transform.position + new Vector3(inputDirection.x * targetDistance, 2, inputDirection.z * targetDistance);

        // Smoothly interpolate to new position
        lowerBodyAimTarget.transform.position = Vector3.Lerp(lowerBodyAimTarget.transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
    }
}
