using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JeffMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 moveInput;
    [SerializeField] float speed;

    IA_PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Start()
    {
        playerMovement = new IA_PlayerMovement();
        playerMovement.Player.Enable();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = playerMovement.Player.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        var newInput = GetCameraBasedInput(moveInput, Camera.main);     //uses the camera position to adjust movement keys
        var newVelocity = new Vector3(newInput.x * speed, rb.velocity.y, newInput.z * speed);

        rb.velocity = newVelocity;
    }

    Vector3 GetCameraBasedInput(Vector2 input, Camera cam)          //camera based movement
    {
        Vector3 camRight = cam.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 camUp = cam.transform.up;
        camUp.y = 0;
        camUp.Normalize();

        return input.x * camRight + input.y * camUp;
    }
}
