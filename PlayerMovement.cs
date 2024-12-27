using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Animations")]
    public Animator animator;
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float rotationSpeed;
    public float rotationVelocity;
    public float turnAngle;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("SlowWalking")]
    public float slowWalkSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode walkKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public Transform castObj;
    public float sphereSize;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;
    

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        slowWalking,
        stationary,
        air
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

    }

    private void Update()
    {
        // ground check
        grounded = Physics.SphereCast(castObj.transform.position, sphereSize, -transform.up, out var hit, playerHeight, whatIsGround);
        MyInput();
        SpeedControl();
        StateHandler();
        if(horizontalInput != 0 || verticalInput != 0 && grounded)
            {
                float targetAngle = Mathf.Atan2(horizontalInput,verticalInput) * Mathf.Rad2Deg + orientation.eulerAngles.y;
                turnAngle = Mathf.SmoothDampAngle(turnAngle, targetAngle, ref rotationVelocity, rotationSpeed, 400);
                rb.MoveRotation(Quaternion.Euler(0f, turnAngle, 0f));
                
            }
        // handle drag
        if (grounded)
        {
            animator.SetBool("isFalling", false);
            animator.SetFloat("Speed", rb.velocity.magnitude);
            rb.drag = groundDrag;
        }
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void StateHandler()
    {
        // Mode - Slow Walking
        if (grounded && Input.GetKey(walkKey) && (horizontalInput != 0 || verticalInput != 0))
        {
            state = MovementState.slowWalking;
            moveSpeed = slowWalkSpeed;
            animator.SetBool("isMoving", true);
        }

        // Mode - Sprinting
        else if(grounded && Input.GetKey(sprintKey) && (horizontalInput != 0 || verticalInput != 0))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
            animator.SetBool("isMoving", true);
        }

        // Mode - Walking
        else if (grounded && (horizontalInput != 0 || verticalInput != 0))
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
            animator.SetBool("isMoving", true);
        }

        else if(grounded && horizontalInput == 0 && verticalInput == 0) //Stationary
        {
            state = MovementState.stationary;
            moveSpeed = 0;
            animator.SetBool("isMoving", false);
        }
        // Mode - Air
        else
        {
            state = MovementState.air;
            animator.SetBool("isFalling", true);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = transform.forward;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 100f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 50f, ForceMode.Force);

        // in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 50f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        animator.SetFloat("Speed", moveSpeed);
        animator.SetBool("isJumping", true);
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        Invoke(nameof(NotJumping), 0.5f);
    }
    private void NotJumping()
    {
        animator.SetBool("isJumping", false);
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if(Physics.Raycast(castObj.transform.position, -transform.up, out var slopeHit, playerHeight + 0.15f, whatIsGround))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}