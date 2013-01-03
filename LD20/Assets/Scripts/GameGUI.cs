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
	private GameObject playerObj;
//	private GameObject towerObj;
//	private TowerGenerator towerMap;

	// Use this for initialization
	void Start ()
	{
		guiTitle = GameObject.Find( "Title" );
		guiDeath = GameObject.Find( "Death" );
		guiWin 	 = GameObject.Find( "Win" );
		guiPause = GameObject.Find( "Pause" );

//		towerObj = GameObject.Find("TowerRoot");
//		towerMap = towerObj.GetComponent<TowerGenerator>();

		playerObj = GameObject.Find("First Person Controller");

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
			guiTitle.SetActiveRecursively(false);
			break;
		case GUIState.Death:
			guiDeath.SetActiveRecursively(false);
			break;
		case GUIState.Win:
			guiWin.SetActiveRecursively(false);
			break;
		case GUIState.Pause:
			guiPause.SetActiveRecursively(false);
			break;
		case GUIState.Gameplay:
		default:
			Debug.Log("EXITING GUI Gameplay/default");
			//towerObj.SetActiveRecursively(false);
			//playerObj.SetActiveRecursively(false);
			//guiTitle.SetActiveRecursively(false);
			guiDeath.SetActiveRecursively(false);
			guiWin.SetActiveRecursively(false);
			guiPause.SetActiveRecursively(false);
			Screen.lockCursor = false;
    		Screen.showCursor = true;
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
			guiTitle.SetActiveRecursively(true);
			playerObj.SetActiveRecursively(false);
			break;
		case GUIState.Death:
			guiDeath.SetActiveRecursively(true);
			break;
		case GUIState.Win:
			guiWin.SetActiveRecursively(true);
			break;
		case GUIState.Gameplay:
			Debug.Log("ENTERING GUI Gameplay");
			playerObj.SetActiveRecursively(true);
			//towerObj.SetActiveRecursively(true);
			Screen.lockCursor = true;
    		Screen.showCursor = false;
			break;
		default:
			break;
		}
	}
	
	public void OnGUI()
	{
		if (guiState != GUIState.Pause)
			return;
		
		if (GUI.Button ( new Rect (10,10,150,50), "Resume", styleDefault ))
		{
			SetState( GUIState.Gameplay );
		}
	}
}
