using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
	
	// Input
	public float inputForce;
	public float jumpForce;
	private Vector3 worldInput;
	private Vector3 lookInput;
	private bool inputJump;
	
	// Movement
	public float playerDamping;
	public bool playerDampingContinuous;
	public float playerJumpMinTime;
	public float playerJumpMaxTime;
	public float playerTopSpeed;
	public float playerGravity = -10.0f;
	private Vector3 worldVelocity;
	private Vector3 lookVelocity;
	private float jumpTimer;
	private float timeSinceOnGround;
	
	// Chase cam
	public float chaseCamTgtDistance;
	public float chaseCamTgtHeight;
	public float chaseCamSeekSpeed;
	public float chaseCamYawSpeed;
	public float chaseCamPitchSpeed;
	public float chaseCamLookSeek;
	public float chaseCamGroundAvoid = 1.0f;
	public bool emergencyCameraPos = false;
	public bool emergencyCameraLook = false;
	public bool enableGroundUnembed = true;
	public bool enableManualLook = true;
	private Vector3 chaseCamPos;
	private Vector3 chaseCamVel;
	private Vector3 chaseCamLook;
	private float currentCamTerrainY;

	// Interaction with world
	private Landscape landscape;
	private float currentTerrainY;
	
	// Use this for initialization
	void Start ()
	{
		worldInput = Vector3.zero;
		inputJump = false;
		jumpTimer = 0.0f;
		timeSinceOnGround = 0.0f;
		chaseCamPos = transform.position + Vector3.forward * chaseCamTgtDistance;
		chaseCamPos.y += chaseCamTgtHeight;
		chaseCamVel = Vector3.zero;
		lookVelocity = Vector3.zero;
		worldVelocity = Vector3.zero;
		chaseCamLook = Vector3.zero;
		
		
		GameObject landscapeObj = GameObject.Find( "Landscape" );
		landscape = landscapeObj.GetComponent<Landscape>();
	}
	
	bool IsOnGround()
	{
		if (jumpTimer > 0.0f)
			return false;
		return (timeSinceOnGround == 0.0f);
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
		
		//
		// JUMP/FLY
		//
		inputJump = Input.GetButton("Jump");
		if (inputJump && IsOnGround())
		{
			//Debug.Log("LAUNCH");
			jumpTimer = playerJumpMinTime;		// Force minimum float time when launching from ground
		}

		if ((inputJump || jumpTimer > 0.0f) && timeSinceOnGround<playerJumpMaxTime)
		{
			//Debug.Log("Time since on ground = " + timeSinceOnGround);
			worldInput.y = 1.0f;
			if (jumpTimer > 0.0f)
				jumpTimer -= Time.deltaTime;
		}
		
		//
		// LOOK AROUND
		//
		lookInput.x = -Input.GetAxis("LookH");
		lookInput.y = -Input.GetAxis("LookV");
		lookInput.z = 0.0f;
		//Debug.DrawRay(transform.position, lookInput * 10, Color.cyan);
	}
	
	void ApplyInput()
	{	
		float xzScale = 1.0f;	
		//Rigidbody body = gameObject.GetComponent<Rigidbody>();
		//Vector3 xzVel = body.GetPointVelocity( transform.position );
		Vector3 xzVel = worldVelocity;
		xzVel.y = 0.0f;	
		if (xzVel.sqrMagnitude > playerTopSpeed*playerTopSpeed)
			xzScale = 0.0f;

		Vector3 force = worldInput;
		force.x *= inputForce * xzScale;
		force.z *= inputForce * xzScale;
		force.y *= jumpForce;
		
		Vector3 xzInput = worldInput;
		xzInput.y = 0.0f;
		if (xzInput.sqrMagnitude < 0.1f*0.1f || playerDampingContinuous)	// If XZ input is zero, damp the player's speed
		{			
			if (xzVel.sqrMagnitude > 1.0f)
				xzVel.Normalize();
				
			force -= xzVel * playerDamping;
		}
		
		Vector3 newpos = transform.position;
		float playerHeightOffGround = newpos.y - currentTerrainY;
		if (playerHeightOffGround < 0.5f)
		{
			timeSinceOnGround = 0.0f;
			newpos.y = currentTerrainY+0.5f;
			if (worldVelocity.y < 0.0f)
				worldVelocity.y = 0.0f;
		}	
		
		force.y += playerGravity;
		
		worldVelocity += force * Time.deltaTime;
		
		newpos += worldVelocity * Time.deltaTime;
		Vector3 facing = worldInput;
		facing.y = 0.0f;
		facing += newpos;
		transform.position = newpos;
		transform.LookAt( facing );
		//body.AddForce( force );
	}
	
	void OnCollisionStay( Collision collisionInfo )
	{
		timeSinceOnGround = 0.0f;
		/*
        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal * 10, Color.red);
        }
        */
    }
	
	void ProcessChaseCam()
	{
		float playerHeightOffGround = transform.position.y - currentTerrainY;
	
		chaseCamLook = Vector3.MoveTowards( chaseCamLook, transform.position + transform.forward, 20.0f * Time.deltaTime );
		//chaseCamLook = transform.position + transform.forward;
		Debug.DrawLine( chaseCamLook, Camera.main.transform.position + Camera.main.transform.right * 0.1f, Color.red );
	
		Vector3 vecToPlayer = chaseCamLook - chaseCamPos;
		Vector3 vecToPlayerXZ = vecToPlayer;
		vecToPlayerXZ.y = 0.0f;
		
		float yOffset = -vecToPlayer.y;
		float dist = vecToPlayer.magnitude;
		vecToPlayer /= dist;
		
		float currentTargetHeight = chaseCamTgtHeight;
		if (worldInput.y > 0.0f || playerHeightOffGround > 2.0f)
			currentTargetHeight *= 3.0f;
		
		chaseCamVel = vecToPlayer * (dist-chaseCamTgtDistance);
		chaseCamVel.y += currentTargetHeight - yOffset;
		
		float xzDist = vecToPlayerXZ.magnitude;
		if (xzDist < 1.0f)
		{
			vecToPlayerXZ /= xzDist;
			chaseCamVel += vecToPlayerXZ * -(1.0f - xzDist);
		}
		
		//
		// Manual look around
		//
		if (enableManualLook)
		{
			lookVelocity = Vector3.MoveTowards( lookVelocity, lookInput, Time.deltaTime * chaseCamLookSeek );
			chaseCamVel += Camera.main.transform.right * lookVelocity.x * -chaseCamYawSpeed;
			if (xzDist > 1.0f)
				chaseCamVel += Camera.main.transform.up * lookVelocity.y * chaseCamPitchSpeed;
		}
		
		//
		// Keep camera above terrain
		//
		if (enableGroundUnembed)
		{
			float distUnderSafeY = (currentCamTerrainY + chaseCamGroundAvoid) - chaseCamPos.y;
			if (distUnderSafeY > 0.0f)
			{
				//Vector3 v = Vector3.up * distUnderSafeY;
				//Debug.DrawRay( transform.position + Vector3.up, v, Color.yellow ); 
				chaseCamVel += Camera.main.transform.up * distUnderSafeY;
			}
		}
		
		// TODO: Seeking is juddery, figure out why
		chaseCamPos += chaseCamVel * Time.deltaTime * chaseCamSeekSpeed;
		
		if (chaseCamPos.y < currentCamTerrainY + 0.5f)
			chaseCamPos.y = currentCamTerrainY + 0.5f;
		
		// Snap main cam to chase cam pos
		if (emergencyCameraPos)
		{
			chaseCamPos = transform.position + transform.forward * -10.0f;
			chaseCamPos.y = currentTerrainY + 4.0f;
		}
		if (emergencyCameraLook)
		{	
			chaseCamLook.y = currentTerrainY;
		}
		Camera.main.transform.position = chaseCamPos;
		Camera.main.transform.LookAt( chaseCamLook );
	}
	
	void QuickenWorld()
	{
		if (transform.position.y - currentTerrainY < 0.9f)
		{
			landscape.ColourTexture( transform.position, 0.0f, Color.green );
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		currentTerrainY = landscape.GetTerrainHeight( transform.position.x, transform.position.z );		// player
		currentCamTerrainY = landscape.GetTerrainHeight( chaseCamPos.x, chaseCamPos.z );				// camera
		
		HandleInput();
		ApplyInput();
		ProcessChaseCam();		
		QuickenWorld();
		
		timeSinceOnGround += Time.deltaTime;
	}
}
