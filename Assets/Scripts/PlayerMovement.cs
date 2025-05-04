using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    
    private Transform cameraTransform;
    private float verticalRotation = 0f;
    public bool isWalking;
    
    void Start()
    {
        SetupCamera();
    }

    void Update()
    {
        BasicMovement();
        CameraMovement();
    }

    void BasicMovement()
    {
        Vector3 moveDirection = Vector3.zero;
        isWalking = false;
        
        if (Input.GetKey(KeyCode.W))
            moveDirection += transform.forward;
        if (Input.GetKey(KeyCode.S))
            moveDirection -= transform.forward;
        if (Input.GetKey(KeyCode.A))
            moveDirection -= transform.right;
        if (Input.GetKey(KeyCode.D))
            moveDirection += transform.right;
        
        if (moveDirection != Vector3.zero)
            isWalking = true;
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
}
