using UnityEngine;
using System.Collections;

public class GLBounds : MonoBehaviour {
	
	// A simple class for drawing the bounding box of an object's renderer at runtime
	// Using Unity's GL class to access OpenGL.
	
	GameObject[] objOfInterest;		// The object we want to draw our box around.  Specifically for this, it's an oil spill.
	static Material lineMaterial;	// A material for drawing lines.
	
	IEnumerator Start()
	{
		while(objOfInterest == null)
		{
			yield return 0; // Wait a frame, then try to set objOfInterest.  Keep doing this until it sticks.
			// This is because the spill doesn't spawn immediately, but the object this is attached to might spawn before
			// the spill.
			objOfInterest = GameObject.FindGameObjectsWithTag("Spill");
		}
	}
	
	// Set up the line material.
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
		objOfInterest = GameObject.FindGameObjectsWithTag("Spill");
		// Don't bother with anything if we don't have an object to draw.
		if(objOfInterest == null)
			return;
		foreach(GameObject obj in objOfInterest)
		{
			// Get the center of the bounds.
			Vector3 centerPos = obj.renderer.bounds.center;
			// Get the size vector of the bounding box.
			Vector3 sizeVec = obj.renderer.bounds.size;
			
			
			// Set up our drawing matrix.
			GL.PushMatrix();
			// Create the line material if needed.
			CreateLineMaterial();
			lineMaterial.SetPass( 0 );
			// We're just going to draw the box via lines.
			GL.Begin(GL.LINES);
			GL.Color(Color.black);
			
			// Set up the four edges at the top of the box.
			GL.Vertex3(centerPos.x+sizeVec.x,centerPos.y+sizeVec.y,centerPos.z+sizeVec.z);
			GL.Vertex3(centerPos.x-sizeVec.x,centerPos.y+sizeVec.y,centerPos.z+sizeVec.z);
			
			GL.Vertex3(centerPos.x-sizeVec.x,centerPos.y+sizeVec.y,centerPos.z+sizeVec.z);
			GL.Vertex3(centerPos.x-sizeVec.x,centerPos.y+sizeVec.y,centerPos.z-sizeVec.z);
			
			GL.Vertex3(centerPos.x-sizeVec.x,centerPos.y+sizeVec.y,centerPos.z-sizeVec.z);
			GL.Vertex3(centerPos.x+sizeVec.x,centerPos.y+sizeVec.y,centerPos.z-sizeVec.z);
			
			GL.Vertex3(centerPos.x+sizeVec.x,centerPos.y+sizeVec.y,centerPos.z-sizeVec.z);
			GL.Vertex3(centerPos.x+sizeVec.x,centerPos.y+sizeVec.y,centerPos.z+sizeVec.z);
			
			// Draw the four edges at the bottom of the box.
			GL.Vertex3(centerPos.x+sizeVec.x,centerPos.y-sizeVec.y,centerPos.z+sizeVec.z);
			GL.Vertex3(centerPos.x-sizeVec.x,centerPos.y-sizeVec.y,centerPos.z+sizeVec.z);
			
			GL.Vertex3(centerPos.x-sizeVec.x,centerPos.y-sizeVec.y,centerPos.z+sizeVec.z);
			GL.Vertex3(centerPos.x-sizeVec.x,centerPos.y-sizeVec.y,centerPos.z-sizeVec.z);
			
			GL.Vertex3(centerPos.x-sizeVec.x,centerPos.y-sizeVec.y,centerPos.z-sizeVec.z);
			GL.Vertex3(centerPos.x+sizeVec.x,centerPos.y-sizeVec.y,centerPos.z-sizeVec.z);
			
			GL.Vertex3(centerPos.x+sizeVec.x,centerPos.y-sizeVec.y,centerPos.z-sizeVec.z);
			GL.Vertex3(centerPos.x+sizeVec.x,centerPos.y-sizeVec.y,centerPos.z+sizeVec.z);
			
			// Draw the four edges that connect the top and bottom.
			GL.Vertex3(centerPos.x+sizeVec.x,centerPos.y-sizeVec.y,centerPos.z+sizeVec.z);
			GL.Vertex3(centerPos.x+sizeVec.x,centerPos.y+sizeVec.y,centerPos.z+sizeVec.z);
			
			GL.Vertex3(centerPos.x-sizeVec.x,centerPos.y-sizeVec.y,centerPos.z+sizeVec.z);
			GL.Vertex3(centerPos.x-sizeVec.x,centerPos.y+sizeVec.y,centerPos.z+sizeVec.z);
			
			GL.Vertex3(centerPos.x-sizeVec.x,centerPos.y-sizeVec.y,centerPos.z-sizeVec.z);
			GL.Vertex3(centerPos.x-sizeVec.x,centerPos.y+sizeVec.y,centerPos.z-sizeVec.z);
			
			GL.Vertex3(centerPos.x+sizeVec.x,centerPos.y-sizeVec.y,centerPos.z-sizeVec.z);
			GL.Vertex3(centerPos.x+sizeVec.x,centerPos.y+sizeVec.y,centerPos.z-sizeVec.z);
			
			
			GL.End();
			
			
			
			GL.PopMatrix();
		
		}
		
		
	}
	
}
