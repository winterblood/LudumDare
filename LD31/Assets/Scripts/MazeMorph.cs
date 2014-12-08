using UnityEngine;
using System.Collections;

public class MazeMorph : MonoBehaviour
{
	public GameObject cube;
	public GameObject gateRed;
	public GameObject gateBlue;
	public GameObject gateYellow;
	public GameObject gateGreen;
	public int activeBlueprint = 3;
	public float blendSpeed = 2.0f;
	public Texture2D levelMap;
	public Texture2D titleImage;
	public Texture2D victoryImage;
	
	private GameObject[,] maze;
	private int[,,] blueprints;
	private float endSequenceTimer = 0.0f;
	private float titleSequenceTimer = 0.0f;
	
	enum mazeTile
	{
		tileWater,
		tileFloor,
		tileWall,
		tileYellow,
		tileRed,
		tileBlue,
		tileGreen
	};
	
	void Start ()
	{
		maze = new GameObject[16,16];
		blueprints = new int[16,16,4];		

		GenerateBlueprints();

		Vector3 pos;
		for (int i=0; i<16; i++)
		{		
			for (int j=0; j<16; j++)
			{
				pos.x = (float)i-7.5f;
				pos.y = blueprints[i,j,activeBlueprint]==1 ? 0.4f : -0.5f;
				pos.z = (float)j-7.5f;
				switch (blueprints[i,j,activeBlueprint])
				{
				case 3:		maze[i,j] = (GameObject)GameObject.Instantiate( gateRed ); break;
				case 4:		maze[i,j] = (GameObject)GameObject.Instantiate( gateGreen ); break;
				case 5:		maze[i,j] = (GameObject)GameObject.Instantiate( gateBlue ); break;
				case 6:		maze[i,j] = (GameObject)GameObject.Instantiate( gateYellow ); break;
				default: 	maze[i,j] = (GameObject)GameObject.Instantiate( cube ); break;
				}
				maze[i,j].transform.position = pos;
				maze[i,j].transform.parent = gameObject.transform;
			}
		}
		
		titleSequenceTimer = 3.0f;
	}

	void OnTriggerEnter( Collider other )
	{
		Debug.Log ("Winner!");
		endSequenceTimer = 3.0f;
	}

	void GenerateBlueprints()
	{
		Color col;
		Color yellow = new Color( 1.0f, 1.0f, 0.0f );	// Default yellow is not pure
		for (int k=0; k<4; k++)
		{
			for (int i=0; i<16; i++)
			{		
				for (int j=0; j<16; j++)
				{
					col = levelMap.GetPixel(i, j+(16*k));
				
					if (col.Equals(Color.white))
						blueprints[i,j,k] = 2; 
					else if (col.Equals(Color.black))
						blueprints[i,j,k] = 1;
					else if (col.Equals(Color.cyan))
						blueprints[i,j,k] = 0;
					else if (col.Equals(Color.red))
						blueprints[i,j,k] = 3;	
					else if (col.Equals(Color.green))
						blueprints[i,j,k] = 4;	
					else if (col.Equals(Color.blue))
						blueprints[i,j,k] = 5;	
					else if (col.Equals(yellow))
						blueprints[i,j,k] = 6;
					else
					{
						Debug.Log ( "RGB=" + col.r + " " + col.g + " " + col.b );
						blueprints[i,j,k] = 1;	// Default to floor!
					}
				}
			}
		}
	}
	
	public bool Reset( int map )
	{
		if (map != activeBlueprint)
		{
			Debug.Log ( "New map " + map );
			activeBlueprint = map;
			return true;
		}
		return false;	
	}	
	
	void Update ()
	{
		if (endSequenceTimer > 0.0f)
		{
			endSequenceTimer -= Time.deltaTime;
			
			Rect pos = new Rect( 100, 100, 800, 283 );
			GUI.DrawTexture( pos, victoryImage );			
			
			if (endSequenceTimer<0.0f)
			{
				Reset( 0 );
				GameObject player = (GameObject)GameObject.Find ("PlayerBall");
				if (player)
				{
					BallControl ctrl = (BallControl)player.GetComponent<BallControl>();
					if (ctrl)
					{
						ctrl.Reset();
					}
				}
				endSequenceTimer = 0.0f;
			}
		}
		
		if (titleSequenceTimer > 0.0f)
		{
			titleSequenceTimer -= Time.deltaTime;
			
			//Rect pos = new Rect( 100, 100, 800, 283 );
			//GUI.DrawTexture( pos, titleImage );
			
			if (titleSequenceTimer<0.0f)
			{
				titleSequenceTimer = 0.0f;
			}
		}
	}
	
	void FixedUpdate ()
	{	
		Vector3 pos;
		float dt = Time.fixedDeltaTime;
		for (int i=0; i<16; i++)
		{		
			for (int j=0; j<16; j++)
			{
				if (!maze[i,j])
					continue;
			
				pos = maze[i,j].transform.position;
				float tgt_y = -1.7f;
				switch (blueprints[i,j,activeBlueprint])
				{
				case 0: 	tgt_y = -1.7f; break;
				default:
				case 1: 	tgt_y = -0.5f; break;
				case 2: 	tgt_y = 0.3f; break;
				}
				if (pos.y > tgt_y)
				{
					pos.y -= dt * blendSpeed;
					if (pos.y < tgt_y)
						pos.y = tgt_y;
				}
				else if (pos.y < tgt_y)
				{
					pos.y += dt * blendSpeed;
					if (pos.y > tgt_y)
						pos.y = tgt_y;
				}
				maze[i,j].transform.position = pos;
			}
		}
	}
}