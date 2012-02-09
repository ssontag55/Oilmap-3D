using UnityEngine;
using System.Collections;

public class UVSwapper : MonoBehaviour {

	/*
	 * A simple script that binds toggling of the color coded depth map to "1" on the keyboard.
	 */ 
	public Material colorCoded;	// The color coded material for this tile
	public Material terrainMat; // The terrain material for this tile
	public Texture2D colorMapped; // The color coded texture to be applied to the color coded material
	
	private bool usingColorCoded = false;
	void Start () {
		renderer.material = terrainMat;
		colorMapped = ParseDEP.CreateDepthTexture(GetComponent<MeshFilter>().mesh);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("1"))
		{
			SwapMaterials();
		}
	
	}
	
	void SwapMaterials()
	{
		Vector2[] currentUVs = GetComponent<MeshFilter>().mesh.uv;
		GetComponent<MeshFilter>().mesh.uv = GetComponent<MeshFilter>().mesh.uv2;
		GetComponent<MeshFilter>().mesh.uv2 = currentUVs;
		
		if(!usingColorCoded)
		{
			usingColorCoded = true;
			renderer.material =colorCoded;
			colorCoded.mainTexture = colorMapped;
		}
		else
		{
			usingColorCoded = false;
			renderer.material = terrainMat;
		}
	}
}
