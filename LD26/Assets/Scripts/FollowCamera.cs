using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{

	public Vector3 offset;
	public Transform followTarget;
	public bool snapToGround = false;
	
	private Landscape landscape;

	// Use this for initialization
	void Start ()
	{
		GameObject landscapeObj = GameObject.Find( "Landscape" );
		landscape = landscapeObj.GetComponent<Landscape>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Reposition this object at an offset from the camera position
		Vector3 pos = followTarget.position;
		pos += followTarget.forward * offset.z;
		pos += followTarget.right * offset.x;
		
		if (snapToGround)
			pos.y = landscape.GetTerrainHeight( pos.x, pos.z );
		
		pos.y += offset.y;
		transform.position = pos;
	}
}
