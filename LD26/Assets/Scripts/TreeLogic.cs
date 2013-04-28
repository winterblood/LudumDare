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
	
	private Landscape landscape;
	private GameObject player;
	private GameObject megaTree;
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
	}
	
	// Update is called once per frame
	void Update ()
	{
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
					
					if (distToPlayer.sqrMagnitude < 1.0f)
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
				//bloomRatio = 1.0f - (bloomRatio*bloomRatio);
				landscape.ColourTexture( transform.position, bloomRatio*bloomRadius, Color.green );
				
				if (!IsMegaTree())
				{
					Vector3 shootVec = megaTree.transform.position - transform.position;
					UpdateShoot( transform.position + shootVec * bloomRatio );
				}
				
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
