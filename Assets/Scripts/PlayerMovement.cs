using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    PlayerStats playerStats;

    public float moveSpeed = 5f;
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
        BasicMovement();
        CameraMovement();
    }

    void BasicMovement()
    {
        moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            moveDirection += transform.forward;
        if (Input.GetKey(KeyCode.S))
            moveDirection -= transform.forward;
        if (Input.GetKey(KeyCode.A))
            moveDirection -= transform.right;
        if (Input.GetKey(KeyCode.D))
            moveDirection += transform.right;

        playerStats.isRunning = moveDirection != Vector3.zero; // If moveDirection doesn't equal zero set isRunning in PlayerStats to true
        transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
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
}
