using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
	
	// Input
	public float inputForce;
	public float jumpForce;
	private Vector3 worldInput;
	private bool inputJump;
	
	// Movement
	public float playerDamping;
	private Vector3 worldVelocity;
	
	// Chase cam
	public float chaseCamTgtDistance;
	public float chaseCamTgtHeight;
	public float chaseCamSeekSpeed;
	private Vector3 chaseCamPos;
	private Vector3 chaseCamVel;	
	
	// Use this for initialization
	void Start ()
	{
		worldInput = Vector3.zero;
		inputJump = false;
		chaseCamPos = transform.position + Vector3.forward * chaseCamTgtDistance;
		chaseCamPos.y += chaseCamTgtHeight;
		chaseCamVel = Vector3.zero;
	}
	
	void HandleInput()
	{
		// Get the input vector from kayboard or analog stick
		Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		
		if (input != Vector3.zero)
		{
			// Get the length of the directon vector and then normalize it
			// Dividing by the length is cheaper than normalizing when we already have the length anyway
			var directionLength = input.magnitude;
			input = input / directionLength;
			
			// Make sure the length is no bigger than 1
			directionLength = Mathf.Min(1, directionLength);
			
			// Make the input vector more sensitive towards the extremes and less sensitive in the middle
			// This makes it easier to control slow speeds when using analog sticks
			directionLength = directionLength * directionLength;
			
			// Multiply the normalized direction vector by the modified length
			input = input * directionLength;
		}
			
		// Rotate input vector to be in worldspace
		Vector3 camZ = Camera.main.transform.forward;
		camZ.y = 0.0f;
		camZ.Normalize();
		Quaternion camToCharacterSpace = Quaternion.FromToRotation( Vector3.forward, camZ );
		worldInput = (camToCharacterSpace * input);
		
		inputJump = Input.GetButton("Jump");
		if (inputJump)
		{
			worldInput.y = 1.0f;
		}
	}
	
	void ApplyInput()
	{
		Rigidbody body = gameObject.GetComponent<Rigidbody>();
		
		Vector3 force = worldInput;
		force.x *= inputForce;
		force.z *= inputForce;
		force.y *= jumpForce;
		
		Vector3 xzInput = worldInput;
		xzInput.y = 0.0f;
		if (xzInput.sqrMagnitude < 0.1f*0.1f)
		{
			// If XZ input is zero, damp the player's speed
			Vector3 vel = body.GetPointVelocity( transform.position );
			vel.y = 0.0f;
			vel.Normalize();
			force -= vel * playerDamping;
		}
		
		body.AddForce( force );
	}
	
	void ProcessChaseCam()
	{
		Vector3 vecToPlayer = transform.position - chaseCamPos;
		Vector3 vecToPlayerXZ = vecToPlayer;
		vecToPlayerXZ.y = 0.0f;
		
		float yOffset = -vecToPlayer.y;
		float dist = vecToPlayer.magnitude;
		vecToPlayer /= dist;
		
		chaseCamVel = vecToPlayer * (dist-chaseCamTgtDistance);
		chaseCamVel.y += chaseCamTgtHeight - yOffset;
		
		float xzDist = vecToPlayerXZ.magnitude;
		if (xzDist < 1.0f)
		{
			vecToPlayerXZ /= xzDist;
			chaseCamVel += vecToPlayerXZ * -(1.0f - xzDist);
		}
		
		// TODO: Seeking is juddery, figure out why
		chaseCamPos += chaseCamVel * Time.deltaTime * chaseCamSeekSpeed;
		
		
		// Snap main cam to chase cam pos
		Camera.main.transform.position = chaseCamPos;
		Camera.main.transform.LookAt( transform );
	}
	
	// Update is called once per frame
	void Update ()
	{
		ProcessChaseCam();
		HandleInput();
		ApplyInput();
	}
}
