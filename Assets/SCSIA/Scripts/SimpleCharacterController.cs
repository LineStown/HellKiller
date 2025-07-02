using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SimpleCharacterController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float lookSpeed = 2f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private Camera mainCamera;
    private float cameraPitch = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;

        if (mainCamera != null)
        {
            mainCamera.transform.SetParent(transform);
            mainCamera.transform.localPosition = new Vector3(0, 1.6f, 0);
            mainCamera.transform.localRotation = Quaternion.identity;
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null)
            return;

        // --- Mouse look ---
        float mouseX = Mouse.current.delta.x.ReadValue() * lookSpeed * Time.deltaTime;
        float mouseY = Mouse.current.delta.y.ReadValue() * lookSpeed * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
        if (mainCamera != null)
            mainCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);

        // --- Movement input ---
        Vector2 moveInput = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
        if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
        if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
        if (Keyboard.current.dKey.isPressed) moveInput.x += 1;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move.normalized * moveSpeed * Time.deltaTime);

        // --- Jump & gravity ---
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
            velocity.y = jumpForce;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
