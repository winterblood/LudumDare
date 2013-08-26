using UnityEngine;
using System.Collections;


public class CameraBlend
{
	public Vector3 pos;
	public Vector3 look;
	public float fov = 60.0f;
	public int priority;
	public int guid;
	
	static public void Blend( ref CameraBlend dest, ref CameraBlend camFrom, ref CameraBlend camTo, float t )
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
		Debug.Log("Created CameraBlends");
		currentCam = new CameraBlend();
		targetCam = new CameraBlend();
		oldCam = new CameraBlend();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (currentCam.guid != targetCam.guid)		// Change of camera!
		{
			Debug.Log( "NEW CAMERA! guid = "+targetCam.guid );
			CameraBlend.Blend( ref oldCam, ref oldCam, ref currentCam, 1.0f );	// Set oldcam to currentcam
			blendInWeight = 0.0f;
		}
	
		if (blendInWeight >= 0.0f)
		{
			CameraBlend.Blend( ref currentCam, ref oldCam, ref targetCam, blendInWeight );
			blendInWeight += Time.deltaTime;
			if (blendInWeight >= 1.0f)
				blendInWeight = -1.0f;
		}
		else
		{
			CameraBlend.Blend( ref currentCam, ref currentCam, ref targetCam, 1.0f );	// Set currentcam to targetcam
		}
	
		Camera.main.transform.position = currentCam.pos;
		Camera.main.transform.LookAt( currentCam.look );
		Camera.main.fov = currentCam.fov;
		
		currentCam.priority = 0;	// Ready for next frame's priority mix
	}
	
	public bool RequestCamera( ref CameraBlend cam )
	{
		if (currentCam.priority > 20000)
			return false;
	
		if (cam.priority < currentCam.priority)
		{
			return false;
		}
		
		CameraBlend.Blend( ref targetCam, ref targetCam, ref cam, 1.0f );	// Set targetcam to cam
		
		return true;
	}
	
	int GetActivePriority()
	{
		return currentCam.priority;
	}
}
