using UnityEngine;
using System.Collections;

public class LookAtCamera : MonoBehaviour {

	// Simple script to be attached to depth markers that will force them to always look at the camera.
	
	Transform camRef;
	
	void Start () {
		// Get the reference to the main camera.
		camRef = Camera.main.transform;
	
	}
	
	
	void Update () {
		// If there's no reference to the camera, try to get one.
		if(camRef == null)
			camRef = Camera.main.transform;
		else
		{
			// Look toward that transform.
			transform.LookAt(camRef);
			
		}
	}
}
