using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour {

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;
	
	private float invertY = 1.0f;

	private Vector3 targetEulerAngles;

	void Update ()
	{
		if (!Screen.lockCursor)
			return;
		
		float dx = Input.GetAxis("Mouse X");
		float dy = Input.GetAxis("Mouse Y");
		if (axes == RotationAxes.MouseXAndY)
		{
			targetEulerAngles.y += dx * sensitivityX;
			if (targetEulerAngles.y>360.0f)
				targetEulerAngles.y-=360.0f;
			if (targetEulerAngles.y<-0.0f)
				targetEulerAngles.y+=360.0f;

			targetEulerAngles.x += dy * sensitivityY * invertY;
			targetEulerAngles.x = Mathf.Clamp (targetEulerAngles.x, minimumY, maximumY);

		}
		else if (axes == RotationAxes.MouseX)
		{
			targetEulerAngles.y += dx * sensitivityX;
			if (targetEulerAngles.y>360.0f)
				targetEulerAngles.y-=360.0f;
			if (targetEulerAngles.y<-0.0f)
				targetEulerAngles.y+=360.0f;

			targetEulerAngles.x = transform.localEulerAngles.x;	// Preserve existing Y look
		}
		else
		{
			targetEulerAngles.y = transform.localEulerAngles.y;	// Preserve existing X look
			targetEulerAngles.x += Input.GetAxis("Mouse Y") * sensitivityY * invertY;
			targetEulerAngles.x = Mathf.Clamp (targetEulerAngles.x, minimumY, maximumY);

		}

		//transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, targetEulerAngles, seekRate * Time.time);
		transform.localEulerAngles = targetEulerAngles;
		
		if (Input.GetKeyDown (KeyCode.Y))
		{
			if (invertY<0.0f)
				invertY = 1.0f;
			else
				invertY = -1.0f;
		}
	}

	void Start ()
	{
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;

		targetEulerAngles = transform.localEulerAngles;
	}
}