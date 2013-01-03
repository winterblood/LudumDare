using UnityEngine;
using System.Collections;

public class LightJitter : MonoBehaviour
{
	private Vector3 centrePos;
	private Vector3 rotation;
	public Vector3 rotSpeed;
	public float jitterRadius;
	public float jitterMultiplier;
	private GameObject playerObj;
	private float lightRange = 7.0f;
	private GameGUI gui;
	
	void Start ()
	{
		centrePos = transform.position;
		playerObj = GameObject.Find("First Person Controller");
		
		GameObject guiobj = GameObject.Find( "HUD" );
		if (guiobj)
		{
			gui = guiobj.GetComponent<GameGUI>();
		}
	}
	
	void Update ()
	{
		rotation.x += rotSpeed.x * Time.deltaTime * jitterMultiplier; if (rotation.x > 360.0f) rotation.x -= 360.0f;
		rotation.y += rotSpeed.y * Time.deltaTime * jitterMultiplier; if (rotation.y > 360.0f) rotation.y -= 360.0f;
		rotation.z += rotSpeed.z * Time.deltaTime * jitterMultiplier; if (rotation.z > 360.0f) rotation.z -= 360.0f;
		
		Vector3 offset = Vector3.zero;
		Vector3 radians = Vector3.zero;
		radians.x = rotation.x * 2.0f * Mathf.PI / 360.0f;
		radians.y = rotation.y * 2.0f * Mathf.PI / 360.0f;
		radians.z = rotation.z * 2.0f * Mathf.PI / 360.0f;
		offset.x = Mathf.Sin(radians.x) * jitterRadius;
		offset.y = Mathf.Sin(radians.y) * jitterRadius;
		offset.z = Mathf.Sin(radians.z) * jitterRadius;

		transform.position = centrePos + offset;
		
		IlluminatePlayer();
	}
	
	void IlluminatePlayer()
	{
		Vector3 dir = playerObj.transform.position - centrePos;
		float distance = dir.magnitude;
		if ( distance < lightRange )
		{
			dir /= distance;
			
			// Within range to be lit
			RaycastHit hitinfo;
			if ( Physics.Raycast( centrePos, dir, out hitinfo, distance))
			{
				return;		// something blocked line of sight
			}
			
			float lightPercent = 1.0f - distance / lightRange;
			gui.AddLight( lightPercent );
		}
	}
}
