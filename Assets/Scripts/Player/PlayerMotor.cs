using UnityEngine;
using System.Collections;

public class PlayerMotor : MonoBehaviour {

	//public static Player Instance;
	
	public bool isPlatformer = false;
	public bool isLocked = false;
	
	public Action CurrentAction { get; set; }
	
	public enum Action
	{
		None = 0,
		Jump = 1,
		Attack = 2,
	}
	
	//public float MoveSpeed = 10f;
	public float TurnSpeed = 16f;
	public float AirTurnSpeed = 1f;
	public float Speed = 10f;
	public float Acceleration = 2f;
	public float Deceleration = 2f;
	public float ReverseAcceleration = 2f;
	public float AirAcceleration = 2f;
	public float ForwardSpeed = 10f;
	public float BackwardSpeed = 2f;
	public float StrafingSpeed = 5f;
	public float SlideSpeed = 10f;
	public float JumpHeightMax = 1.5f;
	public float JumpHeightMin = 0.75f;
	public float Gravity = 21f;
	public float AirDrag = 0.05f;
	public float TerminalVelocity = 20f;
	public float SlideThreshold = 0.6f;
	public float MaxControllableSlideMagnitiude = 0.4f;
	
	private Vector3 slideDirection;
	private Vector3 inputVector;
	private Quaternion camRotation;
		
	//private bool isGrounded;
	private float jumpSpeed;
	private float jumpCutSpeed;
	
	public Vector3 MoveVector { get; set; }
	public Vector2 RelativeInputVector { get; set; }
	public float VerticalVelocity { get; set;}
	public Vector3 VelocityXZ { get; set; }
	
	public Direction MoveDirection { get; set; }

    public bool isGrounded { get; set; }
	
	public enum Direction
	{
		Idle = 0,
		Forward = 1,
		Back = 2,
		Right = 3,
		ForwardRight = 4,
		BackRight = 5,
		Left = 6,
		ForwardLeft =7,
		BackLeft = 8,
	}
	
	public OrientTo RotationMode { get; set; }
	
	public enum OrientTo
	{
		None,
		Input,
		Velocity,
		Camera,
	}
	
	//private Vector3 velocity = Vector3.zero;
	private float currentRotationY = 0f;
	private float desiredRotationY = 0f;
	private float previousDesiredRotationY = 0f;
	private Quaternion defaultRotation = new Quaternion();
	
	private CharacterController controller;
	
	void Awake()
	{
		controller = GetComponent<CharacterController>();
		isGrounded = controller.isGrounded;
	}
	
	void SetRotation(OrientTo mode)
	{
		float turnSpeed = isGrounded ? TurnSpeed : AirTurnSpeed;
		switch(mode)
		{
			case OrientTo.None:
				goto default;
			case OrientTo.Velocity:
				if (controller.velocity.x > 0 || controller.velocity.z > 0)
				{
					desiredRotationY = Quaternion.LookRotation(controller.velocity, Vector3.up).eulerAngles.y;
					break;
				}
				goto default;
			case OrientTo.Input:
				if (inputVector.sqrMagnitude > 0.01f)
				{
					desiredRotationY = Quaternion.LookRotation(camRotation * inputVector, Vector3.up).eulerAngles.y;
					break;
				}
				goto default;
			case OrientTo.Camera:
				desiredRotationY = camRotation.eulerAngles.y;
				break;
			default:
				desiredRotationY = transform.rotation.eulerAngles.y;
				return;	
		}
		currentRotationY = Helper.SuperSmoothLerpAngle(currentRotationY, ref previousDesiredRotationY, desiredRotationY, Time.deltaTime * turnSpeed);
		transform.rotation = Quaternion.Euler(0, currentRotationY, 0);
	}
	
	public void ProcessMotion()
	{

        jumpSpeed = JumpSpeed(JumpHeightMax);
        jumpCutSpeed = JumpSpeed(JumpHeightMin);

        //isGrounded = controller.isGrounded;

        //Gravity = -Physics.gravity.y;

        isLocked = Input.GetAxis("Left Trigger") > 0;

        camRotation = Camera.main ? Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) : defaultRotation;

        

        inputVector = new Vector3(Player.Input.leftAnalog.x, 0, Player.Input.leftAnalog.y);

        //RotationMode = isGrounded ? OrientTo.Input : OrientTo.None;

        RotationMode = !isGrounded ? OrientTo.None :
                          isLocked ? OrientTo.Camera : OrientTo.Input;

        SetRotation(RotationMode);

		float deltaTime = Time.deltaTime;
		float gravity = Gravity * deltaTime;
		// * deltaTime;
		//float acc = isGrounded ? Acceleration : 0f;// * deltaTime;
		
		Vector3 goalVelocity = camRotation * SetGoalVelocity();
		float acceleration = SetAcceleration(goalVelocity);
		
		MoveVector = new Vector3(Mathf.Lerp(controller.velocity.x, goalVelocity.x, acceleration * deltaTime), VerticalVelocity,
		                         Mathf.Lerp(controller.velocity.z, goalVelocity.z, acceleration * deltaTime));

		// Move the Character in World Space
		isGrounded = (controller.Move(MoveVector * deltaTime) & CollisionFlags.Below) != 0;
		
		ApplyGravity(gravity);
		
		VerticalVelocity = MoveVector.y;
		
		VelocityXZ = new Vector3(controller.velocity.x, 0, controller.velocity.z);
				
		Debug.DrawLine(transform.position, transform.position + MoveVector * 0.1f, Color.red, 0, false);
	}
	
	float SetAcceleration(Vector3 goalVelocity)
	{
		if (isGrounded)
		{
			float facingRatio = Vector3.Dot(goalVelocity.normalized,VelocityXZ.normalized);
			
			if (facingRatio < 0f)
				return Mathf.Lerp(Acceleration, ReverseAcceleration, -facingRatio);
			if (facingRatio >= 0f && goalVelocity.sqrMagnitude < VelocityXZ.sqrMagnitude)
				return Deceleration;
			return Acceleration;
		}
		else
			return AirAcceleration;
	}
	
	Vector3 SetGoalVelocity()
	{
		float t = isLocked ? 1 : 0;
		
		Vector3 normalVelocity = inputVector * Speed;
		Vector3 lockVelocity = inputVector;
		
		if (t > 0 && isGrounded)
		{
			lockVelocity.z *= lockVelocity.z < 0 ? BackwardSpeed : ForwardSpeed;
			lockVelocity.x *= StrafingSpeed;
			return t < 1 ? Vector3.Lerp(normalVelocity,lockVelocity,t) : lockVelocity;
		}
		else return normalVelocity;
	}
	
	void ApplyGravity(float gravity)
	{
		if (MoveVector.y > -TerminalVelocity)
			MoveVector = new Vector3(MoveVector.x, MoveVector.y - gravity, MoveVector.z);
		
		if (isGrounded && MoveVector.y < -1)
			MoveVector = new Vector3(MoveVector.x, -1, MoveVector.z);
	}
	
	void ApplySlide()
	{
		if (isGrounded)
			return;
			
		slideDirection = Vector3.zero;
		RaycastHit hitInfo;
		
		if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hitInfo))
		{
			if (hitInfo.normal.y < SlideThreshold)
				slideDirection = new Vector3(hitInfo.normal.x,-hitInfo.normal.y,hitInfo.normal.z);
		}
		
		//Quaternion slopeAngle = Quaternion.FromToRotation(transform.up, hitInfo.normal);
		//Debug.Log (slopeAngle.eulerAngles);
		
		if (slideDirection.magnitude < MaxControllableSlideMagnitiude)
			MoveVector += slideDirection;
		else
			MoveVector = slideDirection;
			
	}
	
	float JumpSpeed(float jumpHeight)
	{
		return Mathf.Sqrt(jumpHeight * Gravity * 2f);
	}
	
	public void TryJump()
	{
		if (isGrounded)
		{
			VerticalVelocity = jumpSpeed;
		}
	}
	
	public void EndJump()
	{
		if (!isGrounded && VerticalVelocity > jumpCutSpeed)
		{
			VerticalVelocity = jumpCutSpeed;
		}
	}
	
	public void Attack()
	{
		CurrentAction = PlayerMotor.Action.Attack;
	}
	
	Direction SetMoveDirection()
	{
		//45 deg = 4 sprites, 22.5 deg = 8 sprites
		float threshold = Mathf.Sin(22.5f * Mathf.Deg2Rad);
		
		int dir = 0;
		int up = 1;
		int down = 2;
		int right = 3;
		int left = 6;
		
		if (inputVector.z >= threshold)
			dir += up;
		else if (inputVector.z <= -threshold)
			dir += down;
		
		if (inputVector.x >= threshold)
			dir += right;
		else if (inputVector.x <= -threshold)
			dir += left;
		
		return (Direction)dir;
	}
	
//	float MoveSpeed()
//	{
//		float moveSpeed = 0f;
//		
//		moveSpeed = ForwardSpeed;
//		
//		switch (MoveDirection)
//		{
//			case Direction.Idle:
//				moveSpeed = 0;
//				break;
//			case Direction.Forward:
//			case Direction.ForwardRight:
//			case Direction.ForwardLeft:
//				moveSpeed = ForwardSpeed;
//				break;
//			case Direction.Back:
//			case Direction.BackRight:
//			case Direction.BackLeft:
//				moveSpeed = BackwardSpeed;
//				break;
//			case Direction.Right:
//			case Direction.Left:
//				moveSpeed = StrafingSpeed;
//				break;
//		}
//		
//		if (slideDirection.magnitude > 0)
//			moveSpeed = SlideSpeed;
//		
//		return moveSpeed;
//	}
}
