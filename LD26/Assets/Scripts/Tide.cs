using UnityEngine;
using System.Collections;

public class Tide : MonoBehaviour
{
	public float risingTideSpeed = 0.1f;
	
	private float startY;
	private Landscape landscape;

	// Use this for initialization
	void Start ()
	{
		startY = transform.position.y;
		
		GameObject landscapeObj = GameObject.Find( "Landscape" );
		landscape = landscapeObj.GetComponent<Landscape>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 pos = transform.position;
	
		if (landscape.IsCompleted())
		{
			// Tide recedes
			if (pos.y > startY)
				pos.y -= risingTideSpeed * Time.deltaTime * 3.0f;
		}
		else
		{
			// Tide rises steadily
			pos.y += risingTideSpeed * Time.deltaTime;
		}
		
		transform.position = pos;
	}
}
