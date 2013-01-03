using UnityEngine;
using System.Collections;

public class Thorns : MonoBehaviour
{
	
	private GameObject player;
	private CharMover charscript;
	
	// Use this for initialization
	void Start ()
	{
		player = GameObject.Find( "Player" );
		if (player)
			charscript = player.GetComponent<CharMover>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 dist = player.transform.position - transform.position;
		if (dist.sqrMagnitude < Mathf.Pow( 3.5f, 2 ))
		{
			//Debug.Log("OUCH!");
			charscript.SetHurt( transform );
		}
	}
}
