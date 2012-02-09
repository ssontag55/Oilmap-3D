/*
 * ParseDEP class
 * This class parses a binary .DEP file to produce either a single mesh or tiles of a mesh
 * based on the bathymetry data of the .DEP file.
 * 
 * Note that current limitations in Unity's engine limits a mesh to having approximately 65,000 vertices.
 * This necessitates tiling the .DEP file when it contains more than 65k depths.
 * 
 */ 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

class ParseDEP
{
	
	/*
	 * BuildTiles
	 * 
	 * Takes in the file path of the .DEP file.  If stretchUVs is true then the corresponding tile's UVs will be mapped over a 0..1,0..1 area
	 * meaning that the texture chosen for a given tile will stretch the entire area rather than tiling over it.
	 * 
	 * 
	 * Returns an array of meshes to use as tiles.
	 * 
	 */ 
	
	public static Mesh[] BuildTiles(string fPath, bool stretchUVs)
	{
		
		//Open the binary file and check the conversion of the units.
		BinaryReader binFile = new BinaryReader(File.Open(fPath,FileMode.Open));
		float conversion = 1.0f;
		int con = binFile.ReadInt16();
		// If con != -1, the depths are in Decimeters.  Set up a conversion to record the depths in the proper units.
		if(con != -1)
			conversion = 0.1f;
		
		// We want to keep track of the maximum depth and maximum coordinates of the set of tiles.
		// This will become necessary when building the water's surface after generating the tiles.
		float maxDepth = 0.0f;
		float maxX = 0.0f;
		float maxZ = 0.0f;
		
		con = binFile.ReadInt16();
		
		// Get the latitude and longitude of the lower left hand corner of the area.
		Vector2 originLatLon = new Vector2(binFile.ReadSingle(),binFile.ReadSingle());
		// We store this in the GeographicCoords class for future use.
		GeographicCoords.SetBase(originLatLon);
		// Record the cell size in degrees.
		Vector2 cellSizeDeg = new Vector2(binFile.ReadSingle(),binFile.ReadSingle());
		// Print this to the console.
		Debug.Log("Cell Size: " + cellSizeDeg);
		// Get the dimensions of our grid.
		Vector2 gridDims = new Vector2(binFile.ReadInt16(),binFile.ReadInt16());
		int theSize = (int)gridDims.x * (int) gridDims.y;
		
		// Read every single data value.
		int[] dataVals = new int[theSize];
		int i = 0;
		for(i = 0; i < dataVals.Length; i++)
		{
			dataVals[i] = binFile.ReadInt16();
		}
		binFile.Close();
		// Get the number of tiles for the area.
		int tileCols = Mathf.Max(1,(int)Mathf.Ceil((int)gridDims.x / 250));
		int tileRows = Mathf.Max(1,(int)Mathf.Ceil((int)gridDims.y/250));
		
		
		List<Mesh> meshList = new List<Mesh>();
		
		// From here, we can determine if building a set of tiles is really necessary.
		// If tileCols = 1 and tileRows = 1, then we don't need all of the overhead below.
		
		// Just call BuildGeo.
		
		Debug.Log("Total Tiles: " + tileCols + ","+ tileRows);
		
		
		// Keep track of the world space coordinates of the model
		Vector2 vertXZ = Vector2.zero;
		// Start our degree vector at the base lat lon
		Vector2 degVec = GeographicCoords.GetBase();
		
		
		
		for(int j = 0; j < tileCols; j++)
		{
			// For each column of tiles we have
			//float tileZ = vertXZ.y;
			
			for(int k = 0; k < tileRows; k++)
			{
				// For each row now, determine where this tile should start.
				// We impose a 250x250 vert limit on tiles because this is below the 65k limit and the tiles remain square.
				// Using this assumption it's easy to determine where new tiles should begin.
				degVec = new Vector2(GeographicCoords.GetBase().x + Mathf.Min(250,gridDims.x) * cellSizeDeg.x * j,GeographicCoords.GetBase().y+ Mathf.Min(250,gridDims.y) * cellSizeDeg.y * k);
				// Create a tile.
				//Debug.Log("DEGREE VEC: " + degVec);
				// Determine where the first vertex of this tile should be based on the geographic distance of its corner from
				// the base latitude/longitude.
				vertXZ = GeographicCoords.GeographicDistance(GeographicCoords.GetBase(),degVec)*GeographicCoords.Scaling();
				
				
				// Determine the dimensions of this tile.
				// The last tiles created may have dimensions smaller than 250x250.
				int remainingCols = (int)Mathf.Min(250,gridDims.x - (250 * j));
				int remainingRows = (int)Mathf.Min(250,gridDims.y - (250*k));
				
				// Store the verts of the mesh as an array of 3-component vectors.
				Vector3[] vertexDepths = new Vector3[remainingCols*remainingRows];
				// Likewise, the UVs are an array of 2-component vectors.
				Vector2[] uvs = new Vector2[vertexDepths.Length];
				
				// As we start the tile, we want keep track of where the X position of this tile was.
				// Since we're building the entire column of tiles at once, we want to reset to this position
				// after a tile is made.
				float tileX = vertXZ.x;
				for(int l = 0;l < (remainingRows); l++)
				{
					for(int m = 0; m < remainingCols; m++)
					{
						// Building the tile now.
						// Populate vertexDepths with information.
						// We grab specific values from the .DEP file, converting them as necessary.
						// We also apply scaling universally to the coordinates so that they don't get "out of hand."
						vertexDepths[(l*remainingCols) + m] = new Vector3(vertXZ.x,-1*dataVals[(250*j+m)+((250*k+l)*(int)gridDims.x)] * conversion * GeographicCoords.VerticalScaling(),vertXZ.y);
						// Record max depth, max X coordinate and max Z coordinate (Unity is a Y-Up engine).
						if(Mathf.Abs(vertexDepths[(l*remainingCols) + m].y) > Mathf.Abs(maxDepth))
						{
							maxDepth = vertexDepths[(l*remainingCols) + m].y;
						}
						if(vertexDepths[(l*remainingCols) + m].x > maxX)
						{
							maxX = vertexDepths[(l*remainingCols) + m].x;
						}
						if(vertexDepths[(l*remainingCols) + m].z > maxZ)
						{
							maxZ = vertexDepths[(l*remainingCols) + m].z;
						}
						
						//For the UVs, we simply place them where they would be if the entire heightmap were "flattened."  This will cause
						// the corresponding texture to tile across the surface.
						uvs[(l*remainingCols) + m] = new Vector2(vertexDepths[(l*remainingCols) + m].x,vertexDepths[(l*remainingCols) + m].z);
						// Increment our vertex position an appropriate distance based on the latitude/longitude of the vert's position and the next vert's latitude/longitude.
						vertXZ += GeographicCoords.GeographicDistance(degVec,degVec+ new Vector2(cellSizeDeg.x,0.0f))*GeographicCoords.Scaling();
						degVec.x += cellSizeDeg.x;
						
					}
					// Revert to our old X position
					vertXZ.x = tileX;
					degVec.x = GeographicCoords.GetBase().x + Mathf.Min(250,gridDims.x) * cellSizeDeg.x * j;
					vertXZ += GeographicCoords.GeographicDistance(degVec,degVec+ new Vector2(0.0f,cellSizeDeg.y))*GeographicCoords.Scaling();
					degVec.y += cellSizeDeg.y;
					
				}
				
				// After we build the vertex array for our tile, we must now inform the engine of how these
				// vertices are grouped in terms of triangles.
				// I've found this is easiest to use a list for it
				List<int> tempTri = new List<int>();
				for(int n = 0; n < remainingRows-1; n++)
				{
					for(int o = 0; o < remainingCols-1; o++)
					{
						// Note that this configuration sets up one quad of the tile.
						// We're basically setting up each quad of tile in each execution of this loop.
						
						// The comments here throw out a hypothetical example:
						// Say we have a 16x16 tile.
						
						// When we build the triangles, every triplet of values in this int array corresponds to a triangle.
						// The values in the triplet correspond to the index of the verts we defined above.
						
						// The example crafted in the comments will assume we're starting from vertex 0.
						
						int offset = n*remainingCols;
						tempTri.Add(offset+o);			// Example: This is vertex "0"
						tempTri.Add(offset+o+remainingCols); // Then this is vertex "0+16" or vert 16.
						tempTri.Add(offset+o+1);		// Now we go back up to vert "1".  This defines a triangle in a counter-clockwise fashion.
						
						// Now to complete the quad, we make the second triangle.
						tempTri.Add(offset+o+remainingCols);  // Start at vert "16"
						tempTri.Add(offset+o+remainingCols+1);  // Go to the next vert after this, "17"
						tempTri.Add(offset+o+1);  //  Move back to "1"
					}
					
				}
				
				// Unity meshes can have a second set of UVs.
				// We're going to assume these coordinates will always be stretched.
				// In the context of the visualizer, we only want these particular UVs to be used with the color coded 
				// height information of the terrain.
				Vector2[] uvs2 = new Vector2[uvs.Length];
				Vector2 uvUnit = new Vector2(maxX,maxZ);
				//Create a vector in which to scale our UVs.
				uvUnit.x = 1.0f/uvUnit.x;
				uvUnit.y = 1.0f/uvUnit.y;
				
	
				
				for(int q = 0; q < uvs.Length; q++)
				{
					// We take the origin  al UVs and scale them down into something that lies between
					// 0.0,1.0 in both X and Y.  We do the same thing to the original UVs if they're supposed to be stretched.
					uvs2[q] = Vector2.Scale(uvs[q],uvUnit);
					
				}
				
				// Create a new Mesh onbject and assign the vertex, UV and triangle information.
				Mesh theMesh = new Mesh();
				theMesh.vertices = vertexDepths;
				theMesh.uv = uvs;
				theMesh.uv2 = uvs2;
				theMesh.triangles = tempTri.ToArray();
				theMesh.RecalculateNormals();
				
				// Now we're going to calculate tangents...
				// We calculate the normals and tangents of the mesh in the event we want to use any shaders which depend on them
				// such as shaders with normal/bump maps.
				Vector3[] theNormals  = theMesh.normals;
				Vector4[] theTangents = new Vector4[theNormals.Length];
				for(int l = 0; l < theTangents.Length; l++)
				{
					theTangents[l] = new Vector4(theNormals[l].x,theNormals[l].y,theNormals[l].z,1);
				}
				theMesh.tangents = theTangents;
				// Add the mesh to our list.
				meshList.Add(theMesh);
				Debug.Log("Made tile");
			}
		}
		// Record our maximum coordinates
		GeographicCoords.MaxCoords = new Vector3(maxX,maxDepth,maxZ);
		// Return the array of tiles.
		return meshList.ToArray();
	}
	
	
	/*
	 * CreateDepthTexture
	 * Creates a texture in run time that is color coded by depth.
	 * Takes in a mesh to compare the height of its verts against the maximum depth stored in GeographicCoords
	 * in order to color the texture.
	 */ 
	
	public static Texture2D CreateDepthTexture(Mesh tile)
	{
		// Set up our colors.  In this case, near white to dark grey.
		Color lowestDepth = new Color(0.9f,0.9f,0.9f,1.0f);
		Color maxDepth = new Color(0.1f,0.1f,0.1f,1.0f);
		
		Vector3[] theVerts = tile.vertices;
		Vector2[] theUvs = tile.uv2;
		
		// The max size for a tile mesh is 250x250, so 256x256 is acceptable
		// Keep it in powers of 2, and keep it square.  OpenGL will otherwise calculate it to the nearest power of two in each
		// dimension above it, wasting resources.
		// This texture is a 32bit RGBA texture with NO mipmapping.
		Texture2D producedTexture = new Texture2D(256,256, TextureFormat.ARGB32,false);
		
		// We set up an array of colors that will eventually go into our texture.
		Color[] pixelColors = new Color[256*256];
		
		for(int j = 0; j < pixelColors.Length; j++)
		{
			//Set all initial pixels to black, just in case some of them aren't set for some reason.
			pixelColors[j] = Color.black;
		}
		
		for(int i = 0; i < theVerts.Length; i++)
		{
			// Calculate how deep this vert is relative to the maximum depth of all tiles.
			float depthPercentage = theVerts[i].y / GeographicCoords.MaxCoords.y;
			
			// Calculate what pixel we'll be altering on the texture based on the UVs.
			int pixelCol = Mathf.FloorToInt(theUvs[i].x * 256);
			int pixelRow = Mathf.FloorToInt(theUvs[i].y * 256);
			
			// We do a bit of "flooding" around that corresponding pixel to avoid any gaps in our texture.
			for(int k = pixelRow-3; k < pixelRow+1; k++)
			{
				for(int l = pixelCol-1; l < pixelCol+2; l++)
				{
					// Calculate the position on the texture we want to alter.
					int texturePosition = (k * 256) + l;
					if(texturePosition >= 0 && texturePosition < pixelColors.Length)
					{
						// One of the great things in Unity's API is the Lerp function.
						// It can be found in the Mathf class for interpolating floating point numbers
						// In the Vector classes to interpolate the components of vectors
						// and in the color class, too.
						
						// In just about all of these cases, the Lerp methods take arguments as follows:
						
						// From-value, the To-Value, and how far between those we want.
						// In this case, lowestDepth is our sea level color, or 0.0 depth.
						// Max depth is whatever the maximum depth from the .DEP file was.  We want something depthPercentage between that.
						pixelColors[texturePosition] = Color.Lerp(lowestDepth,maxDepth,depthPercentage);
					}
				}
			}
		}
		// Set the pixels of our texture based on the colors we manipulated above.
		producedTexture.SetPixels(pixelColors);
		// Apply them to the texture
		producedTexture.Apply();
		
		return producedTexture;
		
	}
	
	/*
	 * BuildGeo
	 * 
	 * This is really the same as the BuildTiles function above, only it only builds a single tile.
	 */ 
	
	public static Mesh BuildGeo(string fPath, bool stretchUVs)
	{
		BinaryReader binFile = new BinaryReader(File.Open(fPath,FileMode.Open));
		float conversion = 1;
		int throwAway = binFile.ReadInt16();
		if(throwAway != -1)
		{
			Debug.Log("Decimeters");
			conversion = 0.1f;
		}
		float maxDepth = 0.0f;
		float maxX = 0.0f;
		float maxZ = 0.0f;
		throwAway = binFile.ReadInt16();
		
		Vector2 originLatLon = new Vector2(binFile.ReadSingle(),binFile.ReadSingle());
		Debug.Log("Origin At: " + originLatLon);
		GeographicCoords.SetBase(originLatLon);
		Vector2 cellSizeDeg = new Vector2(binFile.ReadSingle(),binFile.ReadSingle());
		Debug.Log("Cell Size: " + cellSizeDeg);
		Vector2 gridDims = new Vector2(binFile.ReadInt16(),binFile.ReadInt16());
		Debug.Log("Grid Dims: " + gridDims);
		int theSize = (int)gridDims.x * (int)gridDims.y;
		
		// This is the array that stores our depths.
		int[] dataVals = new int[theSize];
		
		int i = 0;
		for(i = 0; i < dataVals.Length; i++)
		{
			dataVals[i] = binFile.ReadInt16();
		}
		binFile.Close();
		int numCols = (int)gridDims.x;
		int numRows = (int)gridDims.y;
		Vector3[] vertexDepths = new Vector3[theSize];
		Vector2[] uvs = new Vector2[theSize];
		Vector2 degVec = GeographicCoords.GetBase();
		Vector2 vertXZ = Vector2.zero;
		
		for(i = 0; i < theSize; i++)
		{
			vertexDepths[i] = new Vector3(vertXZ.x,-1*dataVals[i]*GeographicCoords.VerticalScaling()*conversion,vertXZ.y);
			vertXZ += GeographicCoords.GeographicDistance(degVec,degVec+ new Vector2(cellSizeDeg.x,0.0f))*GeographicCoords.Scaling();
			degVec.x += cellSizeDeg.x;
			if(Mathf.Abs(vertexDepths[i].y) > Mathf.Abs(maxDepth))
			{
				maxDepth = vertexDepths[i].y;
			}
			if(vertexDepths[i].x > maxX)
			{
				maxX = vertexDepths[i].x;
			}
			if(vertexDepths[i].z > maxZ)
			{
				maxZ = vertexDepths[i].z;
			}
			if((i+1) % numCols == 0 && (i != theSize-1))
			{
				degVec.x = GeographicCoords.GetBase().x;
				vertXZ.x = 0.0f;
				vertXZ += GeographicCoords.GeographicDistance(degVec,degVec+ new Vector2(0.0f,cellSizeDeg.y))*GeographicCoords.Scaling();
				degVec.y += cellSizeDeg.y;
			}
			uvs[i] = new Vector2(vertexDepths[i].x,vertexDepths[i].z);
		}
		
		GeographicCoords.MaxCoords = new Vector3(maxX,maxDepth,maxZ);
		
		binFile.Close();
		List<int> tempTri = new List<int>();
		
		
		
		for(int j = 0; j < numRows-1; j++)
		{
			for(int k = 0; k < numCols-1; k++)
			{
				//if(j < numRows-1)
				//{
					int offset = j*numCols;
					tempTri.Add(offset+k);			// "0"
					tempTri.Add(offset+k+numCols); // "0+16"
					tempTri.Add(offset+k+1);		// "1"
					
					
					tempTri.Add(offset+k+numCols);  // "16"
					tempTri.Add(offset+k+numCols+1);  // "17"
					tempTri.Add(offset+k+1);  //"1"
				//}
				
			}
			
		}
		
		Mesh theMesh = new Mesh();
		theMesh.vertices = vertexDepths;
		theMesh.uv = uvs;
		theMesh.triangles = tempTri.ToArray();
		theMesh.RecalculateNormals();
		
		// Now we're going to calculate tangents...
		Vector3[] theNormals  = theMesh.normals;
		Vector4[] theTangents = new Vector4[theNormals.Length];
		for(int l = 0; l < theTangents.Length; l++)
		{
			theTangents[l] = new Vector4(theNormals[l].x,theNormals[l].y,theNormals[l].z,1);
		}
		theMesh.tangents = theTangents;
		
		return theMesh;
		
		
	}
	
	
}