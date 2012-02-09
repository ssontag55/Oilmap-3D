using UnityEngine;
using System.Collections;

public class StartMenu : MonoBehaviour {

	
	
	void OnGUI()
	{
		UIScalingCS.ScaleUI();
		
		GUI.Box(new Rect(400,200,200,50),"ASA 3D Viewer Beta");
		
		if(GUI.Button(new Rect(300,384,200,20),"Load an Oilmap"))
		{
			// Load the scene that contains the Oilmap loader.
			Application.LoadLevel(1);
		}
		if(GUI.Button(new Rect(500,384,200,20),"Load a SIMAP"))
		{
			// Load the scene that contains the SIMAP loader
			Application.LoadLevel(2);
		}
		
	}
	
	
}
