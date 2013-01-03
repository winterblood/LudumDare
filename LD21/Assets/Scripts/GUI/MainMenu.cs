using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
	public GUISkin skin;
	
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	public Texture buttonTexture;
	
    void OnGUI()
	{
		/*
        if (!buttonTexture) {
            Debug.LogError("Please assign a texture on the inspector");
            return;
        }
        if (GUI.Button(new Rect(10, 10, 150, 150), buttonTexture, skin.GetStyle("menu")))
		{
            Debug.Log("Clicked the button with an image");
		}
        */
		
		float buttonWidth = 100;
		float buttonHeight = 100;
		
        if (GUI.Button(new Rect(Screen.width*0.5f-buttonWidth*0.5f, Screen.height*0.9f-buttonHeight*0.5f, buttonWidth, buttonHeight), "Play", skin.GetStyle("menu")))
		{
            Application.LoadLevel( "zone1" );
		}
    }
}
