using UnityEngine;
using System.Collections;

public enum eCellType
{
	empty,
	wall,
	block,
	player,
	exit,
	key
}

public class Cell
{
	public eCellType type;
	public int playerTime;
}

public class TimeJump : MonoBehaviour
{
	enum eGameState
	{
		title,
		play,
		win,
		lose,
		finished
	}
	
	enum eAction
	{
		none,
		hold,
		north,
		south,
		east,
		west,
		timejump
	}
	
	public int			gridSize = 8;
	public int			historySize = 300;
	
	public int 			cellSize = 60;
	public Texture		texWall;
	public Texture		texPlayer;
	public Texture		texPlayerPast;
	public Texture		texBlock;
	public Texture		texExit;
	public Texture		texExitLocked;
	public Texture		texBackground;
	public Texture		texKey;
	public Material		digiNumerals;
	public AudioClip	sfxWin;
	public AudioClip	sfxLose;
	public AudioClip	sfxKey;
	public AudioClip	sfxPush;
	public AudioClip	sfxTimeJump;
	public AudioClip	sfxInvalid;
	public AudioClip	sfxAccept;
	public int			startLevel = 0;
	public GUIStyle		style;
	
	private int 			globalTime = 0;
	private int 			playerTime = 0;
	private float			turnProgress = -1.0f;
	private float			turnStartTime = 0.0f;
	private bool			waitForRelease = false;
	private bool			hasKey = false;
	private bool			lastLevel = false;
	private int				level = 0;
	
	// Game loop
	private eGameState		gameState = eGameState.title;
	
	private Cell[,] map;
	private Cell[,] mapFuture;
	private Cell[,] mapPast;
	private Cell[,] mapStart;
	private eAction[] actionHistory;
	
	private const string level1 = "......";
	
	void AllocateMap( Cell[,] map )
	{
		// TODO: allocate these in a contiguous block for speed if time allows
	
		for (int x=0; x<gridSize; x++)
		{
			for (int y=0; y<gridSize; y++)
			{
				map[x,y] = new Cell();
			}
		}
	}

	void CopyMap( Cell[,] mapfrom, Cell[,] mapto )
	{
		for (int x=0; x<gridSize; x++)
		{
			for (int y=0; y<gridSize; y++)
			{
				mapto[x,y].type = mapfrom[x,y].type;
				mapto[x,y].playerTime = mapfrom[x,y].playerTime;
			}
		}
	}

	void ZeroMapTimes( Cell[,] map )
	{
		for (int x=0; x<gridSize; x++)
		{
			for (int y=0; y<gridSize; y++)
			{
				map[x,y].playerTime = 0;
			}
		}
	}	
			
									
	// Use this for initialization
	void Awake ()
	{
		actionHistory 	= new eAction[historySize];
		map 			= new Cell[gridSize,gridSize];
		mapFuture 		= new Cell[gridSize,gridSize];
		mapPast 		= new Cell[gridSize,gridSize];
		mapStart		= new Cell[gridSize,gridSize];
		
		AllocateMap(map);
		AllocateMap(mapFuture);
		AllocateMap(mapPast);
		AllocateMap(mapStart);

		InitLevel();
	}
	
	void InitLevel()
	{
		hasKey = false;
	
		for (int x=0; x<gridSize; x++)
		{
			for (int y=0; y<gridSize; y++)
			{
				eCellType cellType = eCellType.empty;
				if (x==0 || y==0 || x==gridSize-1 || y==gridSize-1)
					cellType = eCellType.wall;
			
				map[x,y].type = cellType;
				map[x,y].playerTime = -1;
			}
		}
		
		switch (level)
		{
		case 0:		// Intro
			// Spawn player
			map[1,1].type = eCellType.player;
			map[1,1].playerTime = 0;
	
			map[4,4].type = eCellType.key;
	
			// Spawn exit
			map[6,6].type = eCellType.exit;
			break;
			
		case 1:		// Require time loop
			// Spawn player
			map[1,1].type = eCellType.player;
			map[1,1].playerTime = 0;
	
			// Spawn blocks
			map[1,2].type = eCellType.wall;	
			map[2,2].type = eCellType.wall;
			map[3,2].type = eCellType.wall;	
			map[4,2].type = eCellType.wall;	
			map[3,5].type = eCellType.wall;	
			map[4,5].type = eCellType.wall;	
			map[5,5].type = eCellType.wall;	
			map[6,5].type = eCellType.wall;	
			
			map[4,4].type = eCellType.key;
	
			// Spawn exit
			map[6,6].type = eCellType.exit;
			break;

		case 2:		// Show crates
			// Spawn player
			map[1,1].type = eCellType.player;
			map[1,1].playerTime = 0;
	
			// Spawn walls
			map[1,2].type = eCellType.wall;	
			map[2,2].type = eCellType.wall;
			map[3,2].type = eCellType.wall;	
			map[4,2].type = eCellType.wall;
			map[5,2].type = eCellType.wall;		
			map[2,5].type = eCellType.wall;	
			map[3,5].type = eCellType.wall;	
			map[4,5].type = eCellType.wall;	
			map[5,5].type = eCellType.wall;	
			map[6,5].type = eCellType.wall;	
	
			map[3,1].type = eCellType.block;
			map[3,6].type = eCellType.block;
			
			map[4,4].type = eCellType.key;	
	
			// Spawn exit
			map[6,6].type = eCellType.exit;
			break;

		case 3:		// Retrace your steps
			// Spawn player
			map[1,1].type = eCellType.player;
			map[1,1].playerTime = 0;
	
			// Spawn walls
			map[2,3].type = eCellType.wall;
			map[3,3].type = eCellType.wall;	
			map[4,3].type = eCellType.wall;
			map[5,3].type = eCellType.wall;		
			map[2,4].type = eCellType.block;	
			map[2,5].type = eCellType.wall;	
			map[5,4].type = eCellType.wall;	
			map[5,5].type = eCellType.wall;	
			map[5,6].type = eCellType.wall;	
	
			//map[3,1].type = eCellType.block;
			//map[3,6].type = eCellType.block;
			
			map[3,4].type = eCellType.key;		
	
			// Spawn exit
			map[6,6].type = eCellType.exit;
			break;						

		case 4:		// Retrace with blocks 
			// Spawn player
			map[1,1].type = eCellType.player;
			map[1,1].playerTime = 0;
	
			// Spawn walls
			map[1,2].type = eCellType.wall;
			map[2,2].type = eCellType.wall;	
			map[3,2].type = eCellType.wall;
			map[5,2].type = eCellType.wall;		
			map[5,3].type = eCellType.wall;	
			map[3,4].type = eCellType.wall;	
			map[5,4].type = eCellType.wall;	
			map[5,5].type = eCellType.wall;	
			map[5,6].type = eCellType.wall;
			map[4,6].type = eCellType.wall;	
	
			map[4,2].type = eCellType.block;
			//map[3,6].type = eCellType.block;
			
			map[2,5].type = eCellType.key;		
	
			// Spawn exit
			map[6,6].type = eCellType.exit;
			lastLevel = true;
			break;																																					
		}
												
		CopyMap( map, mapStart );	// mapStart gets duplicate players added to it as player progresses	
		
		globalTime = 0;
		playerTime = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (turnProgress >= 0.0f)
		{
			Time.timeScale = 1.0f;
			turnProgress = (Time.realtimeSinceStartup - turnStartTime) * 2.0f;
			if (turnProgress>1.0f)
			{
				turnProgress = -1.0f;
			}
		}
		else if (gameState == eGameState.play)
		{
			Time.timeScale *= 0.96f;
		}
		else
		{
			Time.timeScale = 1.0f;
		}
		
		digiNumerals.mainTextureOffset = new Vector2( 0.0f, 0.1f*globalTime );
		
		if (turnProgress < 0.0f && gameState==eGameState.play)
		{
			eAction action = eAction.none;
			
			// Read input, update action accordingly
			float stickX = Input.GetAxisRaw("Horizontal");
			float stickY = Input.GetAxisRaw("Vertical");
			
			if (stickX < -0.15f)
				action = eAction.west;
			else if (stickX > 0.15f)
				action = eAction.east;
			else if (stickY > 0.15f)
				action = eAction.north;
			else if (stickY < -0.15f)
				action = eAction.south;
			else if (Input.GetKeyDown( KeyCode.T ))
				action = eAction.timejump;
			else
				waitForRelease = false;
			
			if (action != eAction.none && !waitForRelease)
			{
				waitForRelease = true;
				RecordAction( action );
				ProcessTurn();
				
				turnStartTime = Time.realtimeSinceStartup;
				turnProgress = 0.0f;
			}
		}
	}
	
	void RecordAction ( eAction action )
	{
		//Debug.Log( "RecordAction = " + action);
		actionHistory[playerTime] = action;
	}

	void TakeAction ( int x, int y, eAction action )
	{
		if (map[x,y].type != eCellType.player)
			Debug.DebugBreak();
	
		map[x,y].playerTime++;	// Increment player time before they move so it gets copied into new cell
	
		switch (action)
		{
		case eAction.north:
			Move( x, y, x, y-1 );
			break;
		case eAction.south:
			Move( x, y, x, y+1 );
			break;
		case eAction.east:
			Move( x, y, x+1, y );
			break;
		case eAction.west:
			Move( x, y, x-1, y );
			break;
		case eAction.timejump:
			DoTimeJump( x, y );
			break;
		}
	}
	
	bool DoTimeJump( int x, int y )
	{
		if (map[x,y].playerTime < playerTime)
		{
			Debug.Log( "TimeJump - past self disappearing" );
			// This is one of your past selves disappearing back into the past
			mapFuture[x,y].type = eCellType.empty;
			mapFuture[x,y].playerTime = -1;
			audio.PlayOneShot(sfxTimeJump);
			return false;
		}
	
		if (mapStart[x,y].type != eCellType.empty)
		{
			Debug.Log( "TimeJump - cell occupied" );
			// Something is lurking at the beginning of time in the square you want to occupy. Die unexpectedly, or just fail?
			audio.PlayOneShot(sfxInvalid);
			return false;	// Fail for now
		}
			
		mapStart[x,y].type 			= map[x,y].type;
		mapStart[x,y].playerTime 	= map[x,y].playerTime;
		globalTime					= -1;
		
		CopyMap( mapStart, map );
		CopyMap( mapStart, mapPast );
		ZeroMapTimes( mapPast );
		CopyMap( mapStart, mapFuture );
		
		audio.PlayOneShot(sfxTimeJump);
		return true;
	}
	
	bool Move( int fromx, int fromy, int tox, int toy )
	{
		if (mapFuture[fromx,fromy].type == eCellType.player &&
			mapFuture[tox,toy].type == eCellType.exit &&
			hasKey)
		{
			Debug.Log("ESCAPED!");
			audio.PlayOneShot(sfxWin);
			if (lastLevel)
				gameState = eGameState.finished;
			else
				gameState = eGameState.win;
		}
		if (mapFuture[tox,toy].type == eCellType.empty ||
			(mapFuture[tox,toy].type == eCellType.exit && hasKey) ||
			(mapFuture[tox,toy].type == eCellType.key && mapFuture[fromx,fromy].type == eCellType.player) )
		{			
			if (mapFuture[tox,toy].type == eCellType.key)
			{
				audio.PlayOneShot(sfxKey);
				hasKey = true;
			}
				
			// Move cell contents, whatever they are (could be player or block)
			mapFuture[tox,toy].type 		= map[fromx,fromy].type;
			mapFuture[tox,toy].playerTime 	= map[fromx,fromy].playerTime;
			
			// Leave previous cell empty
			mapFuture[fromx,fromy].type 		= eCellType.empty;
			mapFuture[fromx,fromy].playerTime 	= -1;
			
			mapPast[fromx,fromy].playerTime		= (((tox-fromx)+2)<<4) + ((toy-fromy)+2);	// encode move direction
			Debug.Log("Encoded move: x=" + (tox-fromx) + " y=" + (toy-fromy) + " as " + mapPast[fromx,fromy].playerTime);
			
			return true;
		}
		if (mapFuture[fromx,fromy].type == eCellType.player && mapFuture[tox,toy].type == eCellType.block)
		{
			Debug.Log("PUSH");
			if (Move(tox,toy,tox+tox-fromx,toy+toy-fromy)) // Try to move the block!
			{
				audio.PlayOneShot(sfxPush);
				
				// Move cell contents, whatever they are (could be player or block)
				mapFuture[tox,toy].type 		= map[fromx,fromy].type;
				mapFuture[tox,toy].playerTime 	= map[fromx,fromy].playerTime;
				
				// Leave previous cell empty
				mapFuture[fromx,fromy].type 		= eCellType.empty;
				mapFuture[fromx,fromy].playerTime 	= -1;

				mapPast[fromx,fromy].playerTime		= (((tox-fromx)+2)<<4) + ((toy-fromy)+2);	// encode move direction
						
				return true;
			}
		}
		
		Debug.Log("BLOCKED");
		mapFuture[fromx,fromy].playerTime 	= map[fromx,fromy].playerTime;
		return false;
	}
	
	void ProcessTurn()
	{
		CopyMap( map, mapFuture );	// Create future state identical to current state
		CopyMap( map, mapPast );	// Create copy of current state for next frame to compare to
		ZeroMapTimes( mapPast );
	
		for (int x=0; x<gridSize; x++)
		{
			for (int y=0; y<gridSize; y++)
			{
				if (map[x,y].type == eCellType.player)
				{
					Debug.Log( "Player at " + x + ", " + y + " at her time " + map[x,y].playerTime + " doing: " + actionHistory[map[x,y].playerTime] );
					TakeAction( x, y, actionHistory[map[x,y].playerTime] );
				}
			}
		}
		
		CopyMap( mapFuture, map );	// Copy future state back into current state ready for next turn
	
		playerTime++;
		globalTime++;
		if (globalTime >= 10 && gameState == eGameState.play)
		{
			audio.PlayOneShot(sfxLose);
			gameState = eGameState.lose;
		}
		
	}
	
	void OnGUI()
	{	
		switch( gameState )
		{
		case eGameState.title:
			GUI.Box( new Rect(0,100,Screen.width,100), "C H R O N I C   H Y S T E R E S I S", style );
			GUI.Box( new Rect(0,170,Screen.width,100), "By Chris Payne for Ludum Dare 27", style );
			GUI.Box( new Rect(0,500,Screen.width,100), "Use cursor keys, and T to time warp back to start", style );
			if (GUI.Button(new Rect(0,300,Screen.width,100), "Press Space/Return to Play", style) || Input.GetButtonDown("Jump"))
			{
				audio.PlayOneShot(sfxAccept);
				gameState = eGameState.play;
				level = startLevel;
				lastLevel = false;
				InitLevel();
			}
			break;
		
		case eGameState.win:
			DrawGame();
			GUI.Box( new Rect(0,200,Screen.width,100), "Escaped!", style );
			if (GUI.Button(new Rect(0,300,Screen.width,100), "Press Space/Return to Continue", style) || Input.GetButtonDown("Jump"))
			{
				audio.PlayOneShot(sfxAccept);
				gameState = eGameState.play;
				level++;
				InitLevel();
			}
			break;
			
		case eGameState.lose:
			DrawGame();
			GUI.Box( new Rect(0,200,Screen.width,100), "Erased from existence!", style );
			if (GUI.Button(new Rect(0,300,Screen.width,100), "Press Space/Return to Retry", style) || Input.GetButtonDown("Jump"))
			{
				audio.PlayOneShot(sfxAccept);
				gameState = eGameState.play;
				InitLevel();
			}
			break;
		
		case eGameState.finished:
			DrawGame();
			GUI.Box( new Rect(0,200,Screen.width,100), "Chronic Hysteresis Escaped!", style );
			GUI.Box( new Rect(0,250,Screen.width,100), "You have mastered creative temporal physics. Use your power wisely.", style );
			if (GUI.Button(new Rect(0,400,Screen.width,100), "Press Space/Return to Replay", style) || Input.GetButtonDown("Jump"))
			{
				audio.PlayOneShot(sfxAccept);
				gameState = eGameState.title;
			}
			break;			
			
		default:
			DrawGame();
			//GUI.Box( new Rect(Screen.width-120, 20, 100, 50), "Time left = " + (10-globalTime) );
			//GUI.Box( new Rect(20, 20, 200, 70), "playerTime = " + playerTime + "\nglobalTime = " + globalTime + "\n\nturnProgress = " + turnProgress);
			break;
			
		}
	}
	
	void DrawGame()
	{
		float originx = (Screen.width - cellSize * gridSize) * 0.5f;
		float originy = (Screen.height - cellSize * gridSize) * 0.5f;
		
		int expand = 90;
		GUI.DrawTexture( new Rect(originx-expand, originy-expand, cellSize * gridSize + expand*2, cellSize * gridSize + expand*2), texBackground );		
				
		Texture tex = null;
		float randx = Random.Range( -1.0f, 1.0f );
		float randy = Random.Range( -1.0f, 1.0f );
	
		for (int x=0; x<gridSize; x++)
		{
			for (int y=0; y<gridSize; y++)
			{
				eCellType type = map[x,y].type;
				int xdir = 0;
				int ydir = 0;
				if (turnProgress >= 0.0f)
				{
					type = mapPast[x,y].type;
					if (mapPast[x,y].playerTime > 0)
					{
						xdir = ((mapPast[x,y].playerTime & ~0xf) >> 4)-2;
						ydir = (mapPast[x,y].playerTime & 0xF)-2;
					}
				}
				switch (type)
				{
				case eCellType.empty:	tex = null;
					break;
				case eCellType.wall:	tex = texWall;
					break;
				case eCellType.block:	tex = texBlock;
					break;
				case eCellType.key:		tex = texKey;
					break;
				case eCellType.player:	tex = texPlayer;
										if (map[x,y].playerTime < playerTime)
											tex = texPlayerPast;
										//GUI.Box( new Rect(x*cellSize+originx, y*cellSize+originy, 20, 20), "" + map[x,y].playerTime );
					break;
				case eCellType.exit:	tex = hasKey ? texExit : texExitLocked;
					break;
				}
				
				if (tex)
				{
					float animx = (float)xdir*cellSize*turnProgress;
					float animy = (float)ydir*cellSize*turnProgress;
					
					if (gameState != eGameState.win)
					{
						animx += randx * Mathf.Max(0.0f,(float)globalTime-6.0f);
						animy += randy * Mathf.Max(0.0f,(float)globalTime-6.0f);
					}
					
					GUI.DrawTexture( new Rect(x*cellSize+originx+animx, y*cellSize+originy+animy, cellSize, cellSize), tex );
				}
			}
		}
	}
}
