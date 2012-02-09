using UnityEngine;
using System.Collections;
public class Particle3D
{
	float latitude = 0f;
	float longitude = 0f;
	float height = 0f;
	
	long nwhere;
	float mass;
	float radius;
	float thickness;
	float viscosity;
	float watercontent;
	float flashpoint;
	
	
	//SIMAP specific:
	float xspilold;
	float yspilold;
	long ptype = -1;
	float surf;
	float dspilm;
	float viscm;
	float fwc;
	
	private Vector3 xyz = Vector3.zero;
	private static Color[] pCols = {new Color(1.0f,0.0f,0.0f,1.0f),new Color(0.0f,1.0f,0.0f,1.0f),new Color(0.0f,0.0f,1.0f,1.0f),new Color(1.0f,1.0f,1.0f,1.0f)};
	
	// Constructor right now that takes the location of the particle.
	public Particle3D(float lat,float lng,float nHeight,int where,float nmass,float nradius, float nthickness, float nviscosity, float nwatercontent, float nflashpoint)
	{
		latitude = lat;
		longitude = lng;
		height = -1*nHeight;
		nwhere = where;
		radius = nradius;
		
		thickness = nthickness;
		viscosity = nviscosity;
		
		// Now we calculate the XYZ coordinates.
		Vector2 geoDist = GeographicCoords.GeographicDistance(GeographicCoords.GetBase(),new Vector2(latitude,longitude));
		xyz = new Vector3(geoDist.x*GeographicCoords.Scaling(),height*GeographicCoords.VerticalScaling(),geoDist.y*GeographicCoords.Scaling());
		
	}
	
	public Particle3D(float nLat,float nLong, float nHeight, float nxspilold, float nyspilold, long nptype, float nsurf, float ndspilm, float nviscm, float nfwc)
	{
		latitude = nLat;
		longitude = nLong;
		height = -nHeight;
		
		
		
		xspilold = nxspilold;
		yspilold = nyspilold;
		ptype = (long)Mathf.Abs(nptype);
		
		if(ptype < 0 || ptype > 3)
		{
			Debug.Log(ptype);
			nwhere = 21;
		}
		Vector2 geoDist = GeographicCoords.GeographicDistance(GeographicCoords.GetBase(),new Vector2(latitude,longitude));
		xyz = new Vector3(geoDist.x*GeographicCoords.Scaling(),height*GeographicCoords.VerticalScaling(),geoDist.y*GeographicCoords.Scaling());
		if((xyz.x < 0 || xyz.x > GeographicCoords.MaxCoords.x) || (xyz.z < 0 || xyz.z > GeographicCoords.MaxCoords.z))
		{
			nwhere=21;
		}
	}
	
	public void SetNWhere(int val)
	{
		nwhere = val;
	}
	
	public float GetViscosity()
	{
		return viscosity;
	}
	
	public float GetThickness()
	{
		return thickness;
	}
	
	public float GetRadius()
	{
		return radius;
	}
	
	public long GetNWhere()
	{
		return nwhere;
	}
	
	public Particle MakeParticle()
	{
		Particle tempParticle = new Particle();
		Debug.Log("Making a particle at: " + xyz);
		tempParticle.position = xyz;
		return tempParticle;
	}
	
	public Color GetPColor() 
	{
		if(ptype > -1)
		{
			long pCol = (long)Mathf.Clamp(ptype,0,pCols.Length-1);
			return pCols[pCol];
		}
		else
		{
			return pCols[pCols.Length-1];
		}
	}
	
	public Vector3 GetXYZ()
	{
		return xyz;
	}
	
	//Other attributes to be added later.
	
}