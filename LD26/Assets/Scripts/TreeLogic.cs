using UnityEngine;
using System.Collections;

public class TreeLogic : MonoBehaviour
{

	enum eTreeState
	{
		DEAD,
		REVIVING,
		COMPLETE
	};
	
	public float bloomDuration = 3.0f;
	public float bloomRadius = 12.0f;
	public bool testCamera = false;
	
	private Landscape landscape;
	private GameObject player;
	private GameObject megaTree;
	private CameraBlender cameraBlender;
	private CameraBlend cutawayCam;
	private eTreeState state = eTreeState.DEAD;
	private float reviveTimer = 0.0f;
	
	private int megaTreeChildren = 0;
	private int megaTreeChildrenAlive = 0;

	
	// Use this for initialization
	void Start ()
	{
		GameObject landscapeObj = GameObject.Find( "Landscape" );
		landscape = landscapeObj.GetComponent<Landscape>();
		
		megaTree = landscape.GetMegaTree();
		
		player = GameObject.Find( "Player" );
		
		cameraBlender = Camera.main.gameObject.GetComponent<CameraBlender>();
		cutawayCam = new CameraBlend();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (IsMegaTree() && testCamera)
		{
			cutawayCam.look = Vector3.up * 30.0f;
			cutawayCam.pos = Vector3.up * 130.0f + Vector3.forward * 100.0f;
			cutawayCam.fov = 30.0f;
			cutawayCam.guid = 666;
			cutawayCam.priority = 100;
			cameraBlender.RequestCamera( ref cutawayCam );
		}
	
		switch (state)
		{
		case eTreeState.DEAD:
			{
				if (megaTreeChildren>0)
				{
					if (megaTreeChildrenAlive == megaTreeChildren)
					{
						state = eTreeState.REVIVING;
					}
				}
				else
				{
					Vector3 distToPlayer = transform.position - player.transform.position;
					distToPlayer.y = 0.0f;
					
					if (distToPlayer.sqrMagnitude < 2.0f*2.0f)
					{
						state = eTreeState.REVIVING;
					}
				}
			}
			break;
		case eTreeState.REVIVING:
			{
				reviveTimer += Time.deltaTime;
				
				float bloomRatio = reviveTimer/bloomDuration;
				
				if (!IsMegaTree())	// Shoot linear effect across landscape to MegaTree
				{
					Vector3 shootVec = megaTree.transform.position - transform.position;
					UpdateShoot( transform.position + shootVec * bloomRatio );
					
					cutawayCam.look = transform.position + shootVec * 0.75f;
					cutawayCam.pos = transform.position - shootVec * 0.5f;
					cutawayCam.pos.y = transform.position.y + 5.0f;
					cutawayCam.pos += Vector3.forward;
					cutawayCam.fov = 90.0f;
					cutawayCam.guid = 54321;
					cutawayCam.priority = 100;
					cameraBlender.RequestCamera( ref cutawayCam );
				}

				bloomRatio = 1.0f - (1.0f-bloomRatio)*(1.0f-bloomRatio);
				landscape.ColourTexture( transform.position, bloomRatio*bloomRadius, new Color( 0.25f, 0.5f, 0.25f, 0.25f) );
												
				if (reviveTimer > bloomDuration)
				{
					state = eTreeState.COMPLETE;
					if (!IsMegaTree())
						megaTree.GetComponent<TreeLogic>().NotifyMegaTree();
				}
			}
			break;
		case eTreeState.COMPLETE:
			break;
		}
	}
	
	void UpdateShoot( Vector3 pos )
	{
		landscape.ColourTexture( pos, 0.0f, Color.green );
		
		// Also do particles etc.
	}
	
	void OnGUI()
	{
		if (IsMegaTree())
		{
			GUI.Label( new Rect(10,10,200,20), "Trees "+megaTreeChildrenAlive+"/"+megaTreeChildren );
		}
	}
	
	public bool IsMegaTree()
	{
		return megaTreeChildren>0;
	}
	
	public void SetMegaTree( int count )
	{
		float s = 3.0f;
		transform.localScale.Set( s,s,s );
		
		megaTreeChildren = count;
		megaTreeChildrenAlive = 0;
	}
	
	public void NotifyMegaTree()
	{
		megaTreeChildrenAlive++;
	}
	
	public bool IsCompleted()
	{
		return (state == eTreeState.COMPLETE);
	}
}
