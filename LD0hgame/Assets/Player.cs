using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	private Vector3 velocity;
	private Transform glow;
	public GameObject death;
	public float dampingStrength;
	public float maxSpeed;
	public float pillPowerTime;
	private float score;
	private float respawnDelay;
	
	// Use this for initialization
	void Start () {
		respawnDelay = 0.0f;
		score = 0.0f;
		velocity = Vector3.zero;
		glow = transform.FindChild( "Glow" );
	}
	
	
	
	// Update is called once per frame
	void Update ()
	{
		// Get the input vector from kayboard or analog stick
		var directionVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);		

		if (respawnDelay > 0.0f)
		{
			respawnDelay -= Time.deltaTime;
			if (respawnDelay <= 0.0f)
				Application.LoadLevel ("Zone1");
			
			directionVector = Vector3.zero;
		}
		else
		{
			score += Time.deltaTime;
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
		
		if (respawnDelay <= 0.0f)
		{
			if (pillPowerTime > 0.0f)
			{
				pillPowerTime -= Time.deltaTime;
				if (pillPowerTime > 1.5f)
					glow.renderer.enabled = true;
				else
					glow.renderer.enabled = !glow.renderer.enabled;
			}
			else
			{
				glow.renderer.enabled = false;
			}
		}
	}
	
	void OnGUI()
	{
		GUI.Label( new Rect(10, 10, 100, 40), "Time: " + score );
	}
	
	void Pill()
	{
		pillPowerTime = 7.0f;
	}
	
	void Hit()
	{
		if (pillPowerTime <= 0.0f && respawnDelay <= 0.0f)
		{
			renderer.enabled = false;
			transform.FindChild("Trail").renderer.enabled = false;
			Instantiate( death, transform.position, Quaternion.identity );
			respawnDelay = 3.0f;
		}
	}
}
