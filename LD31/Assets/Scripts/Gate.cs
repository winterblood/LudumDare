using UnityEngine;
using System.Collections;

public class Gate : MonoBehaviour
{
	public int targetMap = 0;

	void Start ()
	{

	}

	void OnTriggerEnter( Collider other )
	{
		Debug.Log ("Gate!");
		
		bool changed = false;
		GameObject maze = (GameObject)GameObject.Find ("Maze");
		if (maze)
		{
			MazeMorph morph = (MazeMorph)maze.GetComponent<MazeMorph>();
			if (morph)
			{
				changed = morph.Reset( targetMap );
			}
		}		
		
		if (changed)
		{
			GameObject player = (GameObject)GameObject.Find ("PlayerBall");
			if (player)
			{
				BallControl ctrl = (BallControl)player.GetComponent<BallControl>();
				if (ctrl)
				{
					ctrl.Immobilise();
				}
			}
		}
	}

	void Update ()
	{

	}
}