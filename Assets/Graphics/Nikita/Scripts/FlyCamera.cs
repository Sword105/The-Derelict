using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;               // Units per second
    public float sprintMultiplier = 1.8f;      // Hold Left Ctrl to move faster (optional)
    public bool worldUpForVertical = true;     // Use world Y for up/down (recommended)

    [Header("Mouse Look")]
    public float mouseSensitivity = 2.0f;      // Higher = faster look
    public float pitchMin = -89f;
    public float pitchMax = 89f;

    [Header("Cursor")]
    public bool lockCursorOnStart = true;

    float yaw;
    float pitch;

    void Start()
    {
        var e = transform.rotation.eulerAngles;
        yaw = e.y;
        pitch = e.x;

        if (lockCursorOnStart)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        // --- Cursor lock toggle ---
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // --- Mouse look ---
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float mx = Input.GetAxis("Mouse X") * mouseSensitivity;
            float my = Input.GetAxis("Mouse Y") * mouseSensitivity;

            yaw += mx;
            pitch -= my;
            pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        // --- Movement ---
        // WASD (X/Z in camera-local space)
        float x = Input.GetAxisRaw("Horizontal"); // A/D
        float z = Input.GetAxisRaw("Vertical");   // W/S

        // Up/Down: Space / LeftShift (as requested)
        float y = 0f;
        if (Input.GetKey(KeyCode.Space)) y += 1f;                // Up
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) y -= 1f; // Down

        // Optional speed boost on LeftCtrl
        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftControl) ? sprintMultiplier : 1f);

        Vector3 right = transform.right;
        Vector3 forward = transform.forward;
        Vector3 up = worldUpForVertical ? Vector3.up : transform.up;

        // Don’t normalize so diagonals aren’t slower; multiply by dt
        Vector3 delta =
            (right * x + forward * z + up * y) * (speed * Time.deltaTime);

        transform.position += delta;
    }
}
