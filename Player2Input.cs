using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	[Header("Movement")]
	public float speed;
	public float moveSpeedSmooth = 5;
	private	float smoothVelocity;

	public float walkSpeed;
	public float canterSpeed;
	public float sprintSpeed;
	public float sneakSpeed;

	public float attackRange = 1f;
    public int attackDamage = 40;
    public LayerMask attackMask; // Set this to the "Attackable" layer in the inspector
    public KeyCode attackKey = KeyCode.Mouse0; // Left mouse button by default, can be changed


	public float jumpHeight = 15;
	private float input;
	private float newSpeed;
	private float turnAngle;
	public float alignSpeed;
	public float maxAlignAngle;
	public float rotationSpeed;
	private float rotationVelocity;
	private Vector3 velocity = Vector3.zero;
	private float horizontalInput;
	private float verticalInput;

	public float stamina = 100;
	public float sprintStaminaCost;
	public float staminaRecharge;


	public float health = 100;
	public float healthRegen;
	public float healthRegenCost;

	public float hunger = 100;
	public float thirst = 100;
	public float healthCost;
	public float hungerdown;
	public float thirstdown;



	[Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode walkKey = KeyCode.C;
	public KeyCode sneakKey = KeyCode.LeftControl;

	public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        cantering,
        stationary,
        air,
		sneaking
    }
	[Header("GameObjects")]
	public PhysicalCC physicalCC;
	public Transform bodyRender;
	public Transform cameraTransform;
	public Transform playerController;
	public Transform playerModel;
	public Animator animator;
	private CharacterController controller;
	private Vector3 groundNormal;
	
	private void OnCollisionStay(Collision collision)
    {
        groundNormal = collision.contacts[0].normal;
    }
	void Start()
	{
		controller = GetComponent<CharacterController>();
	}
	void Update()
	{	
		if (physicalCC.isGround)
		{
			Rotation();
			BarsCheck();
			MoveStatePlayer();
			MovePlayerC();
			CheckJump();
        	if(Input.GetKeyDown(attackKey)) // Checks if the designated attack key is pressed
        	{
        	    Attack();
        	}
		}

	}

	private void Rotation()
	{
		horizontalInput = Input.GetAxisRaw("Horizontal");
		verticalInput = Input.GetAxisRaw("Vertical");
		if (physicalCC.isGround)
		{
			float angle = Vector3.Angle(playerModel.transform.up, physicalCC.groundNormal);

			if (angle < maxAlignAngle)
			{
				// Get the desired rotation from the ground's normal.
				Quaternion toRotation = Quaternion.FromToRotation(transform.up, physicalCC.groundNormal) * transform.rotation;

				// Smoothly rotate towards the desired rotation.
				playerModel.rotation = Quaternion.Slerp(playerModel.rotation, toRotation, alignSpeed * Time.deltaTime);
			}
		}
		if (physicalCC.isGround && (horizontalInput != 0 || verticalInput != 0))
		{
			float targetAngle = Mathf.Atan2(horizontalInput,verticalInput) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
        	turnAngle = Mathf.SmoothDampAngle(turnAngle, targetAngle, ref rotationVelocity, rotationSpeed, 400);
        	playerController.rotation = Quaternion.Euler(transform.rotation.x, turnAngle, transform.rotation.z);
			
		}
	}
	public delegate void PlayerStateHandler(MovementState state);
	public event PlayerStateHandler OnPlayerStateChange;
	private void MoveStatePlayer()
	{
		animator.SetFloat("Speed", speed);
		if(physicalCC.GroundCheck() && Input.GetKey(walkKey) && (horizontalInput != 0 || verticalInput != 0)) //Check if walking
		{
			newSpeed = walkSpeed;
			state = MovementState.walking;
			OnPlayerStateChange?.Invoke(state);
			animator.SetBool("isMoving", true);
			animator.SetFloat("Speed", speed);
			animator.SetBool("isFalling", false);
		}
		else if (physicalCC.GroundCheck() && Input.GetKey(sneakKey) && (horizontalInput != 0 || verticalInput != 0)) //Check if sneaking
		{
			newSpeed = sneakSpeed;
			state = MovementState.sneaking;
			OnPlayerStateChange?.Invoke(state);
			animator.SetBool("isMoving", true);
			animator.SetFloat("Speed", speed);
			animator.SetBool("isFalling", false);
		}
		else if(physicalCC.GroundCheck() && Input.GetKey(sprintKey) && (horizontalInput != 0 || verticalInput != 0)) //check if sprinting
		{
			if(state == MovementState.sprinting && stamina > 0)
			{
				newSpeed = sprintSpeed;
				state = MovementState.sprinting;
				OnPlayerStateChange?.Invoke(state);
				animator.SetBool("isMoving", true);
				animator.SetFloat("Speed", speed);
				animator.SetBool("isFalling", false);
			}
			else if (stamina >= 20)
			{
				newSpeed = sprintSpeed;
				state = MovementState.sprinting;
				OnPlayerStateChange?.Invoke(state);
				animator.SetBool("isMoving", true);
				animator.SetFloat("Speed", speed);
				animator.SetBool("isFalling", false);
			}
			else
			{
				newSpeed = canterSpeed;
				state = MovementState.cantering;
				OnPlayerStateChange?.Invoke(state);
				animator.SetBool("isMoving", true);	
				animator.SetFloat("Speed", speed);
				animator.SetBool("isFalling", false);
			}

		}
		else if(physicalCC.GroundCheck() && (horizontalInput != 0 || verticalInput != 0)) //check if cantering
		{
			newSpeed = canterSpeed;
			state = MovementState.cantering;
			OnPlayerStateChange?.Invoke(state);
			animator.SetBool("isMoving", true);	
			animator.SetFloat("Speed", speed);
			animator.SetBool("isFalling", false);
		}
		else if(physicalCC.GroundCheck() == false) //check if in air
		{
			animator.SetBool("isFalling", true);
			OnPlayerStateChange?.Invoke(state);
			newSpeed = 0;
			state = MovementState.air;
		}
		else //stationary
		{
			state = MovementState.stationary;
			OnPlayerStateChange?.Invoke(state);
			newSpeed = 0;
			animator.SetBool("isMoving", false);
			animator.SetBool("isFalling", false);
		}
		if(speed < 0.1 && newSpeed == 0)
		{
			speed = 0;
			return;
		}
		speed = Mathf.SmoothDamp(speed, newSpeed, ref smoothVelocity, moveSpeedSmooth);
	}
	private void MovePlayerC()
	{
		if (physicalCC.isGround && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
		{
			input = 1;
		}
		else if (physicalCC.isGround && speed > 0 && physicalCC.groundAngle < physicalCC.slopeLimit)
		{
			input = 1;
		}
		else
		{
			input = 0;
		}
		physicalCC.moveInput = Vector3.ClampMagnitude(transform.forward * input, 1f) * speed;
	}

	private void CheckJump()
	{
		if(stamina >= 30)
		{
			if (Input.GetKeyDown(KeyCode.Space))
				{
					physicalCC.inertiaVelocity.y = 0f;
					physicalCC.inertiaVelocity.y += jumpHeight;
					animator.SetBool("isJumping", true);
					stamina -= 30;
					Invoke(nameof(JustJumped), 0.5f);
				}
		}
		else
		{

		}

	}
	private void BarsCheck()
	{
		//stamina
		if(stamina <= 100 && state != MovementState.sprinting)
		{
			stamina += staminaRecharge * Time.deltaTime;
		}
		if (state == MovementState.sprinting)
		{
			stamina -= sprintStaminaCost  * Time.deltaTime;
		}
		//health
		if (health > 100)
		{
			health = 100;
		}
		if (health < 0)
		{
			health = 0;
		}
		//thirst
		if (thirst > 100)
		{
			thirst = 100;
		}
		if (thirst < 0)
		{
			thirst = 0;
		}
		//hunger
		if (hunger > 100)
		{
			hunger = 100;
		}
		if (hunger < 0)
		{
			hunger = 0;
		}
		if (state == MovementState.sprinting)
		{
			hunger -= hungerdown * 2 * Time.deltaTime;
			thirst -= thirstdown * 2 * Time.deltaTime;
		}
		else
		{
			hunger -= hungerdown * Time.deltaTime;
			thirst -= thirstdown * Time.deltaTime;
		}
		if (hunger <= 0)
		{
			health = health - healthCost* Time.deltaTime;
		}
		if (thirst <= 0)
		{
			health = health - healthCost* Time.deltaTime;
		}
		if (health < 100 && thirst > 0)
		{
			health = health + healthRegen * Time.deltaTime;
			thirst = thirst - healthRegenCost * Time.deltaTime;
		}
		if (health < 100 && hunger > 0)
		{
			health = health + healthRegen * Time.deltaTime;
			hunger = hunger - healthRegenCost * Time.deltaTime;
		}
	}
	public void IncreaseHunger(float amount)
	{
		hunger += amount;
	}
    private void JustJumped()
    {
        animator.SetBool("isJumping", false);
    }
	void Attack()
	{
		StartCoroutine(PerformAttackOverTime(0.5f, 10));
	}

	IEnumerator PerformAttackOverTime(float duration, int times)
	{
		bool enemyHit = false; // Add this flag

		for (int i = 0; i < times; i++)
		{
			if (enemyHit) // If an enemy has been hit, break the loop
				break;

			float radius = 0.25f;
			if (Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hit, attackRange, attackMask))
			{
				// Try to get a Health component from the hit object or its parent.
				Health health = hit.transform.GetComponentInParent<Health>();
				if (health != null)
				{
					health.Value -= attackDamage; // Here you are using the public Value property
					enemyHit = true; // Set the flag to true
				}
			}

			// Wait for the next attack.
			yield return new WaitForSeconds(duration / times);
		}
	}
}