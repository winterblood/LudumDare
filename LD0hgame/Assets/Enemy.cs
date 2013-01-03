using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	
	private Vector3 velocity;
	private GameObject player;
	public GameObject death;
	private Player script;
	public float dampingStrength;
	public float maxSpeed;
	
	// Use this for initialization
	void Start () {
		velocity = Vector3.zero;
		
		player = GameObject.Find("Player");
		script = player.GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!renderer.enabled)
			return;
		
		// Get the input vector from kayboard or analog stick
		var directionVector = player.transform.position - transform.position;
		
		if (script.pillPowerTime > 0.0f && directionVector.sqrMagnitude < 8.0f*8.0f)
			directionVector = -directionVector;
		
		if (directionVector.sqrMagnitude < 0.95f * 0.95f)
		{
			renderer.enabled = false;
			transform.FindChild("Trail").renderer.enabled = false;
			player.GetComponent("Player").SendMessage("Hit");
			Instantiate( death, transform.position, Quaternion.identity );
		}
		
		transform.position += velocity * Time.deltaTime;
		
		velocity += directionVector;
		
		Vector3 damping = -velocity;
		damping.Normalize();
		
		velocity += damping * dampingStrength * Time.deltaTime;
		
		if (velocity.sqrMagnitude > maxSpeed * maxSpeed)
		{
			velocity.Normalize();
			velocity *= maxSpeed;
		}
	}
}
