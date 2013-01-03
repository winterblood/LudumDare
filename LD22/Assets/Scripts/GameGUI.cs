using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour
{
	public enum GUIState
	{
		Invalid,
		Menu,
		Gameplay,
		Pause
	};
	
	public GUISkin skin;
	public float windowHeight = 128.0f;
	
	private GUIState guiState = GUIState.Invalid;
	private string showString;
	private float showTimer = -1.0f;
	private float showYPos = -200.0f;
	private float showYTarget = -200.0f;
	private bool showTruncated = false;
	private Rect showRect = new Rect(0.0f, 0.0f, 0.0f, 0.0f);
	private GameObject[] inventorySlot;
		
	// Use this for initialization
	void Start ()
	{
		inventorySlot = new GameObject[10];
		SetState( GUIState.Menu );
		
		Terrain.activeTerrain.terrainData.wavingGrassTint = new Color( 0.7f, 0.7f, 0.7f, 1.0f );
	}
	
	// Update is called once per frame
	void Update ()
	{
		float dt = Time.deltaTime;
			
		if (Input.GetKeyDown ( KeyCode.Escape ))
		{
			if (guiState == GUIState.Pause)
			{
				SetState( GUIState.Gameplay );
			}
			else if (guiState == GUIState.Gameplay)
			{
				SetState( GUIState.Pause );
			}
		}
		
		switch (guiState)
		{
//		case GUIState.Title:
//			if (Input.GetMouseButtonDown(0))
//				SetState( GUIState.Gameplay );
//			break;
		case GUIState.Gameplay:
			Time.maximumDeltaTime = 0.1f;
			break;
		case GUIState.Pause:
			Time.maximumDeltaTime = 0.0f;
			dt = 0.0f;
			break;
		default:
			break;
		}
		
		if (showTimer > 0.0f)
		{
			showYTarget = 20.0f;
			showTimer -= dt;
		}
		else
		{
			showYTarget = -windowHeight;	// Back offscreen
		}
		
		showYPos = Mathf.MoveTowards( showYPos, showYTarget, 200.0f*dt*2.0f);
	}
	
	public void SetState( GUIState state )
	{
		if (state == guiState)
			return;

		//
		// Exit old state
		//
		switch (guiState)
		{
		case GUIState.Menu:
			break;
		case GUIState.Pause:
			break;
		case GUIState.Gameplay:
		default:
			Debug.Log("EXITING GUI Gameplay/default");
//			Screen.lockCursor = false;
//   		Screen.showCursor = true;
			break;
		}

		//
		// Enter new state
		//
		guiState = state;
		switch (guiState)
		{
		case GUIState.Menu:
			Debug.Log("ENTERING Menu");
			break;
		case GUIState.Pause:
			Debug.Log("ENTERING Pause");
			break;
		case GUIState.Gameplay:
			Debug.Log("ENTERING GUI Gameplay");
//			Screen.lockCursor = true;
//    		Screen.showCursor = false;
			break;
		default:
			break;
		}
	}
	
	public void Print( string str, float time )
	{
		showString = str;
		showTimer = time;
		showTruncated = str.Length < 20;
	}
	
	public bool IsMessageShown()
	{
		return (showTimer >= 0.0f);
	}
		
	public void AddToInventory( GameObject obj )
	{
		for (int i=0; i<10; i++)
		{
			if (inventorySlot[i] == null)
			{
				inventorySlot[i] = obj;
				obj.SetActiveRecursively(false);
				return;
			}
		}
		Debug.Break();
	}
	
	public void RemoveFromInventory( GameObject obj )
	{
		for (int i=0; i<10; i++)
		{
			if (inventorySlot[i] == obj)
			{
				inventorySlot[i] = null;
				return;
			}
		}
		Debug.Break();
	}
	
	public bool	IsCarrying( GameObject obj )
	{
		for (int i=0; i<10; i++)
		{
			if (obj == inventorySlot[i])
				return true;
		}
		return false;
	}
	
	public void OnGUI()
	{
		GUI.skin = skin;
		
		if (showYPos > 1.0f-windowHeight)
		{
			if (showTruncated)
			{
				showRect.x = Screen.width*0.5f-150.0f;
				showRect.y = showYPos;
				showRect.width = 300.0f;
				showRect.height = windowHeight;
			}
			else
			{
				showRect.x = 100.0f;
				showRect.y = showYPos;
				showRect.width = Screen.width-200.0f;
				showRect.height = windowHeight;
			}

			GUI.TextArea( showRect, showString );
		}
		
		if (guiState == GUIState.Pause)
		{
			if (GUI.Button ( new Rect (100,100,Screen.width-200.0f,Screen.height-200.0f), "Resume" ))
			{
				SetState( GUIState.Gameplay );
			}
		}
		else if (guiState == GUIState.Menu)
		{
			if (GUI.Button ( new Rect (100,100,Screen.width-200.0f,Screen.height-200.0f), "Play" ))
			{
				SetState( GUIState.Gameplay );
			}
		}
		
		// Inventory
		Rect r = new Rect (10,200,200,32);
		for (int i=0; i<10; i++)
		{
			if (inventorySlot[i] != null)
			{
				GUI.Button ( r, inventorySlot[i].name );
				r.y += 34;
			}
		}
	}
}
