using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerGenerator : MonoBehaviour
{
	public enum Tile
	{
		Empty,
		WallStone,
		WallNoCollide,
		WallCrumble
	}
	
	public enum TileFlag
	{
		None			= 0,
		TopClear		= 1<<0
	}

	public Object templateWallStone;
	public Object templateWallStoneNoCollide;
	public Object templateWallStoneCrumble;
	public Object templateCapStone;
	public Object templateDustPtl;
	public GameObject player;
	public AudioClip sfxTowerRise;
	public int ledgeDensity;

	class Map
	{
		public Vector3		Size;
		public Tile[,,] 	Tiles;
		public int[,,] 		TileFlags;
	}

	private Map mMap;

	void GenerateMap( int xsize, int ysize, int zsize, int seed )
	{
		Random.seed = seed;
		mMap = new Map();
		mMap.Size.x = xsize;
		mMap.Size.y = ysize;
		mMap.Size.z = zsize;
		mMap.Tiles 		= new Tile[xsize,ysize,zsize];
		mMap.TileFlags 	= new int[xsize,ysize,zsize];

		// Fill map with stone core and random surrounds
		for (int k=0; k<zsize; k++)
		{
			for (int i=0; i<xsize; i++)
			{
				for (int j=0; j<ysize; j++)
				{
					mMap.TileFlags[i,j,k] = (int)TileFlag.None;

					if ((i==0 && j==0) || (i==0 && j==ysize-1) || (i==xsize-1 && j==0) || (i==xsize-1 && j==ysize-1))
					{
						//
						// Corner space, leave empty
						//
						mMap.Tiles[i,j,k] = Tile.Empty;
					}
					else if (i==0 || i==xsize-1 || j==0 || j==ysize-1)
					{
						//
						// Outer edge of tower, where all the fun stuff happens
						//
						int r = Random.Range(0,100);
						if (r < ledgeDensity)
							mMap.Tiles[i,j,k] = Tile.WallStone;
						else
							mMap.Tiles[i,j,k] = Tile.Empty;
					}
					else if (i==1 || i==xsize-2 || j==1 || j==ysize-2)
					{
						//
						// Main wall of tower, solid for now (may add holes later)
						//
						mMap.Tiles[i,j,k] = Tile.WallStone;
					}
					else
					{
						//
						// Interior of tower - empty for now
						//
						mMap.Tiles[i,j,k] = Tile.Empty;
					}

				}
			}
		}
		
		//
		// Flags pass
		//
		for (int k=0; k<zsize; k++)
		{
			for (int i=0; i<xsize; i++)
			{
				for (int j=0; j<ysize; j++)
				{
					if (mMap.Tiles[i,j,k] == Tile.WallStone)
					{
						if ((k == zsize-1) || (mMap.Tiles[i,j,k+1] == Tile.Empty))
						{
							mMap.TileFlags[i,j,k] |= (int)TileFlag.TopClear;
							if ((k > 0) && (mMap.Tiles[i,j,k-1] == Tile.Empty))
							{
								if (k>9 && Random.Range(0,100) < 50)
									mMap.Tiles[i,j,k] = Tile.WallCrumble;	// One in ten above a set height will crumble away when stood on
							}
						}
					}
				}
			}
		}
	}

	private Vector3 centrepos = Vector3.zero;
	private float cubesize = 2.5f;
	private float cubesize_reciprocal = 1.0f;

	void PopulateScene()
	{
		Debug.Log( "PopulateScene" );

		cubesize_reciprocal = 1.0f/cubesize;
		Vector3 pos;
		GameObject clone;

		for (int k=0; k<mMap.Size.z; k++)
		{
			for (int i=0; i<mMap.Size.x; i++)
			{
				for (int j=0; j<mMap.Size.y; j++)
				{
					Tile tile = mMap.Tiles[i,j,k];
					if (tile != Tile.Empty)
					{
						Object template;
						switch (tile)
						{
						case Tile.WallStone:	template = templateWallStone; 			break;
						case Tile.WallNoCollide:template = templateWallStoneNoCollide; 	break;
						case Tile.WallCrumble:	template = templateWallStoneCrumble;	break;
						default:				template = templateWallStone;			break;
						}
						pos = new Vector3((float)i * cubesize, ((float)k * cubesize) + cubesize*0.5f, (float)j * cubesize);
						clone = (GameObject)Instantiate(template, pos, Quaternion.identity);
						clone.GetComponent<Transform>().parent = transform;
					}
				}
			}
		}

		pos = new Vector3(3.0f * cubesize, mMap.Size.z * cubesize, 3.0f * cubesize);
		clone = (GameObject)Instantiate(templateCapStone, pos, Quaternion.identity);
		clone.GetComponent<Transform>().parent = transform;

		Vector3 undergroundpos = transform.position;
		undergroundpos.y -= (mMap.Size.z * cubesize) + 0.5f;
		transform.position = undergroundpos;

		centrepos = transform.position;
		centrepos.x += mMap.Size.x * cubesize * 0.5f;
		centrepos.z += mMap.Size.y * cubesize * 0.5f;
	}

	public float GetCubeSize()
	{
		return cubesize;
	}

	public Vector3 GetCentrePos()
	{
		return centrepos;
	}

	public bool IsCubeAtPositionClimbable ( Vector3 pos )
	{
		Vector3 coord = pos - transform.position;
		coord.y -= cubesize*0.5f;
		coord *= cubesize_reciprocal;

		int i = (int)coord.x;
		int j = (int)coord.z;
		int k = (int)coord.y;
		if ((i < 0 || i >= mMap.Size.x) &&
			(j < 0 || j >= mMap.Size.y) &&
			(k < 0 || k >= mMap.Size.z))
			return false;
			
		if (i==3 && j==3)
			return false;	// Do not allow mantling onto central objects

		if ((mMap.TileFlags[i,j,k] & (int)TileFlag.TopClear) == 0)
			return false;
/*
		// Now check that there is room for player to mantle from the side he is on
		coord = player.transform.position - transform.position;
		coord.y -= cubesize*0.5f + 1.0f;
		coord *= cubesize_reciprocal;
		int pi = (int)coord.x;
		int pj = (int)coord.z;
		int pk = (int)coord.y;
		if ((pi < 0 || pi >= mMap.Size.x) &&
			(pj < 0 || pj >= mMap.Size.y) &&
			(pk < 0 || pk >= mMap.Size.z-1))
			return true;	// Player off map or at top? No problem

		if (mMap.Tiles[pi,pj,pk+1] != Tile.Empty)
		{
			Debug.Log("Mantle blocked!");
			return false;
		}
*/
		return true;
	}

	private bool towerTriggered = false;

	// Use this for initialization
	void Start ()
	{
		GenerateMap( 7, 7, 25, 3 );
		PopulateScene();
	}

	void Update ()
	{
		if (!towerTriggered)
		{
			Vector3 dist = player.transform.position - GetCentrePos();
			dist.y = 0.0f;
			if (dist.sqrMagnitude < 30.0f * 30.0f)
			{
				towerTriggered = true;
				audio.PlayOneShot( sfxTowerRise );
			}
		}
		
		if (towerTriggered)
		{
			Vector3 rising_pos = transform.position;
			if (transform.position.y < 0.0f)
			{
				rising_pos.y += (mMap.Size.z + 1) * cubesize * 0.11f *  Time.deltaTime;
				if (rising_pos.y > 0.0f)
					rising_pos.y = 0.0f;

				//
				// Generate dust ptls!
				//
				Vector3 pos = new Vector3(0.0f, 0.5f, 0.0f);
				int r = Random.Range(0,100);
				if (r<=25)
				{
					pos.x = Random.Range(0,(mMap.Size.x-1) * cubesize);
				}
				else if (r<=50)
				{
					pos.x = Random.Range(0,(mMap.Size.x-1) * cubesize);
					pos.z = mMap.Size.x * cubesize;
				}
				else if (r<=75)
				{
					pos.z = Random.Range(0,(mMap.Size.y-1) * cubesize);
				}
				else
				{
					pos.x = mMap.Size.x * cubesize;
					pos.z = Random.Range(0,(mMap.Size.y-1) * cubesize);
				}

				GameObject clone = (GameObject)Instantiate( templateDustPtl, pos, Quaternion.identity );
			}
			else
			{
				rising_pos.y = 0.0f;
			}
			transform.position = rising_pos;


		}
	}
}
