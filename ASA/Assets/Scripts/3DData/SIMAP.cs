using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using ASA.OIL.DRAW;
//using ASAMAP.COMMON;

namespace ASAMAP.IO
{

  


    public  class SIMAP
    {

        private string _infile = string.Empty;

        public SIMAP(string ifile)
        {
            _infile = ifile;
        }

        public string ReadSPillData(DateTime dt, string infile,  ref ArrayList trjln, ref ArrayList oilobj, ref ArrayList oilcns, ref ArrayList thkns, int model_id)
        {             
      //     SIMAP_HC3File.ConcGrid.GetConcData(string HC3Path, int timeStep);

           SIMAP_HC3File.ConcGrid con = new SIMAP_HC3File.ConcGrid();
         //  con = GetConcData(_infile + ".hc3",  2774, dt);
           con = GetConcData(_infile + ".hc3",  dt);
           oilcns = new ArrayList();
         
        //   thkns[] thnk = new thkns[1];
           contobj[] cntobj1 = new contobj[1];
           oilThickness oilthk = new oilThickness();
       //    System.Array.Resize(ref oilthk, con.Records.Length);
            System.Array.Resize(ref cntobj1, con.Records.Length);

            double originalX;
            double originaly;
            originalX = con.OriginX;
            originaly = con.OriginY;

            int numx = con.GridX;
            int numy = con.GridY;

            double dx = con.CellWidth;
            double dy = con.CellHeight;

            oilthk = new oilThickness((short)0, (short)0, (short)numx, (short) numy, (float)originalX, (float)originaly, (float)dx, (float)dy, 1000, 0.1f); 
 
           thkns.Add(oilthk);
            for (int i = 0; i < con.Records.Length; i++)
            {

     
                cntobj1[i] = new contobj((short) con.Records[i].i, (short) con.Records[i].j, con.Records[i].max);
                oilcns.Add(cntobj1);    
               
            }
      
            string result = "OK";
            
            int nsteps = 0;


            //Oilidx[] oilidx = new Oilidx[1];
            //TrajLine[] trjlin = new TrajLine[1];
            //try
            //{

            //    infile = Path.ChangeExtension(infile, ".omp");
            //    if (File.Exists(infile))
            //        result = READOMP(infile, ref oilidx);
            //    else
            //    {
            //        result = infile + " doesn't exist";
            //        return result;
            //    }

            //    if (result != "OK")
            //        return result;

            //    nsteps = oilidx.GetLength(0);


            //    infile = Path.ChangeExtension(infile, ".oml");
            //    if (File.Exists(infile))
            //        result = READOML(infile, oilidx, ref trjln, ref oilobj);
            //    else
            //    {
            //        result = infile + " doesn't exist";
            //        return result;
            //    }

            //    if (result != "OK")
            //        return result;

            //    if (model_id != 2)
            //    {
            //        infile = Path.ChangeExtension(infile, ".omc");

            //        if (File.Exists(infile))
            //            result = READOMC(infile, nsteps, ref oilcns, ref thkns);
            //        else
            //        {
            //            result = infile + " doesn't exist";
            //            return "OK for OML, OMP, but  no OMC";
            //        }

            //        if (result != "OK")
            //            return "OK for OML, OMP, but  no OMC";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    result = "ERROR: Main Model Read: " + ex.Message;
            //}
            return result;
        }


        public static SIMAP_HC3File.ConcGrid GetConcData(string HC3Path, DateTime dt)
        {
            SIMAP_IN3File IN3 = new SIMAP_IN3File(Path.ChangeExtension(HC3Path, "IN3"));
            object value = IN3.GetValue(SIMAP_IN3File.KEY_NPLUMECELLSZ);

            if (value == null)
                return null;
           //   SIMAP_HC3File hc3 = new SIMAP_HC3File(HC3Path, 1, dt);
          SIMAP_HC3File hc3 = new SIMAP_HC3File(HC3Path, (int)value, dt);
            return hc3.ReadTimeStep(hc3.timestep);
        }
    }

    public class SIMAP_HC3File
    {
        private static object _threadSafe = new object();
        private const int CONC_FACTOR = -10000;
        private SIMAP_LU3File _LU3;
        private string _path;
        private int _zCells;
        private int _timestep;  //xp added
       
        public int timestep
        {


            get { return _timestep; }

        }


        public struct ConcRecord
        {
            public int i;
            public int j;
            public int mean;
            public int max;
            public int[] conc;
        }

        public class ConcGrid
        {
            public int GridX { get; set; }
            public int GridY { get; set; }
            public double OriginX { get; set; }
            public double OriginY { get; set; }
            public double CellWidth { get; set; }
            public double CellHeight { get; set; }
            public ConcRecord[] Records { get; set; }
        }


    

        public SIMAP_HC3File(string path, int zCells, DateTime dt)
        {
            _path = path;
            _zCells = zCells;
            _LU3 = new SIMAP_LU3File(Path.ChangeExtension(path, "LU3"), dt);
            _timestep = _LU3.timestep;
        }

        protected int CalcConc(int conc)
        {
            return (conc < 0) ? conc * CONC_FACTOR : conc;
        }

        public int GetNumTimeSteps()
        {
            return _LU3.GetNumTimeSteps();
        }

        public ConcGrid ReadTimeStep(int timeStep)
        {
            if (timeStep < 1 || timeStep > _LU3.GetNumTimeSteps())
                return null;

            ConcGrid conc = new ConcGrid();
            int offset = 4 + _zCells / 4;
            int numBytes = 2 * _zCells + 8;
            int recIndex = _LU3.GetRecordIndex(timeStep);
            int numPoints = _LU3.GetNumPoints(timeStep, offset);
            conc.Records = new ConcRecord[numPoints];

            //lock (_threadSafe)
            //{
				BinaryReader reader = new BinaryReader(File.Open(_path,FileMode.Open,FileAccess.Read,FileShare.Read));
                //using (BinaryReader reader = new BinaryReader(Stream.Synchronized(
                //    new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read))))
                //{
                    reader.BaseStream.Position = (recIndex++ - 1) * numBytes;
                    conc.GridX = reader.ReadInt16();
                    conc.GridY = reader.ReadInt16();

                    reader.BaseStream.Position = (recIndex++ - 1) * numBytes;
                    conc.OriginX = reader.ReadSingle();
                    conc.OriginY = reader.ReadSingle();

                    reader.BaseStream.Position = (recIndex++ - 1) * numBytes;
                    conc.CellWidth = reader.ReadSingle();
                    conc.CellHeight = reader.ReadSingle();

                    reader.BaseStream.Position = (recIndex + offset) * numBytes;

                    for (int n = 0; n < numPoints; n++)
                    {
                        conc.Records[n].i = reader.ReadInt16();
                        conc.Records[n].j = reader.ReadInt16();
                        conc.Records[n].mean = CalcConc(reader.ReadInt16());
                        conc.Records[n].max = CalcConc(reader.ReadInt16());
                        conc.Records[n].conc = new int[_zCells];
                        for (int z = 0; z < _zCells; z++)
                            conc.Records[n].conc[z] = CalcConc(reader.ReadInt16());
                    }

                    reader.BaseStream.Close();
                    reader.Close();
                //}
            //}
            return conc;
        }
    }

    public class SIMAP_IN3File
    {
        private static object _threadSafe = new object();
        private Dictionary<string, object> _data = new Dictionary<string, object>();

        public const string KEY_NPLUMECELLSZ = "NPLUMECELLSZ";

        public SIMAP_IN3File(string path)
        {
            int intVal;
            double dblVal;

            lock (_threadSafe)
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                    {
                        string[] tokens = reader.ReadLine().Split('=');

                        if (tokens.Length > 1)
                        {
                            string key = tokens[0].Trim().ToUpper();
                            string value = tokens[1].Trim();

                            if (Int32.TryParse(value, out intVal))
                            {
                                _data.Add(key, intVal);
                            }
                            else if (Double.TryParse(value, out dblVal))
                            {
                                _data.Add(key, dblVal);
                            }
                            else
                            {
                                _data.Add(key, value);
                            }
                        }
                    }
                }
            }
        }

        public object GetValue(string key)
        {
            string cleanKey = key.Trim().ToUpper();

            if (_data.ContainsKey(cleanKey))
                return _data[cleanKey];

            return null;
        }
    }

    public class SIMAP_LU3File
    {
        private int _timestep = -9999;  //xp added
        public int timestep
        {
            get { return _timestep; }
        }

        private static object _threadSafe = new object();
        protected const long RECORD_LENGTH = 96; // bytes
        protected struct Record
        {
            public int iver;
            public int simtime;
            public float time;
            public int rec1st;
            public int rec2st;
            public int rec2end;
            public int rec3st;
            public int rec3end;
            public int rec5st;
            public int rec5end;
            public int sed2st;
            public int sed2end;
            public int rar1st;
            public int rar1end;
            public int rth1st;
            public int rth1end;
            public int rsf1st;
            public int rsf1end;
            public int rsp1st;
            public int rsp1end;
            public int rss1st;
            public int rss1end;
            public int rat1st;
            public int rat1end;
        }

        private Record[] _recs;

        public SIMAP_LU3File(string path, DateTime dt)
        {


            ///////
            DateTime baseTime = DateTime.Parse("12/31/1979 00:00");
         //  DateTime newt = baseTime.AddMinutes(15941430);
           long totalstep = 0;
            TimeSpan tsp = dt - baseTime;
            double totalmin = tsp.TotalMinutes; 
      

            ////////
            //lock (_threadSafe)
            //{
                BinaryReader reader = new BinaryReader(File.Open(path,FileMode.Open, FileAccess.Read,FileShare.Read));
                //using (BinaryReader reader = new BinaryReader(Stream.Synchronized(
                    //new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))))
                //{
                    long recCount = reader.BaseStream.Length / RECORD_LENGTH;
                    totalstep = recCount; 
                    _recs = new Record[recCount];

                    for (int n = 0; n < recCount; n++)
                    {
                        _recs[n].iver = reader.ReadInt32();
                        _recs[n].simtime = reader.ReadInt32();
                        _recs[n].time = reader.ReadSingle();
                        _recs[n].rec1st = reader.ReadInt32();
                        _recs[n].rec2st = reader.ReadInt32();
                        _recs[n].rec2end = reader.ReadInt32();
                        _recs[n].rec3st = reader.ReadInt32();
                        _recs[n].rec3end = reader.ReadInt32();
                        _recs[n].rec5st = reader.ReadInt32();
                        _recs[n].rec5end = reader.ReadInt32();
                        _recs[n].sed2st = reader.ReadInt32();
                        _recs[n].sed2end = reader.ReadInt32();
                        _recs[n].rar1st = reader.ReadInt32();
                        _recs[n].rar1end = reader.ReadInt32();
                        _recs[n].rth1st = reader.ReadInt32();
                        _recs[n].rth1end = reader.ReadInt32();
                        _recs[n].rsf1st = reader.ReadInt32();
                        _recs[n].rsf1end = reader.ReadInt32();
                        _recs[n].rsp1st = reader.ReadInt32();
                        _recs[n].rsp1end = reader.ReadInt32();
                        _recs[n].rss1st = reader.ReadInt32();
                        _recs[n].rss1end = reader.ReadInt32();
                        _recs[n].rat1st = reader.ReadInt32();
                        _recs[n].rat1end = reader.ReadInt32();

                        if (n > 1 && n < recCount)
                        {
                            if (_recs[n - 1].simtime <= totalmin && _recs[n].simtime >= totalmin)
                            {
                                if (totalmin - _recs[n - 1].simtime > _recs[n].simtime - totalmin)
                                    _timestep = n - 1;
                                else
                                    _timestep = n;
                            } 
                        }
                    }
                    //temp
                    if (totalmin > _recs[recCount - 1].simtime)
                        _timestep = Convert.ToInt32( totalstep);
                    else if (totalmin < _recs[0].simtime)
                        _timestep = 0;                 
                    reader.BaseStream.Close();
                    reader.Close();
                //}
            //}
        }

        public int GetRecordIndex(int timeStep)
        {
			
            return _recs[timeStep].rth1st;
        }

        public int GetNumPoints(int timeStep, int count)
        {
			
            return _recs[timeStep].rth1end - _recs[timeStep].rth1st - count + 1;
        }

        public int GetNumTimeSteps()
        {
            return _recs.Length;
        }
    }
}
