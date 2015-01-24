using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {
	
	//public enum InputDevice {Joystick, Mouse};
	//public InputDevice inputDevice;

    private Player player;

	public Transform PlayerTransform;	
	
	public Vector3 Offset = new Vector3(0f,1.67f,0);
	public float DistanceDefault = 3f;
	public float DistanceMin = 1f;
	public float DistanceMax = 10f;
	public float DistanceSmooth = 0.05f;
	public float DistanceResumeSmooth = 0.4f;
	public float MouseWheelSensitivity = 0.55f;
	public float AimVerticalDefault = 20f;
	public float AimVerticalMin = -40f;
	public float AimVerticalMax = 50f;
	public float AimSensitivityX = 1f;
	public float AimSensitivityY = 0.8f;
	public float RotationSmooth = 20f;
	public float PositionSmoothIn = 8f;
	public float PositionSmoothOut = 2f;
	
	public float horizontalDrag = 0.2f;
	public float upDrag = 0.5f;
	public float downDrag = 0.5f;
	public float forwardDrag = 0.5f;
	public float backwardDrag = 0.5f;

    public bool invertY;
	
	private Vector3 rotation = Vector3.zero;
	private Vector3 desiredRotation = Vector3.zero;
	private Vector3 lastRotation = Vector3.zero;
		
	private Vector3 position = Vector3.zero;
	private Vector3 trackPosition = Vector3.zero;
	private Vector3 lastPosition = Vector3.zero;
	
	private float velDistance = 0f;
	private float distance = 0f;
	private float newDistance = 0f;
	private float distanceSmooth = 0f;
	private float preOccludedDistance = 0;

    private Vector3 lookPosition = Vector3.zero;
	
	void Start()
	{
        //player = GameObject.FindGameObjectWithTag(Tags.player).GetComponent<Player>() as Player;
		Reset();
	}
	
	public void Reset()
	{
        if (PlayerTransform == null)
            //PlayerTransform = GameObject.FindGameObjectWithTag(Tags.player).transform;
            PlayerTransform = Player.Instance.transform;

		rotation.x = AimVerticalDefault;
		rotation.y = PlayerTransform.rotation.eulerAngles.y;
		lastRotation = desiredRotation = rotation;
		distance = newDistance = preOccludedDistance = DistanceDefault;
		lookPosition = trackPosition = position = lastPosition = PlayerTransform.position;
		transform.position = SetCameraPosition(distance);
		transform.LookAt(lookPosition);
	}
	
	void LateUpdate()
	{
		if (PlayerTransform == null) return;
		
		float mouseY = Player.Input.rightAnalog.y;
        float mouseX = Player.Input.rightAnalog.x;
		float zoom = Input.GetAxis("Mouse ScrollWheel");
		HandleInput(mouseX,mouseY,zoom);
		
		CalculateNewPosition();
		CheckOcclusion();
		UpdatePosition();
	}

	void HandleInput(float mouseX, float mouseY, float zoom)
	{
		//int mouseIsEnabled = inputDevice == InputDevice.Mouse ? 1: 0;

        desiredRotation.y += mouseX * AimSensitivityX;
        desiredRotation.x += mouseY * AimSensitivityY;
        desiredRotation.x *= invertY ? -1 : 1;
			
		desiredRotation.x = Helper.ClampAngle(desiredRotation.x, AimVerticalMin, AimVerticalMax);
		desiredRotation.y = Helper.WrapAngle(desiredRotation.y);
		
		if (zoom != 0)
		{
			if (newDistance < preOccludedDistance)
				preOccludedDistance = newDistance;
			preOccludedDistance -= zoom * MouseWheelSensitivity;
			preOccludedDistance = Mathf.Clamp(preOccludedDistance, DistanceMin, DistanceMax);
			newDistance = preOccludedDistance;
			distanceSmooth = DistanceSmooth;
		}
	}
	
	void CalculateNewPosition()
	{
		float deltaTime = Time.deltaTime;
		float posSmooth = Mathf.Lerp(PositionSmoothOut, PositionSmoothIn, Player.Motor.MoveVector.magnitude);
		
		rotation.x = Helper.SuperSmoothLerpAngle(rotation.x, ref lastRotation.x, desiredRotation.x, RotationSmooth * deltaTime);
		rotation.y = Helper.SuperSmoothLerpAngle(rotation.y, ref lastRotation.y, desiredRotation.y, RotationSmooth * deltaTime);
		position = Helper.SuperSmoothLerp(position, ref lastPosition, PlayerTransform.position, posSmooth * deltaTime);
		
		trackPosition = DragModifier(horizontalDrag, upDrag, downDrag, forwardDrag, backwardDrag);
		lookPosition = DragModifier(horizontalDrag, upDrag * 0.25f, 0f, forwardDrag, backwardDrag);
	}
	
	Vector3 DragModifier(float h, float u, float d, float f, float b)
	{
		Vector3 deltaPos = Quaternion.Euler(0, -rotation.y, 0) * (position - PlayerTransform.position);
		deltaPos.x *= h;
		deltaPos.y *= deltaPos.y < 0 ? u : d;
		deltaPos.z *= deltaPos.z < 0 ? f : b;
		return PlayerTransform.position + Quaternion.Euler(0, rotation.y, 0) * (deltaPos + Offset);
	}
	
	Vector3 SetCameraPosition(float dist)
	{
		return trackPosition + Quaternion.Euler(rotation.x, rotation.y, 0) * Vector3.back * dist;
	}
	
	void UpdatePosition()
	{
		distance = Mathf.SmoothDamp(distance, newDistance, ref velDistance, distanceSmooth);
		transform.position = SetCameraPosition(distance);
		transform.LookAt(lookPosition);
	}
	
	void CheckOcclusion()
	{
		Vector3 pos = SetCameraPosition(preOccludedDistance);
		float nearestDistance = Helper.CheckCameraPoints(camera, trackPosition, pos);
		
		if (nearestDistance != -1)
		{
			distance = nearestDistance - camera.nearClipPlane;
			newDistance = distance;
			distanceSmooth = DistanceResumeSmooth;
		}
		else
			newDistance = preOccludedDistance;
	}
}
