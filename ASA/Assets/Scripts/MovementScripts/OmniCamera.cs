using UnityEngine;
using System.Collections;

public class OmniCamera : MonoBehaviour {
	
	// This is the main camera script.
	// It's designed to emulate the manner in which one would navigate in the Unity3D editor
	// But for use at run time to be able to see large areas.
	
	public Vector2 compassPos = new Vector2(900,700);	// The point on the screen (1024x768) that the three line handles
														// Will draw.
	
	public LayerMask terrain;							// Terrain layer information
	float dollySpeed = 250.0f;							// The speed in which the camera will pan when in pan mode.
	float rotationSpeed = 180.0f;						// The rotation speed in orbit mode
	float scrollSpeed = 100.0f;							// The zoom speed in scroll mode.
	
	private GameObject selection;						// The selection of the camera, for focusing on an object.
	
	
	private Vector3 focusPoint = Vector3.zero;			// The point from which the camera orbits around.
	private float focusDist = 0.0f;						// The distance we're orbiting from

	
	private Vector3 crossSectionStart = Vector3.zero;	// The start point of a line that defines a custom cross section
	private Vector3 crossSectionEnd = Vector3.zero;		// The end point of a line that defines a custom cross section
	
	private bool recordCrossSectionLine = false;		// Are we recording a cross section?
	private Vector3 compassPt = Vector3.zero;			// The point in world space that the compass/handles are drawing.
	static Material lineMaterial;
	
	
	
	static void CreateLineMaterial() {
    if( !lineMaterial ) {
        lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
	            "SubShader { Pass { " +
	            "    Blend SrcAlpha OneMinusSrcAlpha " +
	            "    ZWrite Off Cull Off Fog { Mode Off } " +
	            "    BindChannels {" +
	            "      Bind \"vertex\", vertex Bind \"color\", color }" +
	            "} } }" );
	        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
	        lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
	    }
	}
	
	
	IEnumerator Start () {
		
		yield return 0;
		yield return 0;
		yield return 0;
		
		// Set the focus point on startup.
		focusPoint = GeographicCoords.MaxCoords * 0.5f;
	
	}
	
	void OnPostRender() {
		
		GL.PushMatrix();
		
		CreateLineMaterial();
		lineMaterial.SetPass( 0 );
		GL.Begin(GL.LINES);
		GL.Color(Color.green);
		
		// Draw a line in the world to represent the cross section line.
		// To let the user know where they defined the line.
		GL.Vertex3(crossSectionStart.x,crossSectionStart.y,crossSectionStart.z);
		GL.Vertex3(crossSectionEnd.x,crossSectionEnd.y,crossSectionEnd.z);
		
		
		
		GL.End();
		
		
		
		GL.PopMatrix();
		
		GL.PushMatrix();
		GL.Begin(GL.LINES);
		
		// Draw compass handles.  We scale them to similar proportions as the rest of the UI
		// even though these handles are not drawn on the GUI layer, but drawn instead in the world, a short distance from the camera.
		
		// First draw the red right-hand line, which corresponds to +X.
		GL.Color(Color.red);
		// Define the compass point in the world.
		compassPt = camera.ScreenToWorldPoint(new Vector3(compassPos.x * (Screen.width/UIScalingCS.width),Screen.height-(compassPos.y * (Screen.height/UIScalingCS.height)),1.0f));
		// Draw the red right hand line
		GL.Vertex3(compassPt.x,compassPt.y,compassPt.z);
		Vector3 rgt = compassPt + Vector3.right *(Screen.width/UIScalingCS.width) * 0.2f;
		GL.Vertex3(rgt.x,rgt.y,rgt.z);
		
		// Draw the blue front line, corresponding to +Z
		GL.Color(Color.blue);
		GL.Vertex3(compassPt.x,compassPt.y,compassPt.z);
		Vector3 fwd = compassPt + Vector3.forward* (Screen.width/UIScalingCS.width) * 0.2f;
		GL.Vertex3(fwd.x,fwd.y,fwd.z);
		
		
		// Draw the green Up line, corresponding to +Y
		GL.Color(Color.green);
		GL.Vertex3(compassPt.x,compassPt.y,compassPt.z);
		Vector3 upward = compassPt + Vector3.up* (Screen.width/UIScalingCS.width) * 0.2f;
		GL.Vertex3(upward.x,upward.y,upward.z);
		
		GL.End();
		
		GL.PopMatrix();
		
	}
	
	// Update is called once per frame
	void Update () {
		
		// If T is pressed, go directly to the spill.
		if(Input.GetKeyDown("t"))
		{
			selection = GameObject.FindWithTag("Spill");
			StartCoroutine(CenterObj());
			
		}
		
		// In the future, if there are other objects that can be selected
		// Pressing the "f" key will focus on that object.
		if(Input.GetKeyDown("f") && selection != null)
		{
			focusPoint = selection.transform.position;
			StartCoroutine("CenterObj");
		}
		
		float horz = Input.GetAxis("Horizontal");
		float vert = Input.GetAxis("Vertical");
		Vector3 focusPos = transform.position + (transform.forward*focusDist);
		// Holding down the left alt key enters navigation mode.
		if(Input.GetKey("left alt"))
		{
			// Set the focus distance if it's negative.
			if(focusDist < 0.0f)
			{
				focusDist *= -1.0f;
			}
			// Set up the focus position for the camera.
			
			
			// Record relevant mouse deltas.  The X, Y, and scroll wheel deltas.
			float mouseX = Input.GetAxis("Mouse X");
			float mouseY = Input.GetAxis("Mouse Y");
			float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
			
			
			
			// Holding left click in navigation mode causes the camera to orbit
			if(Input.GetMouseButton(0) || (horz != 0.0f || vert != 0.0f))
			{
				Vector3 myRot = transform.eulerAngles;
				if(!Input.GetMouseButton(0))
				{
					myRot.x -= vert * rotationSpeed * Time.deltaTime;
					myRot.y += horz * rotationSpeed * Time.deltaTime;
				}
				else
				{
					myRot.x -= mouseY * rotationSpeed * Time.deltaTime;
					myRot.y += mouseX * rotationSpeed * Time.deltaTime;
				}

				transform.rotation = Quaternion.Euler(myRot);
				//AdjustLoS(focusPos);
				transform.position = focusPos - (transform.forward*focusDist);
			}
			
			// Holding the middle mouse button causes the camera to pan.
			else if(Input.GetMouseButton(2))
			{
				
				Vector3 myPos = transform.position;
				
				myPos -= (transform.right * dollySpeed * mouseX * Time.deltaTime) + (transform.up * dollySpeed*mouseY*Time.deltaTime);
				
				transform.position = myPos;
				//AdjustLoS(focusPos);
			}
			// Scrolling with the mouse wheel, or holding the right mouse button causes the camera to zoom.
			else if(scrollWheel != 0.0f || Input.GetMouseButton(1))
			{
				
			
				//focusDist -= scrollSpeed * scrollWheel * Time.deltaTime;
				transform.position += scrollSpeed * scrollWheel * Time.deltaTime*transform.forward;
				if(Input.GetMouseButton(1))
				{
					//focusDist -= scrollSpeed * Time.deltaTime * (mouseX + mouseY);
					transform.position -= scrollSpeed * (mouseX + mouseY) * Time.deltaTime*transform.forward;
				}
				focusDist = Vector3.Distance(transform.position,focusPos);
				//transform.position = focusPos - (transform.forward*focusDist);
				//AdjustLoS(focusPos);
			}
			AdjustLoS(focusPos);
		}
		else if(Input.GetKey("z"))
		{
			// Alternate zoom scheme
			//focusDist -= scrollSpeed * vert * Time.deltaTime;
			transform.position += scrollSpeed * vert * Time.deltaTime*transform.forward;
			//transform.position = focusPos - (transform.forward*focusDist);
			
		}
		else if(Input.GetKey("x"))
		{
			transform.position += (transform.right*horz*dollySpeed*Time.deltaTime)+(transform.up*dollySpeed*vert*Time.deltaTime);
		}
		// Holding left control allows the user to create custom cross sections
		// When the left mouse button is first clicked down, record that position as the starting point
		else if(Input.GetKey("left ctrl") && Input.GetMouseButtonDown(0))
		{
			// Enter cross section recording mode
			recordCrossSectionLine = true;
			// Figure out where in world space we clicked and set that point as the start point of our cross section line
			Vector3 mid = Vector3.zero;
			RaycastHit terrainHit;
			if(Physics.Raycast(transform.position,transform.forward,out terrainHit, terrain))
			{
				mid = (transform.forward*terrainHit.distance);
			}
			crossSectionStart = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,mid.magnitude));
			
			
			
		}
		
		if(recordCrossSectionLine && Input.GetMouseButton(0))
		{
			// If we're recording and we've still got the button held down, then keep defining the end point
			// basically until we let the button go.
			Vector3 mid = Vector3.zero;
			RaycastHit terrainHit;
			if(Physics.Raycast(transform.position,transform.forward,out terrainHit, terrain))
			{
				mid = (transform.forward*terrainHit.distance);
			}
			
			crossSectionEnd = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,mid.magnitude));
			
		}
		else if(recordCrossSectionLine && Input.GetMouseButtonUp(0))
		{
			// When we release the button, now we do a few calculations.
			recordCrossSectionLine = false;
			Vector3 mid = Vector3.zero;
			
			// Define the vector between these two points
			Vector3 crossSectionVec = crossSectionEnd - crossSectionStart;
			
			
			
			// Get the average of these two points.
			mid = crossSectionStart + crossSectionEnd;
			mid *= 0.5f;
			mid = new Vector3(mid.x,0.0f,mid.z);// The average of the two points, with the y at sea level
			
			GameObject orthoCam = GameObject.FindWithTag("OrthoCam");
			// Set the orthographic plane for the ortho cam.
			orthoCam.GetComponent<ContextCamera>().SetPlane((crossSectionVec),transform.forward,mid);
		}
	
	}
	
	void AdjustLoS(Vector3 focusPos)
	{
		//transform.position = focusPos - (transform.forward*focusDist);
		
		RaycastHit losHit;
		//Debug.DrawLine(transform.position,focusPos);
		if(Physics.Linecast(focusPos,transform.position,out losHit,terrain))
		{
			
			transform.position = losHit.point;
			focusDist = losHit.distance;
		}
		
	}
	
	// Set up snapping to an object.
	
	IEnumerator CenterObj()
	{
		// Set our focus distance to the magnitude of the bounding box of the object's renderer.
		focusDist = selection.renderer.bounds.size.magnitude;
		if(focusDist <= 1.0)
		{
			// If this distance is less than 1 (very possible if the object is a particle renderer, since the bounding box is defined by the placement of its particles)
			// make it 10.
			focusDist = 10.0f;
			
		}
		// Set the focusing point to the object.
		focusPoint = selection.transform.position;
		// Set our target position to this specfic point in space while maintaining the camera's rotation
		Vector3 targetPosition = focusPoint - (transform.forward*focusDist);
		
		while(Vector3.Distance(transform.position,targetPosition) > 0.1)
		{
			// If we decide to orbit the camera during the centering, update our target position
			targetPosition = focusPoint - (transform.forward*focusDist);
			// Move toward the target point at the same speed we pant the camera.
			transform.position = Vector3.MoveTowards(transform.position,targetPosition,dollySpeed*Time.deltaTime);
			yield return 0;
		}
	}
	
}
