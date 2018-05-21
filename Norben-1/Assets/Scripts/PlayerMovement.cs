using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private CharacterController controller;

    public float walkingSpeed = 2;
    public float jumpForce = 7;
    public float gravity = -12;
    float currentSpeed;
    float velocityY;

    bool grounded = false;
    Collider[] groundCollisions;
    float groundCheckRadius = 1f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float jumpHeight;

    Vector3 movement;
    Animator anim;
    Rigidbody playerRigidbody;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    int floorMask;
    float camRayLength = 100f;

    Transform cameraT;

    // Use this for initialization
    void Awake () {
        anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();

        cameraT = Camera.main.transform;
    }
	
	// Update is called once per frame
	void Update () {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");


        if(h != 0f || v != 0f)
        {
            currentSpeed = walkingSpeed;
        }
        else if(h == 0f || v == 0f)
        {
            currentSpeed = 0f;
        }
        transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);
        //Vector3 velocity = transform.forward * currentSpeed;

        //controller.Move(velocity * Time.deltaTime);

        //velocityY += Time.deltaTime * gravity;

        //Move(h, v);
        Animating(h, v);
        Turning(h, v);

        groundCollisions = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (groundCollisions.Length > 0)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        anim.SetBool("grounded", grounded);

        Debug.Log(grounded);

        if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            grounded = false;
            anim.SetBool("grounded", grounded);
            playerRigidbody.AddForce(new Vector3(0, jumpHeight, 0));
        }

    }

    void isGrounded()
    {

    }

   /* private void Jump()
    {
        if (controller.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocityY = jumpForce;
            }

        } else
        {
            velocityY -= gravity * Time.deltaTime;
        }

        Vector3 moveVector = new Vector3(0, velocityY, 0);
        controller.Move(moveVector * Time.deltaTime);
    }*/

    void Move(float h, float v)
    {
        movement.Set(h, 0f, v);
        //A h és v-t ha egyszerre nyomjuk, akkor 1.4 lesz, ezért normalizálni kell
        movement = movement.normalized * walkingSpeed * Time.deltaTime;

        playerRigidbody.MovePosition(transform.position + movement);
    }

    void Turning(float h , float v)
    {
        Vector2 input = new Vector2(h, v);
        Vector2 inputDir = input.normalized;

        if(inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }
    }

    void Animating(float h, float v)
    {
        bool walking = h != 0f || v != 0f;

        anim.SetBool("IsWalking", walking);
    }

}
