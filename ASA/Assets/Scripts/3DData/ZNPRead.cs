using UnityEngine;
using System.Collections;
using System.IO;

public class ZNPRead {
	
	/*
	 * A quick and simple class to read ZNP files.
	 * All this really does is look for very specific lines in the .ZNP
	 * in order to determine things such as the location of the oil spill
	 * and what .DEP file should be opened.
	 */
	
	public static string ReadZNP(string fPath)
	{
		StreamReader fileReader = new StreamReader(fPath);
		string theLine = "";
		Vector3 spillLocation = Vector3.zero;
		string correspondingGridFile = "";
		while((theLine = fileReader.ReadLine()) != null)
		{
			string[] parseArray;
			float theArg = 0.0f;
			if(theLine.Contains("Release Depth"))
			{
				parseArray = theLine.Split("="[0]);
				float.TryParse(parseArray[1],out theArg);
				spillLocation = new Vector3(spillLocation.x,theArg,spillLocation.z);
			}
			else if(theLine.Contains("Spill Lon"))
			{
				parseArray = theLine.Split("="[0]);
				float.TryParse(parseArray[1],out theArg);
				spillLocation = new Vector3(theArg,spillLocation.y,spillLocation.z);
				
			}
			else if(theLine.Contains("Spill Lat"))
			{
				parseArray = theLine.Split("="[0]);
				float.TryParse(parseArray[1],out theArg);
				spillLocation = new Vector3(spillLocation.x,spillLocation.y,theArg);
			}
			else if(theLine.Contains("Grid File"))
			{
				parseArray = theLine.Split("="[0]);
				correspondingGridFile = parseArray[1];
			}
		}
		fileReader.Close();
		GeographicCoords.SpillLoc = spillLocation;
		Debug.Log("SPILL IS HERE: " +spillLocation);
		return correspondingGridFile;
	}
	
	
}
