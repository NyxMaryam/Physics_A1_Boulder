using NUnit.Framework;
using System;
using System.Linq;
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
        public LayerMask WhatIsProjectile;

        //Vars
        public bool grounded;
        public int framesSinceGrounded = 0;
        public float mouseSensitivity;
        public float accelSpeed;
        public float maxSpeed;
        //public float frictionStrength;
        float xRotation;

        public float counterMovement;
        public float counterMovementThreshold;
        
    //Input
        float xInput, yInput, mouseX, mouseY;
        bool jumpInput;
        bool crouchInput;

    //Prefabs
        public GameObject BasicRock;

    private void Start()
    {
        //TEMP: set input map
        KeyMappings.move_forward = KeyCode.W;
        KeyMappings.move_backward = KeyCode.S;
        KeyMappings.move_left = KeyCode.A;
        KeyMappings.move_right = KeyCode.D;
        KeyMappings.move_jump = KeyCode.Space;
        KeyMappings.move_crouch = KeyCode.LeftControl;

        //TEMP: mouse look gooderer
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        FetchInputs();
        CameraControl();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit groundCheck1Out;
            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out groundCheck1Out, 10f, WhatIsProjectile))
            {
                if(groundCheck1Out.collider.GetComponent<Projectile>() != null)
                {
                    Projectile hitProjectile = groundCheck1Out.collider.GetComponent<Projectile>();
                    hitProjectile.ProjectileRigidbody.linearVelocity = Vector3.zero;
                    hitProjectile.ProjectileRigidbody.AddForce((Camera.main.transform.forward * 3000f) + Vector3.up * 400f);
                }
            }
            else if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out groundCheck1Out, 10f, WhatIsGround))
            {
                Projectile newRock = Projectile.SpawnProjectile(groundCheck1Out.point + new Vector3(0, 1, 0), Orientation.rotation, "Basic Rock");
                
                newRock.ProjectileRigidbody.AddForce(Vector3.up * 800);
            }
        }
    }

    private void FixedUpdate()
    {
        framesSinceGrounded++;
        if (framesSinceGrounded > 1)
            grounded = false;

        
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
        //Debug.Log(relativeVelocity);

        CounterMovement(yInput, xInput, relativeVelocity);

        //check if player mag is too fast
        //cancel inputs going in too fast direction
        if ((relativeVelocity.z > maxSpeed && xInput > 0f) || (relativeVelocity.z < -maxSpeed && xInput < 0f))
            xInput = 0f;
        if ((relativeVelocity.x > maxSpeed && yInput > 0f) || (relativeVelocity.x < -maxSpeed && yInput < 0f))
            yInput = 0f;
        //move player in forward direction by xInput
        PlayerRigidbody.AddForce(Orientation.forward * xInput * accelSpeed * Time.deltaTime);
        //move player in right direction by yInput
        PlayerRigidbody.AddForce(Orientation.right * yInput * accelSpeed * Time.deltaTime);

        //friction for directions not being inputted but still being moved in
        //PlayerRigidbody.AddForce(relativeVelocity * frictionStrength * Time.deltaTime);

        //PlayerRigidbody.AddForce(Orientation.forward * ())
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.collider.gameObject.layer == 6)
        {  
            if (Vector3.Dot(Vector3.up, collision.contacts[0].normal) > 0.5f)
            {
                grounded = true;
                framesSinceGrounded = 0;
            }
        }
    }

    private void CounterMovement(float x, float y, Vector3 mag)
    {
        if (!grounded || Input.GetKey(KeyMappings.move_jump)) return;

        //Debug.Log(x + " : " + y + " : " + mag);

        //Counter movement
        //
        //if (the absolute value of mag.x is greater than our set threshold and the absolute value of our x input less than 0.05) OR 
        //   (mag.x is less than our negative threshold and x input is greater than 0) OR
        //   (mag.x is greater than our threshold and our x input is less than 0)
        //then apply a rigidbody force equal to (moveSpeed * speedMult * player's local right direction * Time.deltaTime * negative x magnitude)
        if (Math.Abs(mag.x) > counterMovementThreshold && Math.Abs(x) < 0.05f || (mag.x < -counterMovementThreshold && x > 0) || (mag.x > counterMovementThreshold && x < 0))
        {
            Debug.Log("X Activated");
            PlayerRigidbody.AddForce(accelSpeed * Orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        else
        {
            Debug.Log("No X Activated");
            Debug.Log(mag.x + " : " + Math.Abs(x) + " : " + xInput);
        }

        //same for y value
        if (Math.Abs(mag.z) > counterMovementThreshold && Math.Abs(y) < 0.05f || (mag.z < -counterMovementThreshold && y > 0) || (mag.z > counterMovementThreshold && y < 0))
        {
            //Debug.Log("Y Activated");
            PlayerRigidbody.AddForce(accelSpeed * Orientation.transform.forward * Time.deltaTime * -mag.z * counterMovement);
        }

        //Limit diagonal running.
        if (Mathf.Sqrt((Mathf.Pow(PlayerRigidbody.linearVelocity.x, 2) + Mathf.Pow(PlayerRigidbody.linearVelocity.z, 2))) > (maxSpeed))
        {
            float fallspeed = PlayerRigidbody.linearVelocity.y;
            Vector3 n = PlayerRigidbody.linearVelocity.normalized * (maxSpeed);
            PlayerRigidbody.linearVelocity = new Vector3(n.x, fallspeed, n.z);
        }
    }
}
