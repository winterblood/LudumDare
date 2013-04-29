using UnityEngine;
using System.Collections;

public class JoustAI : MonoBehaviour
{
	enum eJoustState
	{
		AIM_FOR_UPHILL,
		AIM_FOR_PLAYER,
		AIM_FOR_MEGATREE
	};

	// Input
	public float inputForce = 20.0f;
	public float jumpForce = 20.0f;
	private Vector3 worldInput;
	private Vector3 lookInput;
	private bool inputJump;
	
	// Movement
	public float playerDamping = 3.0f;
	public bool playerDampingContinuous = true;
	public float playerJumpMinTime = 0.5f;
	public float playerJumpMaxTime = 0.9f;
	public float playerTopSpeed = 8.0f;
	public float playerGravity = -9.81f;
	public float playerMaxRotSpeed = 5.0f;
	private Vector3 worldVelocity;
	private Vector3 lookVelocity;
	private float jumpTimer;
	private float timeSinceOnGround;
		
	// Animation
	private GameObject figure;
	
	// AI
	private eJoustState state = eJoustState.AIM_FOR_MEGATREE;
	private float inertiaCompensateWeight = 0.1f;
	private Vector3 currentAimPos = Vector3.zero;
	
	// Interaction with world
	private GameObject player;
	private GameObject megatree;
	private Landscape landscape;
	private float currentTerrainY;	
	
	// Use this for initialization
	void Start ()
	{
		worldInput = Vector3.zero;
		inputJump = false;
		jumpTimer = 0.0f;
		timeSinceOnGround = 0.0f;
		worldVelocity = Vector3.zero;
		
		GameObject landscapeObj = GameObject.Find( "Landscape" );
		landscape = landscapeObj.GetComponent<Landscape>();
		
		megatree = landscape.GetMegaTree();
		
		figure = transform.FindChild("figure").gameObject;
		
		player = GameObject.Find( "Player" );
	}
	
	bool IsOnGround()
	{
		if (jumpTimer > 0.0f)
			return false;
		return (timeSinceOnGround == 0.0f);
	}	
	
	void UpdateState()
	{
		switch( state )
		{
		case eJoustState.AIM_FOR_UPHILL:
		
			break;
			
		case eJoustState.AIM_FOR_PLAYER:
		
			currentAimPos = player.transform.position;
			Vector3 toPlayer = currentAimPos - transform.position;
			
			//
			// JOUST COLLISION
			//
			Vector3 xzToPlayer = toPlayer;
			xzToPlayer.y = 0.0f;
			if (xzToPlayer.sqrMagnitude < 2.0f*2.0f)
			{
				xzToPlayer.Normalize();
				xzToPlayer *= 20.0f;
				if (toPlayer.y < 0.0f)
				{
					player.GetComponent<PlayerMove>().Impact( xzToPlayer );
				}
				else
				{
					worldVelocity += -xzToPlayer;
					state = eJoustState.AIM_FOR_MEGATREE;
					break;
				}
			}
			
			//
			// Launch at player under the right conditions
			//
			if (IsOnGround() && toPlayer.y > 0.0f)
			{
				Debug.Log("AIM_FOR_PLAYER -> AIM_FOR_UPHILL");
				//state = eJoustState.AIM_FOR_UPHILL;
				//break;
			}
			toPlayer.y = 0.0f;
			toPlayer.Normalize();
			Vector3 moving = worldVelocity;
			moving.y = 0.0f;
			moving.Normalize();
			
			if (IsOnGround() && Vector3.Dot( moving, toPlayer )>0.9f)
			{
				jumpTimer = playerJumpMinTime;		// Force minimum float time when launching from ground
			}
			
			currentAimPos -= moving * inertiaCompensateWeight;
			
			break;
			
		case eJoustState.AIM_FOR_MEGATREE:
		
			if (!megatree)
				megatree = landscape.GetMegaTree();
			currentAimPos = megatree.transform.position;
			Vector3 toTree = currentAimPos - transform.position;
			if (toTree.sqrMagnitude < 4.0f)
			{
				Debug.Log("AIM_FOR_MEGATREE -> AIM_FOR_PLAYER");
				state = eJoustState.AIM_FOR_PLAYER;
			}
			break;
		}
	}
	
	void HandleInput()
	{
		Vector3 aimDirection = currentAimPos - transform.position;
		Debug.DrawRay( transform.position + Vector3.up, aimDirection, Color.red );
		
		worldInput = aimDirection;
		worldInput.y = 0.0f;
		worldInput.Normalize();
		
		/*
		// Jump when close to player and moving towards them
		bool inputJump = Random.value < 0.01f;
		if (inputJump && IsOnGround())
		{
			//Debug.Log("LAUNCH");
			jumpTimer = playerJumpMinTime;		// Force minimum float time when launching from ground
		}
		*/
		if ((jumpTimer > 0.0f) && timeSinceOnGround<playerJumpMaxTime)
		{
			//Debug.Log("Time since on ground = " + timeSinceOnGround);
			worldInput.y = 1.0f;
			if (jumpTimer > 0.0f)
				jumpTimer -= Time.deltaTime;
		}
		
		Debug.DrawRay( transform.position + Vector3.up, worldInput, Color.yellow );
	}
	
	void ApplyInput()
	{	
		float xzScale = 1.0f;	
		Vector3 xzVel = worldVelocity;
		xzVel.y = 0.0f;	
		if (xzVel.sqrMagnitude > playerTopSpeed*playerTopSpeed)
			xzScale = 0.0f;

		Vector3 force = worldInput;
		force.x *= inputForce * xzScale;
		force.z *= inputForce * xzScale;
		force.y *= jumpForce;
		force.y += playerGravity;
		
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
		
		worldVelocity += force * Time.deltaTime;
		
		newpos += worldVelocity * Time.deltaTime;
		transform.position = newpos;
		if ( xzInput.sqrMagnitude > 0.2f*0.2f )
		{
			Vector3 facing = worldInput;
			facing.y = 0.0f;
			Quaternion faceQuat = Quaternion.LookRotation( facing );
			facing += newpos;
			transform.rotation = Quaternion.RotateTowards( transform.rotation, faceQuat, playerMaxRotSpeed );
		}
	}
	
	void UpdateAnims()
	{
		if (inputJump)
			figure.animation.CrossFade("JumpUp", 0.2f, PlayMode.StopAll);
		else if (worldInput.sqrMagnitude > 0.2f*0.2f)
			figure.animation.CrossFade ("Walk");
		else
			figure.animation.CrossFade ("Idle");
	}	
	
	// Update is called once per frame
	void Update ()
	{
		currentTerrainY = landscape.GetTerrainHeight( transform.position.x, transform.position.z );		// player
		
		UpdateState();
		HandleInput();
		ApplyInput();
		UpdateAnims();		
		//QuickenWorld();
		
		timeSinceOnGround += Time.deltaTime;
	}
}
