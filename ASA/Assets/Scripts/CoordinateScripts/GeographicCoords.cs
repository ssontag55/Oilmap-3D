using UnityEngine;
using System.Collections;

public class GeographicCoords
{
	// Unity has a large size limit on its coordinates.
	// If we're given a chunk of land, we're going to just say
	// that it's upper right corner is at the origin.
	
	// We STORE the latitude and longitude of this corner though
	// because we need it to establish where the scenario begins at
	// and where the spill is located.
	private static Vector2 baseLatLon = Vector2.zero;
	private static Vector3 spillLatLon = Vector2.zero;
	
	// We also store the largest X, Y, and Z values that we use when building the terrain.
	private static Vector3 maxCoordsXYZ;
	// We're also scaling the coordinates of the world.
	// Currently stored as a non-uniform scale to make the terrain look more interesting.
	private static Vector3 coordScaling = new Vector3(0.002f,0.1f,0.002f);
	
	public static float Scaling()
	{
		return coordScaling.x*1.0f;
	}
	public static float VerticalScaling()
	{
		return coordScaling.y*1.0f;
	}
	
	public static Vector3 ScaleVec
	{
		get
		{
			return coordScaling;
		}
		set
		{
			coordScaling = value;
		}
	}
	
	public static Vector3 SpillLoc
	{
		get
		{
			return spillLatLon;
		}
		set
		{
			spillLatLon = value;
		}
	}
	
	
	// Setters and getters for the Base latitude and longitude.
	// This was translated literally from Unity's variation of Javascript
	public static void SetBase(Vector2 theBase)
	{
		baseLatLon = theBase;
	}
	
	// Get or set maxCoordsXYZ
	public static Vector3 MaxCoords
	{
		get
		{
			return maxCoordsXYZ;
		}
		set
		{
			maxCoordsXYZ = value;
		}
	}
	
	public static Vector2 GetBase()
	{
		return baseLatLon;
	}
	
	// Calculate the geographic distance from one lat/lon to another.
	// This returns a 2-component vector of the distance in meters.
	
	public static Vector2 GeographicDistance(Vector2 latLonA , Vector2 latLonB)
	{
		// The Mathf class gives a lot of the functionality we need here.
		// Just applying the math.
		float earthRadius = 6371100f;
		float metersLat = (2 * Mathf.PI * earthRadius) / 360.0f;
		float metersLng = metersLat * Mathf.Cos(((latLonA.y+latLonB.y)/2.0f)*Mathf.Deg2Rad);
		
		float xMeters = (latLonB.x - latLonA.x) * metersLng;
		float xMeters2 = xMeters;
		if(latLonA.x > 0 && latLonB.x < 0)
		{
			xMeters2 = ((latLonB.x + 360) - latLonA.x) * metersLng;
		}
		if(Mathf.Abs(xMeters2) < Mathf.Abs(xMeters))
		{
			xMeters = xMeters2;
		}
		
		if(latLonA.x < 0 && latLonB.x > 0)
		{
			xMeters2 = ((latLonB.x - 360) - latLonA.x) * metersLng;
		}
		if(Mathf.Abs(xMeters2) < Mathf.Abs(xMeters))
		{
			xMeters = xMeters2;
		}
		
		float yMeters = (latLonB.y - latLonA.y) * metersLat;
		
		
		return new Vector2(xMeters,yMeters);
	}
	
}
