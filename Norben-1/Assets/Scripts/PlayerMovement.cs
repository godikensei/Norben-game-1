using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {


    public float walkingSpeed = 2;
    public float runSpeed = 6;
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

        cameraT = Camera.main.transform;
    }
	
	// Update is called once per frame
	void Update () {

        //Setting up inputs
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        //Storing input and normalize it
        Vector2 input = new Vector2(h, v);
        Vector2 inputDir = input.normalized;

        //Checking if the character is moving
        if (h != 0f || v != 0f)
        {
            currentSpeed = walkingSpeed;
        }
        else if(h == 0f || v == 0f)
        {
            currentSpeed = 0f;
        }

        //Checking if its pressing shift or nah
        bool running = Input.GetKey(KeyCode.LeftShift);

        //Setting currentspeed according to running
        currentSpeed = ((running) ? runSpeed : walkingSpeed) * inputDir.magnitude;

        //Moves tthe character forward with the speed of the current speed
        transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

        //Setting the blend tree parameters
        float animationSpeedPercent = ((running) ? 1f : .5f) * inputDir.magnitude;
        anim.SetFloat("speedPercent", animationSpeedPercent);


        //Checking if the character is grounded or not
        groundCollisions = Physics.OverlapSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (groundCollisions.Length > 0)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        Debug.Log(grounded);
        //***************************

        //Jump if grounded
        if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            grounded = false;
            //anim.SetBool("grounded", grounded);
            playerRigidbody.AddForce(new Vector3(0, jumpHeight, 0));
        }

        //sword attack
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            anim.SetTrigger("Attack");
        }

        //Hold sword with right click
        if (Input.GetButtonDown("Fire2"))
        {
            anim.SetBool("holdingSword", true);
        }

        //Lets go the sword
        if (Input.GetButtonUp("Fire2"))
        {
            anim.SetBool("holdingSword", false);
        }

        //Calling walking animation and turning function
        Animating(h, v);
        Turning(inputDir);

    }


    void Turning(Vector2 inputDir)
    {
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
