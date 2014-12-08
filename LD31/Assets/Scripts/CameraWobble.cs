using UnityEngine;
using System.Collections;

public class CameraWobble : MonoBehaviour
{
	public float wobbleSpeed = 0.2f;
	public float wobbleSize = 0.4f;
	
	private Vector3 basePos;
	private Vector3 xDir;
	private Vector3 upDir;
	private float time;

	void Start ()
	{
		basePos = transform.position;
		xDir = transform.right;
		upDir = transform.up;
		time = 0.0f;
		transform.LookAt( Vector3.zero );
	}

	void Update ()
	{
		time += Time.deltaTime;
		
		// create wobble with no obvious repeat
		float xofs = Mathf.Sin ( time * Mathf.PI * wobbleSpeed) * Mathf.Sin ( time * Mathf.PI * wobbleSpeed * 0.35f);
		float yofs = Mathf.Sin ( time * Mathf.PI * wobbleSpeed * 0.3f ) + Mathf.Sin ( time * Mathf.PI * wobbleSpeed * 0.27f);
		
		transform.position = basePos + xDir * xofs * wobbleSize + upDir * yofs * wobbleSize;
		transform.LookAt( Vector3.zero );
	}
}