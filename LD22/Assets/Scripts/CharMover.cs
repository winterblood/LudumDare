using UnityEngine;
using System.Collections;

public class CharMover : MonoBehaviour
{

	enum ANIMSTATE
	{
		IDLE,
		WALK,
		RECOIL,
		JUMP_LAUNCH,
		JUMP_IDLE,
		JUMP_LAND
	};
	
	public float				inputScale			= 1.0f;		// 1
	public float				inputScaleJumping	= 1.0f;		// 1
	public float 				inputDamping		= 0.1f;		// 0.1
	public float 				frictionGround		= 0.5f;		// 0.5
	public float 				frictionAir			= 0.25f;	// 0.25
	public float				velocityJump		= 0.6f;		// 0.6
	public float				velocityMax			= 0.25f;	// 0.25
	public float				turnSpeed			= 720.0f;	// 720.0f
	
	private Vector3				velocity;
	private Vector3				acceleration;
	private Vector3				surface_normal;
	private GameObject			figure;
	private CharacterController controller;
	private CollisionFlags 		collisionFlags;
	private bool				onGround;
	private bool				triggerInteraction;
	private float				yrot;
	private float				targetyrot;
	private Animation			anim;
	private float				hurtTimer;
	private float				suppressInputTimer;
	private ANIMSTATE			animState;

	
	// Use this for initialization
	void Start ()
	{
		animState = ANIMSTATE.IDLE;
		acceleration = Vector3.zero;
		velocity = Vector3.zero;
		collisionFlags = CollisionFlags.None;
		controller = gameObject.GetComponent<CharacterController>();
		Transform charModel = transform.Find( "Robin" );
		if (charModel)
		{
			Debug.Log ("Found robin");
			figure = charModel.gameObject;
		}
		anim = figure.animation;
		Debug.Log ("Found anim: " + anim.ToString());
		onGround = false;
		triggerInteraction = false;
		yrot = 180.0f;
		targetyrot = 180.0f;
		hurtTimer = 0.0f;
		suppressInputTimer = 0.0f;
	}
	
	public void SetHurt( Transform trans)
	{
		Vector3 recoil_dir = transform.position - trans.position;
		recoil_dir.z = 0.0f;
		recoil_dir.Normalize();
		recoil_dir *= velocityJump * 0.5f;
		velocity = recoil_dir;
		hurtTimer = 0.5f;
		suppressInputTimer = 0.2f;
	}
	
	public bool IsHurt()
	{
		return (hurtTimer>0.0f);
	}
	
	public float GetTargetYRot()
	{
		return targetyrot;
	}
	
	public bool IsInteracting()
	{
		return triggerInteraction;
	}
	
	// Update is called once per frame
	void Update ()
	{
		float dt = Time.deltaTime;
		float lastx = transform.position.x;
		triggerInteraction = false;
		
		Vector3 vinput = Vector3.zero;
		if (suppressInputTimer<=0.0f)
		{
			vinput.x = Input.GetAxisRaw("Horizontal");
			vinput.y = Input.GetAxisRaw("Vertical");
			if (vinput.x>0)
				targetyrot = 91.0f;
			else if (vinput.x<0)
				targetyrot = 269.0f;
		}
		else
			suppressInputTimer -= dt;
		
		if (onGround)
			acceleration = vinput * inputScale;
		else if (!controller.isGrounded)		// From previous frame
			acceleration = vinput * inputScaleJumping;
		else
			acceleration = Vector3.zero;
		
		if (onGround)
		{
			velocity.y = 0.0f;
			if (vinput.y > 0.1f)
			{
				//Debug.Log( "JUMP at " + velocityJump + " in dt=" + dt);
				velocity.y = velocityJump;
				onGround = false;
				animState = ANIMSTATE.JUMP_LAUNCH;
			}
			else if (vinput.y < -0.1f)
			{
				triggerInteraction = true;
			}
		}
		
		// Damp horizontal acceleration
		if (acceleration.x > inputDamping*dt)
		{
			acceleration.x -= inputDamping*dt;
		}
		else if (acceleration.x < -inputDamping*dt)
		{
			acceleration.x += inputDamping*dt;
		}
		else
			acceleration.x = 0.0f;		
		acceleration.y = -0.98f;
		acceleration.z = 0.0f;
		
		velocity += acceleration * dt;
		if (velocity.x > 0.0f)
		{
			if (onGround)
				velocity.x -= frictionGround*dt;
			else
				velocity.x -= frictionAir*dt;
			
			if (velocity.x < 0.0f)
				velocity.x = 0.0f;
			else if (velocity.x > velocityMax)
				velocity.x = velocityMax;
		}
		else if (velocity.x < 0.0f)
		{
			if (onGround)
				velocity.x += frictionGround*dt;
			else
				velocity.x += frictionAir*dt;
			
			if (velocity.x > 0.0f)
				velocity.x = 0.0f;
			else if (velocity.x < -velocityMax)
				velocity.x = -velocityMax;
		}
		else
			velocity.x = 0.0f;
		
		//
		// Move and collide
		//
		onGround = false;
		collisionFlags = controller.Move( velocity );
		if (controller.isGrounded)
		{
			if (surface_normal.y > 0.7f)
				onGround = true;
			else if (suppressInputTimer < 0.1f)
				suppressInputTimer = 0.1f;
		}
		if ((collisionFlags & CollisionFlags.CollidedBelow) > 0)
		{
			//velocity.y *= -0.5f;
			//acceleration.y = 0.0f;
		}
		if ((collisionFlags & CollisionFlags.CollidedAbove) > 0)
		{
			//velocity.y *= -0.1f;
			//acceleration.y = 0.0f;
		}
		if ((collisionFlags & CollisionFlags.CollidedSides) > 0)
		{
			velocity.x *= -0.8f;
			acceleration.x = 0.0f;
		}
		
		
		//
		// Update facing
		//
		/*
		if (transform.position.x > lastx)
			targetyrot = 90.0f;
		else if (transform.position.x < lastx)
			targetyrot = 270.0f;
		*/
		yrot = Mathf.MoveTowardsAngle( yrot, targetyrot, turnSpeed * dt );

		Vector3 euler = Vector3.zero;
		euler.y = yrot;
		transform.localEulerAngles = euler;
		
		//
		// Update anim
		//
		/*
		if (hurtTimer>0.0f)
		{
			anim.CrossFade("plr_recoil", 0.1f);
			hurtTimer -= dt;
		}
		else
		*/
		if (!onGround)
		{
			if (animState==ANIMSTATE.JUMP_LAUNCH)
			{
				anim.CrossFade("Jump", 0.1f);
				animState = ANIMSTATE.JUMP_IDLE;
			}
		}
		else if (Mathf.Abs(velocity.x) > 0.02f)
		{
			anim.CrossFade("Walk");
		}
        else
		{
           	anim.CrossFade("Idle");
		}
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		float dt = Time.deltaTime;
        Vector3 normal = hit.normal;
		surface_normal = normal;
		normal.y = 0.0f;
		normal.z = 0.0f;
		
		// Push away from surface
		velocity += normal * dt;
    }
}