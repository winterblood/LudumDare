using UnityEngine;
using System.Collections;

public class DeathHandler : MonoBehaviour
{
	void Start ()
	{

	}

	void OnTriggerEnter( Collider other )
	{
		Debug.Log ("Dead!");
		
		GameObject player = (GameObject)GameObject.Find ("PlayerBall");
		if (player)
		{
			BallControl ctrl = (BallControl)player.GetComponent<BallControl>();
			if (ctrl)
			{
				ctrl.Reset();
			}
		}
		GameObject maze = (GameObject)GameObject.Find ("Maze");
		if (maze)
		{
			MazeMorph morph = (MazeMorph)maze.GetComponent<MazeMorph>();
			if (morph)
			{
				morph.Reset( 0 );
			}
		}
	}

	void Update ()
	{

	}
}