using UnityEngine;
using System.Collections;

public class TimerTrigger : MonoBehaviour
{	
	public float delay = 3.0f;
	public string message;
	public float duration = 3.0f;
	public GameObject nextTrigger;
	
	private GameObject world;
	private GameGUI gui;
	private Transform player;
	private float timer = 0.0f;
	
	// Use this for initialization
	void Start ()
	{
		player = GameObject.Find("Player").transform;
		world = GameObject.Find("World");
		gui = world.GetComponent<GameGUI>();
		gameObject.active = false;		// Deactivate self ready to be triggered later
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (gameObject.active)
		{
			if (timer <= 0.0f)
				timer = delay;
			timer -= Time.deltaTime;
			if (timer <= 0.0f)
			{
				gui.Print( message, duration );
				gameObject.active = false;	// Deactivate again
				if (nextTrigger)
					nextTrigger.active = true;
			}
		}
	}
}
