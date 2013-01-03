using UnityEngine;
using System.Collections;

public class Touch : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 dir = transform.forward;
		RaycastHit hitinfo;
  		if ( Physics.Raycast( transform.position, dir, out hitinfo, 3.0f))
		{
			Interactive inter = hitinfo.transform.gameObject.GetComponent<Interactive>();	// Try this object
			if (!inter && hitinfo.transform.parent)
				inter = hitinfo.transform.parent.gameObject.GetComponent<Interactive>();	// If not, try it's parent
			if (inter)
			{
				inter.PlayerMightTouch( hitinfo.distance, Input.GetMouseButtonDown(0) );
			}
			else
			{
				//print( "Raycast hit inert object " + hitinfo.transform.name + " at range " + hitinfo.distance );
			}
    	}
	}
}
