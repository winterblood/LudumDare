using UnityEngine;
using System.Collections;

public class GuardBehaviour : MonoBehaviour
{
	public enum GuardState
	{
		Idle,
		Patrol,
		Investigate,
		Pursue,
		Attack,
		Flee
	}
	
	private GuardState guardState = GuardState.Patrol;
	private Vector3 lastSawPlayerAtPos;
	
	public float visionRange;
	public Transform[] waypoint = new Transform[8];
	
	private int nextWaypoint;
	private NavMeshAgent agent;
	private GameObject playerObj;
	private GameGUI gui;

	
	// Use this for initialization
	void Start ()
	{
		nextWaypoint = 0;
		agent = GetComponent<NavMeshAgent>();
		playerObj = GameObject.Find("First Person Controller");
		if (!playerObj)
			print("No player found!");
		
		GameObject guiobj = GameObject.Find( "HUD" );
		if (guiobj)
		{
			gui = guiobj.GetComponent<GameGUI>();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{		
		switch ( guardState )
		{
		case GuardState.Patrol:
			Patrol();
			break;
		case GuardState.Pursue:
			Pursue();
			break;
		case GuardState.Investigate:
			Investigate();
			break;	
		case GuardState.Attack:
			Attack();
			break;	
		default:
			break;
		}
	}
	
	void Patrol()
	{		
		if (agent)
		{
			agent.destination = waypoint[ nextWaypoint ].position;
			
			Vector3 dist = agent.destination - transform.position;
			if (dist.sqrMagnitude < 1.0f)
			{
				nextWaypoint++;
				if (!waypoint[ nextWaypoint ])
					nextWaypoint = 0;
			}
		}
		
		if (CanSeePlayer())
		{
			guardState = GuardState.Pursue;
		}
	}
	
	void Pursue()
	{		
		lastSawPlayerAtPos = playerObj.transform.position;
		if (agent)
		{
			agent.destination = lastSawPlayerAtPos;
			
			Vector3 dist = agent.destination - transform.position;
			if (dist.sqrMagnitude < 3.0f)
			{
				gui.SetState( GameGUI.GUIState.Death );
				guardState = GuardState.Patrol;
				return;
			}
		}
		
		if (!CanSeePlayer())
		{
			guardState = GuardState.Investigate;
		}
	}	
	
	void Investigate()
	{
		if (agent)
		{
			agent.destination = lastSawPlayerAtPos;
			
			Vector3 dist = agent.destination - transform.position;
			if (dist.sqrMagnitude < 2.0f)
			{
				guardState = GuardState.Patrol;
				return;
			}
		}
		
		if (CanSeePlayer())
		{
			guardState = GuardState.Pursue;
		}
	}	
	
	void Attack()
	{		
		lastSawPlayerAtPos = playerObj.transform.position;
		if (agent)
		{
			agent.destination = lastSawPlayerAtPos;
			
			Vector3 dist = agent.destination - transform.position;
			if (dist.sqrMagnitude > 2.0f)
			{
				guardState = GuardState.Pursue;
				return;
			}
		}
		
//		if (GetPlayerHealth() <= 0)
//		{
//			guardState = GuardState.Patrol;
//		}
	}
	
	
	bool CanSeePlayer()
	{
		if (!playerObj)
			return false;
		
		Vector3 eyePos = transform.position + Vector3.up * 1.8f;
		Vector3 vecToPlayer = playerObj.transform.position - eyePos;	// Vector from eye position
		float distToPlayer = vecToPlayer.magnitude;
		vecToPlayer /= distToPlayer;
		
		if (guardState != GuardState.Pursue)
			if (Vector3.Dot(vecToPlayer, transform.forward) < 0.707f )
				return false;		// outside view cone (test skipped if guard is pursuing)
			
		if (distToPlayer > visionRange)
			return false;		// outside visible range
		
		if (distToPlayer < 3.0f)
			return true;		// right under my nose
		
		if (gui.GetPlayerLight() < 0.25f)
			return false;		// too dark to see
		
		// Simple cases handled, have to check LoS now...
		RaycastHit hitinfo;
		if ( Physics.Raycast( eyePos, vecToPlayer, out hitinfo, distToPlayer))
		{
			return false;		// something blocked line of sight
		}
		
		return true;
	}
}
