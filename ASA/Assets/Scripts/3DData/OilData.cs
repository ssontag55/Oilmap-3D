using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

/*
	This code is largely lifted from previous code written by ASA.
	It has been adapted to work within Unity.
*/

public class OilData {
	
	/**
	 * ReadOMP
	 * Read a .OMP or .ZMP binary file.  Takes in the path of the file and a reference to an OilID array
	 * to populate the pointer records.
	 * This function was pretty much written directly from provided code by ASA.
	 * 
	 */ 
	
	public static string ReadOMP(string file, ref OilID[] refID)
	{
		int trjver;
		long numBytes = -1;
		int ntSteps = -1;
		FileStream fs = null;
		BinaryReader reader = null;
		int pos = 0;
		
		try
		{
			fs = File.Open(file,FileMode.Open,FileAccess.Read);
			numBytes = fs.Length;
			if(numBytes <= 0)
				return "Empty OMP file.";
			
			reader = new BinaryReader(fs);
			trjver = reader.ReadInt32(); // Get Traj version indicator
			if(trjver == 1002)
				ntSteps = Convert.ToInt32(numBytes/12)-1;
			else
			{
				fs.Close();
				return "Oilmap version is not supported";
			}
			pos = 12;
			reader.BaseStream.Seek(pos,SeekOrigin.Begin); // Jump ahead 12 bytes.
			System.Array.Resize(ref refID,ntSteps);
			
			for(int i = 0; i < ntSteps; i++)
			{
				// Read into an array, getting Index, Timestep, and Record#
				refID[i] = new OilID(reader.ReadInt32(),reader.ReadInt32(),reader.ReadInt32());
			}
			
			reader.Close();
			fs.Close();
			fs.Dispose();
		}
		catch (Exception ex)
		{
			return "Error: " + ex.Message;
		}
		finally
		{
			if(fs != null)
				fs.Close();
			if(reader != null)
				reader.Close();
		}
		return "OMP file read!";
	}
	
	
	public static string ReadLU3(string file, ref SiSpillets[] spillRec)
	{
		FileStream fs = null;
		BinaryReader reader = null;
		
		try
		{
			fs = File.Open(file,FileMode.Open,FileAccess.Read);
			long recCount = fs.Length/96;
			spillRec = new SiSpillets[recCount];
			reader = new BinaryReader(fs);
			for(int i = 0; i < recCount; i++)
			{
				spillRec[i] = new SiSpillets();
				spillRec[i].iver = reader.ReadInt32();
                spillRec[i].simtime = reader.ReadInt32();
                spillRec[i].time = reader.ReadSingle();
                spillRec[i].rec1st = reader.ReadInt32();
                spillRec[i].rec2st = reader.ReadInt32();
                spillRec[i].rec2end = reader.ReadInt32();
                spillRec[i].rec3st = reader.ReadInt32();
                spillRec[i].rec3end = reader.ReadInt32();
                spillRec[i].rec5st = reader.ReadInt32();
                spillRec[i].rec5end = reader.ReadInt32();
                spillRec[i].sed2st = reader.ReadInt32();
                spillRec[i].sed2end = reader.ReadInt32();
                spillRec[i].rar1st = reader.ReadInt32();
                spillRec[i].rar1end = reader.ReadInt32();
                spillRec[i].rth1st = reader.ReadInt32();
                spillRec[i].rth1end = reader.ReadInt32();
                spillRec[i].rsf1st = reader.ReadInt32();
                spillRec[i].rsf1end = reader.ReadInt32();
                spillRec[i].rsp1st = reader.ReadInt32();
                spillRec[i].rsp1end = reader.ReadInt32();
                spillRec[i].rss1st = reader.ReadInt32();
                spillRec[i].rss1end = reader.ReadInt32();
                spillRec[i].rat1st = reader.ReadInt32();
                spillRec[i].rat1end = reader.ReadInt32();
			}
		}
		catch (Exception ex)
		{
			return "Error: " + ex.Message;
		}
		finally
		{
			if(fs != null)
				fs.Close();
			if(reader != null)
				reader.Close();
		}
		return "LU3 file read!";
	}
	
	/*
	 *  ReadOML
	 * 
	 * Read an OML or ZML file.
	 * Takes in the path of the file, an array of OilIDs, a reference to 3D particles, and a pointer to which record in the array
	 * of OilIDs should be used.
	 * 
	 * Copied whole-cloth from code originally provided by ASA.
	 * 
	 */
	
	public static string ReadOML(string file,OilID[] recs, ref Particle3D[] parts, int trjver, int oilptr)
	{
		int fpos = 0;
		FileStream fs_oml = null;
		BinaryReader reader = null;
		try
		{
			fs_oml = File.Open(file,FileMode.Open,FileAccess.Read);
			reader = new BinaryReader(fs_oml);
			System.Array.Resize(ref parts, recs[oilptr].nRecs);
			
			fpos = (recs[oilptr].rec-1)*40;
			
			reader.BaseStream.Seek(fpos,SeekOrigin.Begin);
			
			for(int i = 0; i < recs[oilptr].nRecs;i++)
			{
				
				
				parts[i] = new Particle3D(reader.ReadSingle(),reader.ReadSingle(),reader.ReadSingle(),reader.ReadInt32(),reader.ReadSingle(),reader.ReadSingle(),reader.ReadSingle(),reader.ReadSingle(),reader.ReadSingle(),reader.ReadSingle());
			}
			reader.Close();
			fs_oml.Close();
			fs_oml.Dispose();
		}
		catch (Exception ex)
		{
			return "Error: ReadOML: " + ex.Message;
		}
		finally
		{
			if(reader != null)
				reader.Close();
			if(fs_oml != null)
			{
				fs_oml.Close();
			}
		}
		return "READ OK";
		
		
	}
	
	public static string ReadTR3(string file, SiSpillets[] recs, ref Particle3D[] parts,int recNum)
	{
		int fpos = 0;
		FileStream fs_tr3 = null;
		BinaryReader reader = null;
		try
		{
			fs_tr3 = File.Open(file,FileMode.Open,FileAccess.Read);
			reader = new BinaryReader(fs_tr3);

			System.Array.Resize(ref parts, (recs[recNum].rec2end-recs[recNum].rec2st)+1);

			fpos = (recs[recNum].rec2st-1)*40;

			reader.BaseStream.Seek(fpos,SeekOrigin.Begin);
			for(int i = 0; i < parts.Length; i++)
			{
				parts[i] = new Particle3D(reader.ReadSingle(),reader.ReadSingle(),reader.ReadSingle(),reader.ReadSingle(),reader.ReadSingle(),reader.ReadInt32(),reader.ReadSingle(),reader.ReadSingle(),reader.ReadSingle(),reader.ReadSingle());
			}
			reader.Close();
			fs_tr3.Close();
			fs_tr3.Dispose();
		}
		catch (Exception ex)
		{
			return "Error: ReadTR3: " + ex.Message;
		}
		finally
		{
			if(reader != null)
				reader.Close();
			if(fs_tr3 != null)
			{
				fs_tr3.Close();
			}
		}
		return "READ OK";
	}
	
}
