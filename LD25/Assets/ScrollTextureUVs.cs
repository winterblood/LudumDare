using UnityEngine;
using System.Collections;

public class ScrollTextureUVs : MonoBehaviour
{
	public float scrollSpeed;
	public bool scrollHorizontal;
	public bool scrollVertical;
	private float scrollValue;
	
	void Start ()
	{
		scrollValue = 0.0f;
	}

	void Update ()
	{
		scrollValue += (Time.deltaTime * scrollSpeed) % 1.0f;
    	renderer.material.mainTextureOffset = new Vector2( scrollHorizontal?scrollValue:0.0f, scrollVertical?scrollValue:0.0f);
	}
}
