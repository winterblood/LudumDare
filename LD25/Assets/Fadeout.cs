using UnityEngine;
using System.Collections;

public class Fadeout : MonoBehaviour
{
	private float alpha;
	
	// Use this for initialization
	void Start ()
	{
		alpha = 0.0f;
		gameObject.SetActive( false );
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (alpha<=0.0f)
			alpha = 0.3f;
		else
		{
			alpha -= Time.deltaTime;
			if (alpha <= 0.0f)
				gameObject.SetActive( false );
		}
	}
}
