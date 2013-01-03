using UnityEngine;
using System.Collections;

public class ProxTrigger : MonoBehaviour
{	
	public float proximity = 3.0f;
	public string message;
	public float duration = 3.0f;
	public GameObject nextTrigger;
	
	private GameObject world;
	private GameGUI gui;
	private Transform player;
	
	// Use this for initialization
	void Start ()
	{
		player = GameObject.Find("Player").transform;
		world = GameObject.Find("World");
		gui = world.GetComponent<GameGUI>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 dist = player.position - transform.position;
		if (dist.sqrMagnitude < proximity*proximity)
		{
			gui.Print( message, duration );
			gameObject.active = false;
			if (nextTrigger)
				nextTrigger.active = true;
		}
	}
}
