using UnityEngine;
using System.Collections;


public class CameraBlend
{
	public Vector3 pos;
	public Vector3 look;
	public float fov;
	public int priority;
	public int guid;
	
	static public void Blend( CameraBlend dest, CameraBlend camFrom, CameraBlend camTo, float t )
	{
		dest.pos 	= Vector3.Lerp( camFrom.pos, camTo.pos, t );
		dest.look 	= Vector3.Lerp( camFrom.look, camTo.look, t );
		dest.fov	= camFrom.fov * (1.0f-t) + camTo.fov * t;
		dest.priority 	= camTo.priority;
		dest.guid 		= camTo.guid;
	}
};

public class CameraBlender : MonoBehaviour
{

	private CameraBlend currentCam;
	private CameraBlend targetCam;
	private CameraBlend oldCam;
	private float blendInWeight = -1.0f;

	// Use this for initialization
	void Start ()
	{
		currentCam = new CameraBlend();
		targetCam = new CameraBlend();
		oldCam = new CameraBlend();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (blendInWeight >= 0.0f)
		{
			CameraBlend.Blend( currentCam, oldCam, targetCam, blendInWeight );
			blendInWeight += Time.deltaTime;
			if (blendInWeight >= 1.0f)
				blendInWeight = -1.0f;
		}
	
	/*
		Camera.main.transform.position = currentCam.pos;
		Camera.main.transform.LookAt( currentCam.look );
		Camera.main.fov = currentCam.fov;
	*/
	}
	
	bool RequestCamera( CameraBlend cam )
	{
		if (cam.priority < currentCam.priority)
		{
			return false;
		}
		
		if (cam.guid != targetCam.guid)		// Change of camera!
		{
			targetCam = cam;
			oldCam = currentCam;
			blendInWeight = 0.0f;
			
		}
		return true;
	}
	
	int GetActivePriority()
	{
		return currentCam.priority;
	}
}