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
	public float bloomRadius = 7.0f;
	
	private Landscape landscape;
	private GameObject player;
	private GameObject megaTree;
	private eTreeState state;
	private float reviveTimer;

	
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
				Vector3 distToPlayer = transform.position - player.transform.position;
				distToPlayer.y = 0.0f;
				
				if (distToPlayer.sqrMagnitude < 1.0f)
				{
					state = eTreeState.REVIVING;
				}
			}
			break;
		case eTreeState.REVIVING:
			{
				reviveTimer += Time.deltaTime;
				
				float bloomRatio = reviveTimer/bloomDuration;
				bloomRatio = 1.0f - (bloomRatio*bloomRatio);
				landscape.ColourTexture( transform.position, bloomRatio*bloomRadius, Color.green );
				
				Vector3 shootVec = megaTree.transform.position - transform.position;
				UpdateShoot( transform.position + shootVec * bloomRatio );
				
				if (reviveTimer > bloomDuration)
				{
					state = eTreeState.COMPLETE;
				}
			}
			break;
		case eTreeState.COMPLETE:
			break;
		}
	}
	
	void UpdateShoot( Vector3 pos )
	{
		landscape.ColourTexture( transform.position, 0.6f, Color.green );
		
		// Also do particles etc.
	}
	
	public bool IsCompleted()
	{
		return (state == eTreeState.COMPLETE);
	}
}
