using UnityEngine;
using System.Collections;

public class OilID  {
	
	/*
	 * A simple class to hold pointer information to data in a OML or ZML file.
	 */ 
	
	public int rec; // The record number we start at
	public int tStep; // The timestep this ID is for
	public int nRecs; // The number of records used.
	
	public OilID(int iRec,int iStep, int iNRecs)
	{
		rec = iRec;
		tStep = iStep;
		nRecs = iNRecs;
	}
}
