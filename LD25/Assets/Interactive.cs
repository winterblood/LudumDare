using UnityEngine;
using System.Collections;

public class Interactive : MonoBehaviour
{

	public float touchRange;
	public GameObject hudHoverOn;
	public GameObject hudHoverOff;
	public GameObject activeSubset;
	public bool toggleable;
	private float timeSinceTouched;
	
	void Start ()
	{
		timeSinceTouched = 10.0f;
		if (touchRange == 0.0f)
			touchRange = 1.0f;
	}
	
	void Update ()
	{
		if (timeSinceTouched < 10.0f)
			timeSinceTouched += Time.deltaTime;
		
		if (timeSinceTouched > 0.2f)
		{
			hudHoverOn.SetActive( false );
			hudHoverOff.SetActive( false );
		}
	}
	
	// Called by player if crosshair raycast touches the object
	public void PlayerMightTouch( float range, bool touch )
	{
		//print( "Touching at range " + range );
		
		if (!activeSubset)
			return;
		
		if (range < touchRange)
		{
			timeSinceTouched = 0.0f;
			
			if (toggleable)
			{
				// Turn on and off
				if (activeSubset.activeSelf && hudHoverOn)
					hudHoverOn.SetActive(true);
				else if (hudHoverOff)
					hudHoverOff.SetActive(true);
				
				if (touch)
				{
					activeSubset.SetActive( !activeSubset.activeSelf );
				}
			}
			else
			{
				// Activate once only
				if (hudHoverOn)
					hudHoverOn.SetActive( !activeSubset.activeSelf );

				if (touch)
				{
					activeSubset.SetActive( true );
				}
			}
		}
	}
}
