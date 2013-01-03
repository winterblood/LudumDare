using UnityEngine;
using System.Collections;

public class Wibble : MonoBehaviour
{
	private Vector3		basePos;
	private float		sineAngle;
	private float		sineAngle2;
	private float		degToRadians;
	private float		sineSpeed;
	private float		sineSpeed2;
	private Vector3		maxRotateAngle;
	private Vector3		maxWobble;
	
	public float		scale;	// 0 leaves defaults
	
	// Use this for initialization
	void Start ()
	{
		sineSpeed = Random.Range( 100.0f, 150.0f );
		sineSpeed2 = Random.Range( 90.0f, 180.0f );
		sineAngle = Random.Range( 0.0f, 360.0f );
		sineAngle2 = Random.Range( 0.0f, 360.0f );
		maxRotateAngle.z = Random.Range( 1.0f, 5.0f );
		maxWobble.x = Random.Range( 0.1f, 0.6f );
		maxWobble.y = Random.Range( 0.1f, 0.6f );
		
		if (scale > 0.0f)
		{
			maxWobble *= scale;
			maxRotateAngle *= scale;
			sineSpeed *= scale;
			sineSpeed2 *= scale;
		}
		basePos = transform.position;
		degToRadians = 2.0f * 3.1415927f / 360.0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		float dt = Time.deltaTime;
		
		sineAngle += sineSpeed * dt;
		sineAngle2 += sineSpeed2 * dt;
		if (sineAngle > 360.0f)
			sineAngle -= 360.0f;
		if (sineAngle2 > 360.0f)
			sineAngle2 -= 360.0f;
		
		Vector3 euler = Vector3.zero;
		euler.z = maxRotateAngle.z * Mathf.Sin( sineAngle * degToRadians );
		Vector3 wibble = Vector3.zero;
		wibble.x = maxWobble.x * Mathf.Sin( (sineAngle + 50.0f) * degToRadians );
		wibble.y = maxWobble.y * Mathf.Sin( sineAngle2 * degToRadians );
		//wibble = Vector3.MoveTowards( wibble, targetWibble, 0.1f );
		transform.position = basePos + wibble;
		transform.localEulerAngles = euler;
	}
}
