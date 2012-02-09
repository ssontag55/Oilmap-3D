using UnityEngine;
using System.Collections;

public class UIScalingCS
{
	
	// Simple class for setting up the resolution of the UI to be completely resolution
	// independent.  The UI will scale to be the same proportions regardless of screen size.
	// The basis we use for this is 1024x768 and all UI controls should be placed inside that screen space.
	public static float width = 1024.0f;
	public static float height = 768.0f;
	
	private static GUISkin theme;	// The theme of the UI
	
	public static void ScaleUI()
	{
		if(theme == null)
			theme = (Resources.Load("UITheme",typeof(GUISkin)) as GUISkin);	// Load up the UI theme if it doesn't already exist
		
		GUI.skin = theme;	// Set the UI theme
		
		// Scale the GUI drawing matrix.
		GUI.matrix = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,new Vector3(Screen.width/width, Screen.height/height,1));
	}
}
