using Unity.VisualScripting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    PlayerStats playerStats;

    public float forwardMoveSpeed = 10f;
    public float backwardMoveSpeed = 5f;
    public float mouseSensitivity = 2f;

    private Transform cameraTransform;
    private float verticalRotation = 0f;
    [HideInInspector] public Vector3 moveDirection;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        SetupCamera();
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (playerStats.canMove)
                BasicMovement();
        if (playerStats.canTurn)
            CameraMovement();
    }

    public void BasicMovement(Vector3 moveDirection_ = default(Vector3), float forwardMoveSpeed_ = default(float))
    {
        moveDirection = moveDirection_; // Same as doing moveDirection = zero, but if a custom direction was used it'd use that
        if (forwardMoveSpeed_ == default(float)) { forwardMoveSpeed_ = forwardMoveSpeed; } // If no custom move speed was used, use preset speed

        if (playerStats.canMove)
            CheckMovementInputs();

        playerStats.isRunning = moveDirection != Vector3.zero; // If moveDirection doesn't equal zero set isRunning in PlayerStats to true

        if (GetLocalMovementDirectionNormalized().z >= 0f) // If ur moving forward
            transform.position += moveDirection.normalized * forwardMoveSpeed_ * Time.deltaTime;
        else if (GetLocalMovementDirectionNormalized().z < 0f) // If ur moving backward
            transform.position += moveDirection.normalized * backwardMoveSpeed * Time.deltaTime;

    }

    public void BasicMovementTowards(Vector3 targetLocation, float forwardMoveSpeed_ = default(float))
    {
        moveDirection = Vector3.zero;
        if (forwardMoveSpeed_ == default(float)) { forwardMoveSpeed_ = forwardMoveSpeed; } // If no custom move speed was used, use preset speed
        moveDirection = NormalizedDirectionTowardsTarget(transform.position, targetLocation);

        playerStats.isRunning = moveDirection != Vector3.zero;
        if (GetLocalMovementDirectionNormalized().z >= 0f) // If ur moving forward
            transform.position = Vector3.MoveTowards(transform.position, targetLocation, forwardMoveSpeed_ * Time.deltaTime);
        else if (GetLocalMovementDirectionNormalized().z < 0f) // If ur moving backward
            transform.position = Vector3.MoveTowards(transform.position, targetLocation, backwardMoveSpeed * Time.deltaTime);
    }

    void CheckMovementInputs()
    {
        if (Input.GetKey(KeyCode.W))
            moveDirection += transform.forward;
        if (Input.GetKey(KeyCode.S))
            moveDirection -= transform.forward;
        if (Input.GetKey(KeyCode.A))
            moveDirection -= transform.right;
        if (Input.GetKey(KeyCode.D))
            moveDirection += transform.right;
    }

    void SetupCamera()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraTransform = Camera.main.transform;
    }

    void CameraMovement()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        // Rotates the PLAYER horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Rotates the CAMERA vertically
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    public float GetMovementDirectionAngle()
    {
        return Vector3.SignedAngle(transform.right, moveDirection, Vector3.up);
        // ^^^ If this equals -90 then the player is running straight forward.
        // ^^^ If this equals 0 then the player is running right
        // ^^^ If this equals 180 then the player is running left
        // ^^^ If this equals 90 then the player is running straight backwards
        // ^^^ If this equals -45 then the player is running forward-right
        // ^^^ If this equals -135 then the player is running forward-left
        // ^^^ If this equals 45 then the player is running backwards-right
        // ^^^ If this equals 135 then the player is running backwards-left
    }

    public Vector3 GetLocalMovementDirectionNormalized()
    {
        // Convert world moveDirection into local space (relative to player facing)

        return transform.InverseTransformDirection(moveDirection.normalized);

        // If we ignore the y value for localMove...
        // localMove being (0,1) means forward
        // localMove being (0,-1) means backward
        // localMove being (-1,0) means left
        // localMove being (1,0) means right
        // etc...
    }

    // The following code was yoinked directly from Unity Documentation
    // https://docs.unity3d.com/2018.3/Documentation/Manual/DirectionDistanceFromOneObjectToAnother.html
    public Vector3 NormalizedDirectionTowardsTarget(Vector3 startPoint, Vector3 targetPoint)
    {
        var heading = targetPoint - startPoint;
        var distance = heading.magnitude;
        var direction = heading / distance; // This is now the normalized direction.
        return direction;
    }
}
