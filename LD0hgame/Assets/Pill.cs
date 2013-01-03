using UnityEngine;
using System.Collections;

public class Pill : MonoBehaviour {
	
	private GameObject player;
	
	// Use this for initialization
	void Start () {
	
		player = GameObject.Find("Player");
		
	}
	
	// Update is called once per frame
	void Update ()
	{	
		if (renderer.enabled)
		{
			var directionVector = player.transform.position - transform.position;
			
			if (directionVector.sqrMagnitude < 1.1f * 1.1f)
			{
				renderer.enabled = false;
				player.GetComponent("Player").SendMessage("Pill");
			}
		}
	}
}
