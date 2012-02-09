using UnityEngine;
using System.Collections;

public class GLWaterLine : MonoBehaviour {
	
	
	// A simple class that just draws a thin blue line where the water line should be
	// in the orthographic view.
	
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

	void OnPostRender() {
		if(transform.eulerAngles.x != 0.0f)
			return;
		GL.PushMatrix();
		
		CreateLineMaterial();
		lineMaterial.SetPass( 0 );
		GL.Begin(GL.LINES);
		GL.Color(Color.blue);
		float frameSize = GetComponent<Camera>().orthographicSize;
		// Set up the points in world space for the water line
		Vector3 leftOfCam = transform.position -transform.right*frameSize + transform.forward;
		Vector3 rightOfCam = transform.position + transform.right*frameSize + transform.forward;
		
		// Draw the water line.
		GL.Vertex3(leftOfCam.x,0,leftOfCam.z);
		GL.Vertex3(rightOfCam.x,0,rightOfCam.z);
		
		
		
		GL.End();
		
		GL.PopMatrix();
		
	}
}
