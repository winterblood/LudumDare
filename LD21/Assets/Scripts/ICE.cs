using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;


public class ICE : MonoBehaviour
{
	/*
	[DllImport("ICEBridge32.dll")]
	[DllImport("ICECore32.dll")]   
	
	// Use this for initialization
	void Start ()
	{
		//create bridge
		ICEBridgeLib.CoBridge bridge = new ICEBridgeLib.CoBridge();
		            
		
		//Set your game id here as given from your game project page
		// 48710711-e831-43fa-93ec-e85ca33bae53
		ICECoreLib.GameId myGameId = new ICECoreLib.GameId(); 
		myGameId.Data1 = 0x48710711;
		myGameId.Data2 = 0xe831;
		myGameId.Data3 = 0x43fa;
		myGameId.Data4 = new byte[] { 0x93, 0xec, 0xe8, 0x5c, 0xa3, 0x3b, 0xae, 0x53 };
		             
		string mySecret = "a9a51697-212d-4b25-a75f-941a2bd99ad1";
		
		//Initialise the bridge with the gameId and secret.
		//This allows the bridge to access users tokens registered on the computer 
		bridge.Initialise(myGameId, mySecret);
		
		//create a game session for the user playing the game              
		ICECoreLib.CoGameSession session = bridge.CreateDefaultGameSession();
		
		
		//Start the session 
		session.RequestStartSession();
		
		
		//block until session has started
		//This is just for simplicity. Real game would react to session start and end events. Session //may end for different reasons or may never start.
		bool started = false;
		do 
		{
			session.UpdateSession();
			started = session.IsSessionStarted();
		
		} while (!started);
		
		
		ICECoreLib.CoAccessControl accessControl = new ICECoreLib.CoAccessControl();
		                
		accessControl.Initialise(session);                
		ICECoreLib.LicenseState hasLicense = accessControl.LicenseState;
		         
		do 
		{
			//do stuff;	
			session.UpdateSession();
		} while (session.IsSessionStarted());

	}
	*/
	// Update is called once per frame
	void Update () {
	
	}
}
