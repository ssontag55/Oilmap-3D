using System;
using System.Collections.Generic;
using System.Text;

namespace ASA.OIL.DRAW//ASA.SPILL.DRAW
{
    public class OilTrackDpt
    {
        public float x;
        public float y;
        public DateTime timestamp;
        public float entrain;
        public float evapor;
        public float surface;
        public float land;

        public OilTrackDpt()
        {
        }

        public OilTrackDpt(float X, float Y)
        {
            x = X;
            y = Y;
        }

        public OilTrackDpt(float X, float Y, DateTime TimeStamp)
        {
            x = X;
            y = Y;
            timestamp = TimeStamp;
        }
        public OilTrackDpt(float X, float Y, DateTime TimeStamp, float Entrain, float Evapor, float Surface, float Land)
        {
            x = X;
            y = Y;
            timestamp = TimeStamp;
            entrain = Entrain;
            surface = Surface;
            land = Land;
        }

    }
    public class Oilidx
    {
        public int rec;
        public int time;
        public int nrecs;

        public Oilidx(int Rec, int Time, int Nrecs)
        {
            rec = Rec;
            time = Time;  //Minutes from 12/31/1979 00:00 
            nrecs = Nrecs;
        }

        public Oilidx()
        {

        }

    }
    public class oilThickness
    {
        public Int16 ncrecs = 0;
        public Int16 ncvals = 0;
        public Int16 imaxoil = 1;
        public Int16 jmaxoil = 1;
        public float olonoil = 0F;
        public float olatoil = 0F;
        public float dlonoil = 0F;
        public float dlatoil = 0F;
        public int sTime = 0;
        public float rval = 0F;

        public oilThickness()
        {

        }

        public oilThickness(Int16 o_ncrecs, Int16 o_ncvals, Int16 o_imaxoil, Int16 o_jmaxoil, float o_olonoil, float o_olatoil,
            float o_dlonoil, float o_dlatoil, int o_sTime, float o_rval)
        {
            ncrecs = o_ncrecs;
            ncvals = o_ncvals;
            imaxoil = o_imaxoil;
            jmaxoil = o_jmaxoil;
            olonoil = o_olonoil;
            olatoil = o_olatoil;
            dlonoil = o_dlonoil;
            dlatoil = o_dlatoil;
            sTime = o_sTime;
            rval = o_rval;
        }
    }



    public class contobj
    {
        public Int16 i = 0;
        public Int16 j = 0;
        public float mass = 0F;

        public contobj()
        {
        }

        public contobj(Int16 o_i, Int16 o_j, float o_mass)
        {
            j = o_j;
            i = o_i;
            mass = o_mass;
        }

    }

    public class oilobject
    {

        public Single lon;
        public Single lat;
        public Single z;
        public int nwhere;
        public Single mass;
        public Single radius;
        public Single thickness;
        public Single viscosity;
        public Single watercontent;
        public Single flashpoint;

        public oilobject()
        {
        }

        public oilobject(Single o_lon, Single o_lat, Single o_z, int o_nwhere, Single o_mass, Single o_radius,
            Single o_thickness, Single o_viscosity, Single o_watercontent, Single o_flashpoint)
        {
            lon = o_lon;
            lat = o_lat;
            z = o_z;
            nwhere = o_nwhere;
            mass = o_mass;
            radius = o_radius;
            thickness = o_thickness;
            viscosity = o_viscosity;
            watercontent = o_watercontent;
            flashpoint = o_flashpoint;
        }

    }

    public class TrajLine
    {
        public float lat;
        public float lon;
        public DateTime timestamp;
        public int elapmin;

        public TrajLine()
        {
        }

        public TrajLine(float Lat, float Lon, DateTime TimeStamp)
        {
            lat = Lat;
            lon = Lon;
            timestamp = TimeStamp;

        }
    }
}
