using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ASAMAP.IO;

public class SIMAPTester : MonoBehaviour {
	
	public string fileLoc = "/Users/jcarvalho/Desktop/ASASIMAP/OILMAPv6/loc_data/GULF_MEX/Modelout/D5000_GOR100_LIGHT-MID";
	
	// Use this for initialization
	void Start () {
		SIMAP test = new SIMAP(fileLoc);
		
		ArrayList first = new ArrayList();
		ArrayList sec = new ArrayList();
		ArrayList third = new ArrayList();
		ArrayList fourth = new ArrayList();
		
		DateTime dt = DateTime.Parse("12/31/1979 00:00");
		
		Debug.Log(test.ReadSPillData(dt,fileLoc,ref first,ref sec,ref third,ref fourth,0));
	
	}
	
	
}
