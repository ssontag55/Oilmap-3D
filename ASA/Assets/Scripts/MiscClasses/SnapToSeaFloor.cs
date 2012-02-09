using UnityEngine;
using System.Collections;

public class SnapToSeaFloor : MonoBehaviour {
	
	// A simple script to snap a given object to the floor of the ocean, using a raycast.
	public LayerMask environment;	// A layer mask to hold information on layers that contain terrain.
	
	IEnumerator Start () {
		yield return 0;
		yield return 0;
		yield return 0;
		// Yield several frames upon creation to make sure there's an ocean floor beneath this object.
		// Clear our parent.
		transform.parent = null;
		// Put is at the top of the ocean, where we were.  This nearly guarantees we'll hit terrain by raycasting downward.
		transform.position = new Vector3(transform.position.x,0.0f,transform.position.z);
		RaycastHit floorHit;
		// If we hit any terrain...
		if(Physics.Raycast(transform.position,-1*Vector3.up,out floorHit, environment))
		{
			// Change our position to where that hit occurred.
			transform.position = floorHit.point;
			
		}
	}
	
	
}
