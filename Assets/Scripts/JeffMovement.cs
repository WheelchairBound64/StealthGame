using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JeffMovement : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float speed;
    Vector2 moveInput;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        var newInput = GetCameraBasedInput(moveInput, Camera.main);     //uses the camera position to adjust movement keys
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        rb.velocity = new Vector3(newInput.x * speed * Time.deltaTime, rb.velocity.y, newInput.y * speed * Time.deltaTime);
    }

    Vector3 GetCameraBasedInput(Vector2 input, Camera cam)          //camera based movement
    {
        Vector3 camRight = cam.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 camForward = cam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        return input.x * camRight + input.y * camForward;
    }
}
