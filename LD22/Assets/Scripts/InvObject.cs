using UnityEngine;
using System.Collections;

public class InvObject : MonoBehaviour
{	
	public float proximity = 2.5f;
	public string descShort;
	public string descLong;
	public float duration = 3.0f;
	public GameObject activateWhenPickedUp;
	
	private GameObject world;
	private GameGUI gui;
	private Transform player;
	private CharMover mover;
	private bool seenLongDesc = false;
	
	// Use this for initialization
	void Start ()
	{
		GameObject plr = GameObject.Find("Player");
		mover = plr.GetComponent<CharMover>();
		player = plr.transform;
		world = GameObject.Find("World");
		gui = world.GetComponent<GameGUI>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		// If player is near object, pop up name
		Vector3 dist = player.position - transform.position;
		if (dist.sqrMagnitude < proximity*proximity)
		{
			if (!gui.IsMessageShown())
				gui.Print( descShort, 0.5f );
		
			// If player picks up object, pop up long desc
			if (mover.IsInteracting() && !seenLongDesc)
			{
				gui.AddToInventory( gameObject );
				gui.Print( descLong, duration );
				seenLongDesc = true;
				if (activateWhenPickedUp)
					activateWhenPickedUp.active = true;
			}
		}	
	}
}
