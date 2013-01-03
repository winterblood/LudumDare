using UnityEngine;
using System.Collections;

public class CollectGold : MonoBehaviour
{
	public int goldValue;
	private GameGUI gui;
	
	// Use this for initialization
	void Start ()
	{
		GameObject guiobj = GameObject.Find( "HUD" );
		if (guiobj)
		{
			gui = guiobj.GetComponent<GameGUI>();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (gui)
		{
			gui.AddGold( goldValue );
			transform.parent.gameObject.SetActive( false );
		}
	}
}
