using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{

	public Transform player;
	public Vector3 cameraOffset;
	public float lookAhead = 0.0f;
	public float cameraSpeed = 1.0f;
	
	private Vector3 targetPos;
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		targetPos = player.transform.position;
		targetPos += player.transform.forward * lookAhead;
		targetPos += cameraOffset;
		transform.position = Vector3.MoveTowards( transform.position, targetPos, Time.deltaTime*cameraSpeed );
	}
}
