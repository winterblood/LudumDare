using UnityEngine;
using System.Collections;

public class InactiveAtStart : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		gameObject.SetActive( false );
	}
	
	void Update ()
	{
		print ("ACTIVATED!");
	}
}
