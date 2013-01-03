using UnityEngine;
using System.Collections;

public class Socket : MonoBehaviour
{
	public GameObject plugObject;
	public float proximity = 3.0f;
	public string descHint;
	public string descComplete;
	public float duration = 5.0f;
	public GameObject activateWhenComplete;
	public GameObject deactivateWhenComplete;
	
	private GameObject world;
	private GameGUI gui;
	private Transform player;
	private CharMover mover;
	
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
		if (!plugObject)
			return;
		
		// If player is near socket...
		Vector3 dist = player.position - transform.position;
		dist.z = 0.0f;
		if (dist.sqrMagnitude < proximity*proximity)
		{
			if (gui.IsCarrying(plugObject))
			{
				if (mover.IsInteracting())
				{
					gui.Print( descComplete, duration );
					gui.RemoveFromInventory(plugObject);
					gameObject.SetActiveRecursively( false );
					if (activateWhenComplete)
						activateWhenComplete.SetActiveRecursively( true );
					if (deactivateWhenComplete)
						deactivateWhenComplete.SetActiveRecursively( false );
				}
			}
			else
			{
				if (descHint!=null && descHint.Length>0 && !gui.IsMessageShown())
					gui.Print( descHint, 0.5f );
			}			
		}	
	}
}
