using UnityEngine;
using System.Collections;

public class Trackline : MonoBehaviour {
	
	// A simple script to draw the trackline defined by the oil spill.
	
	static Material lineMaterial;
	
	// The vertices for the spill.  Set by the oil spill when it finishes building the trackline.
	public Vector3[] verts;
	
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
		// Don't do anything if we don't have any verts.
		if(verts == null)
			return;
		GL.PushMatrix();
		
		CreateLineMaterial();
		lineMaterial.SetPass( 0 );
		GL.Begin(GL.LINES);
		GL.Color(Color.red);
		// Draw lines connecting all these vertices together.
		for(var i = 1; i < verts.Length; i++)
		{
			GL.Vertex3(verts[i-1].x,verts[i-1].y,verts[i-1].z);
			GL.Vertex3(verts[i].x,verts[i].y,verts[i].z);
		}
		
		GL.End();
		
		GL.PopMatrix();
		
	}
}
