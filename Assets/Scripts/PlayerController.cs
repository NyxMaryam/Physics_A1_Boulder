using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Movement
        //ObjectRefs
        public Transform PlayerCamera;
        public Transform Orientation;
        public Transform Head;
        public Rigidbody PlayerRigidbody;
        public LayerMask WhatIsGround;

        //Vars
        public float mouseSensitivity;
        public float accelSpeed;
        public float maxSpeed;
        public float frictionStrength;
        float xRotation;
        
    //Input
        float xInput, yInput, mouseX, mouseY;
        bool jumpInput;
        bool crouchInput;

    private void Start()
    {
        //TEMP: set input map
        KeyMappings.move_forward = KeyCode.W;
        KeyMappings.move_backward = KeyCode.S;
        KeyMappings.move_left = KeyCode.A;
        KeyMappings.move_right = KeyCode.D;
        KeyMappings.move_jump = KeyCode.Space;
        KeyMappings.move_crouch = KeyCode.LeftControl;
    }

    private void Update()
    {
        FetchInputs();
        CameraControl();
        Movement();
    }

    void CameraControl()
    {
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        PlayerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        Orientation.Rotate(Vector3.up * mouseX);

    }
    void FetchInputs()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xInput = (Input.GetKey(KeyMappings.move_forward) ? 1 : 0) + (Input.GetKey(KeyMappings.move_backward) ? -1 : 0);
        yInput = (Input.GetKey(KeyMappings.move_left) ? -1 : 0) + (Input.GetKey(KeyMappings.move_right) ? 1 : 0);

        jumpInput = Input.GetKey(KeyMappings.move_jump);
        crouchInput = Input.GetKey(KeyMappings.move_crouch);
    }
    void Movement()
    {
        Vector3 relativeVelocity = Quaternion.Inverse(Orientation.rotation) * PlayerRigidbody.linearVelocity;
        Debug.Log(relativeVelocity);

        //check if player mag is too fast
        //cancel inputs going in too fast direction
        //move player in forward direction by xInput
        PlayerRigidbody.AddForce(Orientation.forward * xInput * accelSpeed * Time.deltaTime);
        //move player in right direction by yInput
        PlayerRigidbody.AddForce(Orientation.right * yInput * accelSpeed * Time.deltaTime);
        
        //friction for directions not being inputted but still being moved in

        
        //PlayerRigidbody.AddForce(Orientation.forward * ())
    }
}
