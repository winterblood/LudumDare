using UnityEngine;
using System.Collections;

public class CameraTracker : MonoBehaviour
{

	public Transform 	cameraTarget;
	public float		cameraSpeed;
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 target = cameraTarget.position;
		Vector3 newpos = transform.position;
		newpos.x = Mathf.MoveTowards( transform.position.x, target.x, cameraSpeed );
		newpos.y = Mathf.MoveTowards( transform.position.y, target.y, cameraSpeed );
		transform.position = newpos;
	}
}
