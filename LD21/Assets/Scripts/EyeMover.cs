using UnityEngine;
using System.Collections;

public class EyeMover : MonoBehaviour
{	
	public float		eyeAngleAdjust;
	
	private float 		blinkTimer;
	private GameObject 	player;
	private Transform	eyeBone;
	private CharMover 	charscript;
	private float		yrot;
	private float		targetYRot;
	private Vector3		baseEuler;

	// Use this for initialization
	void Start ()
	{
		blinkTimer 	= 0.0f;
		yrot 		= 0.0f;
		targetYRot 	= 0.0f;
		player 		= GameObject.Find( "Player" );
		charscript 	= player.GetComponent<CharMover>();
		eyeBone 	= player.transform.Find( "figure/Armature/root/lower_spine/upper_spine/neck/head/eyes" );
		baseEuler 	= transform.localEulerAngles;
	}
	
	// Update is called once per frame
	void Update ()
	{
		float dt = Time.deltaTime;
		blinkTimer -= Time.deltaTime;
		if (blinkTimer<0.0f)
		{
			if (renderer.enabled)
			{
				renderer.enabled = false;
				blinkTimer = 0.1f;
			}
			else
			{
				renderer.enabled = true;
				blinkTimer = Random.Range( 1.0f, 3.0f );
			}
		}
		
		if (charscript)
		{
			if (charscript.GetTargetYRot() > 180.0f)
			{
				targetYRot = -eyeAngleAdjust;
			}
			else
			{
				targetYRot = eyeAngleAdjust;
			}
			yrot = Mathf.MoveTowardsAngle( yrot, targetYRot, charscript.turnSpeed * dt );
		}
		
		Vector3 euler = baseEuler;
		euler.z += yrot;
		Vector3 pos = eyeBone.position;
		pos.z -= 0.1f;	// Prevent Z-fighting
		transform.position = pos;
		transform.localEulerAngles = euler;
	}
}
