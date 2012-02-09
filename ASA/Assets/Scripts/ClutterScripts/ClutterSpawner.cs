using UnityEngine;
using System.Collections;

public class ClutterSpawner : MonoBehaviour {

	public GameObject clutterZonePrefab;

	public float spacing = 25.0f;
	public LayerMask environmentLayer;
	

	public IEnumerator GenerateClutterZones(Vector3 lastVert)
	{
		int numCols = (int)(lastVert.x / spacing);
		int numRows = (int)(lastVert.z / spacing);
	
		int totalZones = numCols * numRows;
	
		Vector3 zonePlacement = Vector3.zero;
		zonePlacement.x = spacing/2.0f;
	
		for(int i = 0; i < totalZones; i++)
		{
			GameObject clone = (Instantiate(clutterZonePrefab,zonePlacement,Quaternion.identity) as GameObject);
			RaycastHit hit;
			if(Physics.Raycast(clone.transform.position,-Vector3.up,out hit,environmentLayer))
			{
				clone.transform.position = hit.point;
			}
			
			clone.transform.parent = transform;
			zonePlacement.x += spacing;
			if((i+1)%numCols == 0)
			{
				zonePlacement.x = spacing/2.0f;
				zonePlacement.z += spacing;
				//yield return 0;
			}

			
		}
		yield return 0;
	}
}
