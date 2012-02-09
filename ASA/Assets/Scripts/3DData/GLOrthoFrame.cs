using UnityEngine;
using System.Collections;

public class GLOrthoFrame : MonoBehaviour {
	
	// This is a simple script that draws a wireframe representation
	// of where the orthographic camera is, and where it's facing
	// For the purpose of understanding what a custom cross section is capturing.
	
	private GameObject orthoCam;	// A reference to the orthographic camera in the scene.
	
	static Material lineMaterial;
	
	void Start()
	{
		// Assign this value immediately.
		orthoCam = GameObject.FindWithTag("OrthoCam");
	}
	
	// Create the line material
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

	void OnPostRender() {
		
		GL.PushMatrix();
		
		CreateLineMaterial();
		lineMaterial.SetPass( 0 );
		GL.Begin(GL.LINES);
		
		// Draw the blue line that represents the heading of the camera.
		// We draw it as long as the camera's far clip plane in order to demonstrate the total capture distance.
		GL.Color(Color.blue);
		GL.Vertex3(orthoCam.transform.position.x,orthoCam.transform.position.y,orthoCam.transform.position.z);
		Vector3 plusFwd = orthoCam.transform.position + (orthoCam.transform.forward * orthoCam.GetComponent<Camera>().farClipPlane);
		GL.Vertex3(plusFwd.x,plusFwd.y,plusFwd.z);
		
		GL.End();
		
		// Draw the red line that represents the right hand vector of the camera.
		GL.Begin(GL.LINES);
		GL.Color(Color.red);
		GL.Vertex3(orthoCam.transform.position.x,orthoCam.transform.position.y,orthoCam.transform.position.z);
		Vector3 plusRgt = orthoCam.transform.position + (orthoCam.transform.right * orthoCam.GetComponent<Camera>().orthographicSize);
		GL.Vertex3(plusRgt.x,plusRgt.y,plusRgt.z);
		
		GL.End();
		
		// Now we draw a series of green lines that represents the camera's frame.
		// This is the size of the orthographic view represented by the camera.
		GL.Begin(GL.LINES);
		GL.Color(Color.green);
		Vector3 stepRight = orthoCam.transform.right * orthoCam.GetComponent<Camera>().orthographicSize;
		Vector3 stepUp = orthoCam.transform.up * orthoCam.GetComponent<Camera>().orthographicSize;
		
		Vector3 UL = orthoCam.transform.position - stepRight + stepUp;
		Vector3 LL = orthoCam.transform.position - stepRight - stepUp;
		Vector3 LR = orthoCam.transform.position + stepRight - stepUp;
		Vector3 UR = orthoCam.transform.position + stepRight + stepUp;
		
		GL.Vertex3(UL.x,UL.y,UL.z);
		GL.Vertex3(UR.x,UR.y,UR.z);
		
		GL.Vertex3(UR.x,UR.y,UR.z);
		GL.Vertex3(LR.x,LR.y,LR.z);
		
		GL.Vertex3(LR.x,LR.y,LR.z);
		GL.Vertex3(LL.x,LL.y,LL.z);
		
		GL.Vertex3(LL.x,LL.y,LL.z);
		GL.Vertex3(UL.x,UL.y,UL.z);
		
		GL.End();
		
		
		GL.PopMatrix();
		
	}
}
