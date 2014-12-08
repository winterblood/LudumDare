using UnityEngine;
using System.Collections;

public class BallControl : MonoBehaviour
{
	public float accelMultiplier = 2.0f;
	public Camera mainCam;
	
	private Vector3 spawnPos;

	void Start ()
	{
		spawnPos = transform.position;
	}

	public void Reset()
	{
		Immobilise();
		transform.position = spawnPos;
	}

	public void Immobilise()
	{
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;		
	}

	void Update ()
	{
		Vector3 force = Vector3.zero;
		force.x = Input.GetAxis( "Horizontal" );
		force.z = Input.GetAxis( "Vertical" );
		
		if (transform.position.y > 0.0f && transform.position.y < 0.3f)	// Cheap immobilisation unless on floor
		{
			Transform t = mainCam.gameObject.transform;
			Vector3 stored = t.position + t.forward;
			Vector3 ahead = t.position;
			ahead += mainCam.gameObject.transform.up;
			ahead.y = t.position.y;
			t.LookAt( ahead );
				
			Vector3 worldForce = t.TransformDirection( force );
			GetComponent<Rigidbody>().AddForce( worldForce * accelMultiplier);
			
			t.LookAt ( stored );
		}
	}
}