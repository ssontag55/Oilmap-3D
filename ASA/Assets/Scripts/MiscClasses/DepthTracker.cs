using UnityEngine;
using System.Collections;

public class DepthTracker : MonoBehaviour {

	// Simple script that places 3D text at defined points from the surface to the
	// bottom of the ocean, using the location of the psill as a column of reference.
	
	public int resolution = 10;	// How many depth markers will we have?
	public GameObject depthText; // The prefab for the depth marker.  Should have a TextMesh component
	
	
	IEnumerator Start()
	{
		yield return new WaitForSeconds(0.5f);	// Wait 0.5 seconds to make sure the spill is where it should be
		Vector3 spawnPt;
		float depthRatio;
		GameObject clone;
		for(int i = 0; i < resolution; i++)
		{
			// Set the percentage of our total depth that we'll place this marker.
			depthRatio = (1.0f*i) / (1.0f * resolution);
			
			// Computer the spawn point, where the depth is depthRatio * transform.position.y.
			spawnPt = new Vector3(transform.position.x,transform.position.y*depthRatio,transform.position.z);
			// Spawn the marker.
			clone = (Instantiate(depthText,spawnPt,Quaternion.identity) as GameObject);
			
			// Now alter the text of the marker to display the depth information.
			clone.GetComponent<TextMesh>().text = ""+((transform.position.y*depthRatio)/GeographicCoords.VerticalScaling()) + "m";
		}
		
		// After spawning the above markers, place one directly on the bottom of the ocean, too.
		spawnPt = new Vector3(transform.position.x,transform.position.y,transform.position.z);
		clone = (Instantiate(depthText,spawnPt,Quaternion.identity) as GameObject);
		clone.GetComponent<TextMesh>().text = ""+((transform.position.y)/GeographicCoords.VerticalScaling()) + "m";
		
	}
	

}
