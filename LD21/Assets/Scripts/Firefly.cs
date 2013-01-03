using UnityEngine;
using System.Collections;

public class Firefly : MonoBehaviour
{
	enum eStatus
	{
		Free,
		Held,
		Trapped
	};
	public int colour;	// 0,1,2 = R, G, B
	public Transform target;
	
	private eStatus status;
	private float fireflySpeed = 0.4f;
	private GameObject player;
	private GameObject jar;
	private JarScript jarscript;
	private CharMover charscript;
	private Vector3 pos = new Vector3(0,0,0);
	
	// Use this for initialization
	void Start ()
	{
		status = eStatus.Free;
		player = GameObject.Find( "Player" );
		if (player)
			charscript = player.GetComponent<CharMover>();
		jar = GameObject.Find( "Jar" );
		if (jar)
			jarscript = jar.GetComponent<JarScript>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch (status)
		{
		case Firefly.eStatus.Free:
			{
				//
				// Check distance to player
				//
				if (!charscript.IsHurt())
				{
					Vector3 dist = player.transform.position - transform.position;
					if (dist.sqrMagnitude < Mathf.Pow( 1.5f, 2 ))
					{
						//Debug.Log("HELD!");
						status = Firefly.eStatus.Held;	
					}
				}
			
				//
				// Move towards target
				//
				if (target)
				{
					pos.x = Mathf.MoveTowards( transform.position.x, target.position.x, fireflySpeed );
					pos.y = Mathf.MoveTowards( transform.position.y, target.position.y+1.5f, fireflySpeed );
					transform.position = pos;
				}
			}
			break;

		case Firefly.eStatus.Held:
			{
				//
				// Move towards player
				//
				if (player)
				{
					charscript.SetHolding();
					Vector3 grippos = player.transform.position + player.transform.forward * 1.5f;
					pos.x = Mathf.MoveTowards( transform.position.x, grippos.x, fireflySpeed*3.0f );
					pos.y = Mathf.MoveTowards( transform.position.y, grippos.y, fireflySpeed*3.0f );
					transform.position = pos;
				
					if (charscript.IsHurt())
						status = Firefly.eStatus.Free;
				}
				
				//
				// Check distance to jar
				//
				if (jar && jarscript)
				{
					Vector3 dist = transform.position - jar.transform.position;
					//Debug.Log("Dist="+dist.magnitude);
					if (dist.sqrMagnitude < Mathf.Pow( 3.0f, 2 ))
					{
						//Debug.Log("TRAPPED!");
						jarscript.AddFirefly();
						status = Firefly.eStatus.Trapped;
					}
				}
			}
			break;
			
		case Firefly.eStatus.Trapped:
			{
				//
				// Move towards target
				//
				if (jar)
				{
					pos.x = Mathf.MoveTowards( transform.position.x, jar.transform.position.x, fireflySpeed );
					pos.y = Mathf.MoveTowards( transform.position.y, jar.transform.position.y, fireflySpeed );
					transform.position = pos;
				}
			}
			break;
		}
	
	}
}
