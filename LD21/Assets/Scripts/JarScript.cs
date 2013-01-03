using UnityEngine;
using System.Collections;

public class JarScript : MonoBehaviour
{
	public int numFirefliesRequired;
	
	private int numFirefliesTrapped;
	
	
	// Use this for initialization
	void Start ()
	{
		numFirefliesTrapped = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (numFirefliesTrapped >= numFirefliesRequired)
		{
			//Debug.Log("AREA CLEAR");
		}
	}
	
	public void AddFirefly()
	{
		numFirefliesTrapped++;
	}
}
