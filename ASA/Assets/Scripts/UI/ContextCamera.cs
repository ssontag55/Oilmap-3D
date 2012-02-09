using UnityEngine;
using System.Collections;

public class ContextCamera : MonoBehaviour {
	
	public GameObject waterSurface;	// A reference to the water surface.  Set when the terrain is created.
	

	private float dist;		// The distance from the focal point the orthographic camera should be.

	private Rect winRect = new Rect(708,0,316,361);	// Default rectangle for the map window.
	private Rect texRect;

	private bool showTrackLine = true;				// Should we show the track line on the map window?
	private string viewString = "CROSS SECTION";	// Title bar for the window
	
	private bool minimizedMap = true;				// Is the map window collapsed?
	private bool viewCtrls = false;					// Are we viewing the controls for the software?
	private Vector3 mainPos = Vector3.zero;			// The focal point of the camera.
	private Trackline lineRef;						// A reference to the OpenGL based trackline, to toggle it on and off.
	// Use this for initialization
	IEnumerator Start () {
		
		// Wait three frames after there's an actual spill defined before continuing.
		while(GameObject.FindWithTag("Spill") == null)
		{
			yield return 0;
		}
		yield return 0;
		yield return 0;
		yield return 0;
		// Set our focal point to the spill, but the point at sea-level of the spill
		mainPos = GameObject.FindWithTag("Spill").transform.position;
		mainPos.y = 0.0f;
		
		dist = 5.0f;
		
		// Set the size of the orthographic view to either 25, or the magnitude of the bounding box's size of the spill.  Whichever's biggest.
		GetComponent<Camera>().orthographicSize = Mathf.Max(GameObject.FindWithTag("Spill").renderer.bounds.size.magnitude,25.0f);
		// Set the trackline reference.
		lineRef = GetComponent<Trackline>();
	}
	// Update is called once per frame
	void Update () {
		// Set the proper heading and distance from the focal point
		transform.position = mainPos - (transform.forward*dist);
		if(lineRef != null)
			lineRef.enabled = showTrackLine;
		
		
		if(Input.GetKeyDown("2"))
			minimizedMap = !minimizedMap;
		if(Input.GetKeyDown("3"))
			showTrackLine = !showTrackLine;
		if(Input.GetKeyDown("4"))
		{
			if(waterSurface != null)
				waterSurface.active = !waterSurface.active;
		}
	}
	
	
	// Set the plane of this camera.
	// Basically, when a user builds a cross section,
	// we define the rotation and size of the camera's view.
	public void SetPlane(Vector3 crossSectionLine,Vector3 fwdVec, Vector3 position)
	{
		// Set the position of the camera.
		transform.position = position;
		
		//Get a perpendicular vector from the fwdVec and the crossSection line
		Vector3 perpVec = Vector3.Cross(crossSectionLine,fwdVec);
		//Vector3 perpVec = Vector3.Reflect(crossSectionLine,fwdVec);
		//perpVec = new Vector3(-perpVec.z,0.0f,perpVec.x);
		perpVec = new Vector3(perpVec.x,0.0f,perpVec.z);	// Remove the y-component of this vector
		transform.rotation = Quaternion.LookRotation(perpVec.normalized);//And normalize it.
		// Set the size of the orthographic view to be half the magnitude of the cross section line.
		// The orthographicSize variable is actually 50% of what the total viewing length is.
		GetComponent<Camera>().orthographicSize = crossSectionLine.magnitude*0.5f;
		// Set the far clip plane
		GetComponent<Camera>().farClipPlane = 5000.0f;
		// Set the camera distance based on the orthographic size.
		dist = GetComponent<Camera>().orthographicSize;
		// Set up the focal point again
		mainPos = transform.position + (transform.forward*dist);
		// Explicitly open the map window.
		minimizedMap = false;
	}
	
	void OnGUI () {
		// Draw nothing if we have no spill.
		if(GameObject.FindWithTag("Spill") == null)
			return;
		
		UIScalingCS.ScaleUI();
		
		// If we're not viewing the softare's controls, then display the different UI elements.
		if(!viewCtrls)
		{
			// Present a button for viewing the controls if desired.
			viewCtrls = GUI.Button(new Rect(0,320,150,30), "View Controls");
			if(minimizedMap)
			{
				// If the map's minimized, set up a button to show the window.
				minimizedMap = !GUI.Button(new Rect(winRect.x,winRect.y,winRect.width,30), "Show Window");
			}
			else
			{
				// Otherwise, draw the window.
				winRect = GUI.Window(0,winRect,MinimapWindow,viewString);
				winRect = new Rect(Mathf.Clamp(winRect.x,0,1024-winRect.width),Mathf.Clamp(winRect.y,0,768-winRect.height),winRect.width,winRect.height);
			}
			// Display the scale of the world for user reference.
			GUI.Box(new Rect(300,0,300,30),"World Scale: X: " + GeographicCoords.Scaling() + " Y: " + GeographicCoords.VerticalScaling() + " Z: " + GeographicCoords.Scaling());
			
			if(GUI.Button(new Rect(0,0,300,30), "Load New Scenario"))
			{
				// If this button is clicked, then we reload this scene.
				// It's much easier to clean up everything if we just completely reload the scene.
				Application.LoadLevel(0);
			}
		}
		else
		{
			// If we're viewing the controls then present a box with control information in it.
			GUI.Box(new Rect(20,32,980,384),"");
			viewCtrls = !GUI.Button(new Rect(805,33,150,30), "Close");
			GUI.Box(new Rect(30,64,320,320), "Navigation: \nLeft Alt+LMB: Orbit Camera\nLeft Alt+MMB: Pan Camera\nLeft Alt+RMB or Scroll Wheel:  Zoom Camera\nT: Zoom to Oil Spill\n Alternate Navigation:\nZ+Arrow Keys: Zoom Camera\nX+Arrow Keys: Pan Camera\nLeft Alt+Arrow Keys: Orbit Camera");
			GUI.Box(new Rect(350,64,320,320), "Orthographig View:\nLeft Control + LMB down: Define Cross Section line\n Left Control+LMB Up: Set Cross Section line.");
			GUI.Box(new Rect(670,64,320,320), "Other Keybinds:\n`: Toggle Spill Playback On/Off\n1: Toggle Terrain Depth Color Coding\n2:Toggle Cross Section View\n3: Toggle track line visibility in cross section\n4: Toggle Water Surface Visibility");
		}
			
			
		
	
	}
	
	
	// Draws the minimap window
	void MinimapWindow(int windowID)
	{
		// Create a box inside the window
		//GUI.Box(new Rect(30,45,256,256),"");
		
		
		
		// Draw the render texture of this orthographic camera.
		GUI.DrawTexture(new Rect(30,45,256,256),GetComponent<Camera>().targetTexture);
		
		// View rotation buttons
		if(GUI.Button(new Rect(30,15,256,30), "^"))
		{
			// Top view
			transform.eulerAngles = new Vector3(90,0,0);
			
		}
		
		if(GUI.Button(new Rect(30,301,256,30), "F"))
		{
			// Front view (facing toward +Z)
			transform.eulerAngles = new Vector3(0,0,0);
			
		}
		
		if(GUI.Button(new Rect(286,45,30,256), ">"))
		{
			// Rotate -90 degrees
			transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,0);
			transform.Rotate(0f,-90.0f,0f);
			
		}
		
		if(GUI.Button(new Rect(0,45,30,256), "<"))
		{
			// rotate 90 degrees.
			transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,0);
		
			transform.Rotate(0f,90.0f,0f);
			
		}
		if(GUI.Button(new Rect(0,15,30,30), "ISO"))
		{
			// Isometric view
			transform.eulerAngles = new Vector3(30,45,0);
			
		}
		
		// Clicking this should minimize the window.
		minimizedMap = GUI.Button(new Rect(286,15,30,30), "X");
		
		// Toggle to enable or disable trackline
		showTrackLine = GUI.Toggle(new Rect(5,331,120,12),showTrackLine,"Show Trackline");
		// Toggle to enable or disable the water surface if it exists.
		if(waterSurface != null)
			waterSurface.active = GUI.Toggle(new Rect(125,331,140,12),waterSurface.active,"Show Water Surface");
		GUI.DragWindow();
	}
	
}
