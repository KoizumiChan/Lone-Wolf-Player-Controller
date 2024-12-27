using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PhysicalCC : MonoBehaviour
{
	public CharacterController cc { get; private set; }
	private IEnumerator dampingCor;

	[Header("Ground Check")]
	public bool isGround;
	public float groundAngle;
	public float groundAngleRelative;
	public Vector3 groundNormal { get; private set; }

	[Header("Movement")]
	public bool ProjectMoveOnGround;
	public Vector3 moveInput;
	private Vector3 moveVelocity;

	[Header("Slope and inertia")]
	public float slopeLimit = 45;
	public float inertiaDampingTime = 0.1f;
	public float slopeStartForce = 3f;
	public float slopeAcceleration = 3f;
	public Vector3 inertiaVelocity;
	public Transform castFront;
	public Transform castRear;
	public Transform castMid;
	public float raycastHeight;

	[Header("interaction with the platform")]
	public bool platformAction;
	public Vector3 platformVelocity;

	[Header("Collision")]
	public bool applyCollision = true;
	public float pushForce = 55f;

	private void Start()
	{
		cc = GetComponent<CharacterController>();
	}

	private void Update()
	{
		GroundCheck();

		if (isGround)
		{
			moveVelocity = ProjectMoveOnGround? Vector3.ProjectOnPlane (moveInput, groundNormal) : moveInput;

			if (groundAngle < slopeLimit && inertiaVelocity != Vector3.zero) InertiaDamping();
		}

		GravityUpdate();

		Vector3 moveDirection = (moveVelocity + inertiaVelocity + platformVelocity);

		cc.Move((moveDirection) * Time.deltaTime);
	}

	private void GravityUpdate()
	{
		if (isGround && groundAngle > slopeLimit)
		{
			inertiaVelocity += Vector3.ProjectOnPlane(groundNormal.normalized + (Vector3.down * (groundAngle / 30)).normalized * Mathf.Pow(slopeStartForce, slopeAcceleration), groundNormal) * Time.deltaTime;
		}
		else if (!isGround) inertiaVelocity.y -= Mathf.Pow(3f, 3) * Time.deltaTime;
	}

	private void InertiaDamping()
	{
		var a = Vector3.zero;

		//inertia braking when the force of movement is applied
		var resistanceAngle = Vector3.Angle(Vector3.ProjectOnPlane(inertiaVelocity, groundNormal),
		Vector3.ProjectOnPlane(moveVelocity, groundNormal));

		resistanceAngle = resistanceAngle == 0 ? 90 : resistanceAngle;

		inertiaVelocity = (inertiaVelocity + moveVelocity).magnitude <= 0.1f ? Vector3.zero : Vector3.SmoothDamp(inertiaVelocity, Vector3.zero, ref a, inertiaDampingTime / (3 / (180 / resistanceAngle)));
	}

	public bool GroundCheck()
	{
		if (Physics.SphereCast(transform.position, cc.radius, Vector3.down, out RaycastHit hit, cc.height / 2 - cc.radius + 0.03f))
		{
			isGround = true;
			if(Physics.BoxCast(castMid.position, new Vector3(cc.radius / 2.5f, cc.radius / 3f, cc.radius / 2.5f), Vector3.down, out RaycastHit hitAngle, transform.rotation, raycastHeight))
			{
				groundNormal = hitAngle.normal;
			}
			else
			{
				groundNormal = Vector3.up;
			}
			groundAngle = Vector3.Angle(Vector3.up, hit.normal);
			//Physics.SphereCast(castFront.position, cc.radius, Vector3.down, out RaycastHit frontHit, raycastHeight + 0.03f);
			//Physics.SphereCast(castRear.position, cc.radius, Vector3.down, out RaycastHit rearHit, raycastHeight + 0.03f);
			

			if (hit.transform.tag == "Platform")
				platformVelocity = hit.collider.attachedRigidbody == null | !platformAction ?
				 Vector3.zero : hit.collider.attachedRigidbody.velocity;

			if (Physics.BoxCast(transform.position, new Vector3(cc.radius / 2.5f, cc.radius / 3f, cc.radius / 2.5f),Vector3.down, out RaycastHit helpHit, transform.rotation, cc.height / 2 - cc.radius / 2))
			{
				groundAngle = Vector3.Angle(Vector3.up, helpHit.normal);
			}
			return true;
		}
		else
		{
			platformVelocity = Vector3.zero;
			isGround = false;
			return false;
		}
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (!applyCollision) return;

		Rigidbody body = hit.collider.attachedRigidbody;

		// check rigidbody
		if (body == null || body.isKinematic) return;

		Vector3 pushDir = hit.point - (hit.point + hit.moveDirection.normalized);

		// Apply the push
		body.AddForce(pushDir * pushForce, ForceMode.Force);
	}

}
