using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour
{
	
	public enum GUIState
	{
		Invalid,
		Gameplay,
		Title,
		Death,
		Win,
		Pause
	}

	private GUIState guiState = GUIState.Invalid;
	public GUIStyle styleDefault;

	private GameObject guiTitle;
	private GameObject guiDeath;
	private GameObject guiWin;
	private GameObject guiPause;
	private GameObject guiIngame;
	//private GameObject playerObj;
	private GUITexture crosshair;
	public bool invertY;

	private int goldCollected;
	private float litAmount;
	private float litAmountLastFrame;
	
	// Use this for initialization
	void Start ()
	{
		guiTitle = GameObject.Find( "Title" );
		guiDeath = GameObject.Find( "Death" );
		guiWin 	 = GameObject.Find( "Win" );
		guiPause = GameObject.Find( "Pause" );
		guiIngame= GameObject.Find( "Ingame" );
		
		Transform guiCrosshair = guiIngame.transform.FindChild( "crosshair" );
		if (guiCrosshair)
			crosshair = guiCrosshair.gameObject.guiTexture;
		else
			print( "No crosshair!" );

		//playerObj = GameObject.Find("First Person Controller");
		goldCollected = 0;
		litAmount = 0.0f;
		litAmountLastFrame = 0.0f;
		
		invertY = true;
		Debug.Log("GameGUI.Start");
		SetState( GUIState.Title );
	}

	// Update is called once per frame
	void Update ()
	{
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
		case GUIState.Title:
			if (Input.GetMouseButtonDown(0))
				SetState( GUIState.Gameplay );
			break;
		case GUIState.Death:
			break;
		case GUIState.Win:
			break;
		case GUIState.Pause:
			break;
		case GUIState.Gameplay:
			litAmountLastFrame = litAmount;
			litAmount = 0.0f;	// reset ready to accumulate more light
			if (crosshair)
			{
				Color col = crosshair.color;
				col.a = 0.25f + litAmount * 0.75f;
				crosshair.color = col;
			}
			break;
		default:
			break;
		}
	}

	public bool IsActive()
	{
		return guiState != GUIState.Gameplay;
	}
	
	public void SetState( GUIState state )
	{
		//Debug.Log("GameGUI.SetState");
		
		if (state == guiState)
			return;

		//
		// Exit old state
		//
		switch (guiState)
		{
		case GUIState.Title:
			//Debug.Log("EXITING GUI Title");
			guiTitle.SetActive(false);
			//playerObj.SetActive(true);
			break;
		case GUIState.Death:
			guiDeath.SetActive(false);
			break;
		case GUIState.Win:
			guiWin.SetActive(false);
			break;
		case GUIState.Pause:
			guiPause.SetActive(false);
			break;
		case GUIState.Gameplay:
		default:
			Debug.Log("EXITING GUI Gameplay/default");
			guiDeath.SetActive(false);
			guiWin.SetActive(false);
			guiPause.SetActive(false);
			guiIngame.SetActive(false);
			//playerObj.SetActive(false);
			Screen.lockCursor = false;
    		Screen.showCursor = true;
			Time.timeScale = 0.0f;
			break;
		}

		//
		// Enter new state
		//
		guiState = state;
		switch (guiState)
		{
		case GUIState.Title:
			Debug.Log("ENTERING GUI Title");
			guiTitle.SetActive(true);
			//playerObj.SetActive(false);
			break;
		case GUIState.Pause:
			guiPause.SetActive(true);
			break;	
		case GUIState.Death:
			guiDeath.SetActive(true);
			break;
		case GUIState.Win:
			guiWin.SetActive(true);
			break;
		case GUIState.Gameplay:
			Debug.Log("ENTERING GUI Gameplay");
			//playerObj.SetActive(true);
			guiIngame.SetActive(true);
			Screen.lockCursor = true;
    		Screen.showCursor = false;
			Time.timeScale = 1.0f;
			break;
		default:
			break;
		}
	}
	
	public void OnGUI()
	{
		switch (guiState)
		{
		case GUIState.Pause:
			{
				if (GUI.Button ( new Rect (435,300,150,30), "Resume", styleDefault ))
				{
					SetState( GUIState.Gameplay );
				}
				string text = "Invert Y: " + (invertY?"on":"off");
				if (GUI.Button ( new Rect (435,330,150,30), text, styleDefault ))
				{
					invertY = !invertY;
				}
				if (GUI.Button ( new Rect (435,360,150,30), "Restart", styleDefault ))
				{
					Application.LoadLevel ( 0 );
				}
				
			}
			break;
		case GUIState.Death:
		case GUIState.Win:
			if (GUI.Button ( new Rect (435,380,150,30), "Restart", styleDefault ))
			{
				Application.LoadLevel ( 0 );
			}
			break;
		case GUIState.Gameplay:
			{
				string text = goldCollected + "/1000 gold  " + (int)(litAmountLastFrame*100.0f) + "% brightness";
				GUI.Box( new Rect(20,500, 80,40), text, styleDefault );
			}
			break;
		default:
			break;
		}
			
	}
	
	public void AddGold( int gold )
	{
		goldCollected += gold;
		
		if (goldCollected >= 1000)
			SetState( GUIState.Win );
	}
	
	public void AddLight( float light )
	{
		litAmount += light;
		if (litAmount > 1.0f)
			litAmount = 1.0f;
	}
	
	public float GetPlayerLight()
	{
		return litAmountLastFrame;
	}
}
