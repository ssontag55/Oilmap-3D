using UnityEngine;
using System.Collections;

public class BuildTerrainMesh : MonoBehaviour {

	public string dataFPath = "/Users/Sample.txt";
	public Material terrainMat;
	public GameObject waterPrefab;
	public GameObject tile;

	public GameObject mapGen;

	private GameObject waterClone;
	
	private static Mesh[] tiles;

	void Start()
	{
		// Build the mesh(es) before the first update.
		// The sample datapath name above should be changed by some other class before this
		// Coroutine is called.
		StartCoroutine("ProduceMesh");
	}
	

	public IEnumerator ProduceMesh()
	{
		// If for some reason there's already a water surface spawned, destroy it.
		if(waterClone != null)
			Destroy(waterClone);
		
		//Build the tiles for the associated .DEP file
		tiles = ParseDEP.BuildTiles(dataFPath,false);
		for(int i = 0; i < tiles.Length; i++)
		{
			// Now we build the objects that will hold the tiles.
			GameObject clone = (Instantiate(tile,Vector3.zero,Quaternion.identity) as GameObject);
			// Rename the tiles just to keep track of them.
			clone.name += i + "";
			// Set the object's tile and material for rendering.
			clone.GetComponent<MeshFilter>().mesh = tiles[i];
			clone.GetComponent<Renderer>().material = terrainMat;
			
			// Recalculate the bounds of that mesh.
			clone.GetComponent<MeshFilter>().mesh.RecalculateBounds();
			// Make this tile a child of this game object, for organization purposes.
			clone.transform.parent = transform;
			// Set up a mesh collider.
			clone.GetComponent<MeshCollider>().mesh = tiles[i];
			yield return 0;
			
		}
		
		// Now we build a water surface.
		
		waterClone = (Instantiate(waterPrefab,new Vector3(GeographicCoords.MaxCoords.x*0.5f,0.0f,GeographicCoords.MaxCoords.z*0.5f), Quaternion.identity) as GameObject);
		waterClone.transform.parent = transform;
		// Scale the surface of the water according to the maximum coordinates we have for the world, so that it covers the entire area.
		waterClone.transform.localScale = 0.1f* new Vector3(GeographicCoords.MaxCoords.x,10.0f,GeographicCoords.MaxCoords.z);
		
		// Make sure to tell the orthographic camera that there's a water surface
		// Since the controls to turn it on and off are in that script.
		GameObject.FindWithTag("OrthoCam").GetComponent<ContextCamera>().waterSurface = waterClone;
		
		
		
	}
}
