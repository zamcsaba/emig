using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace E_Mig
{
    public static class VonatQuery
    {
        public static List<Vonat> Result;

        public static async Task<List<Vonat>> queryByUIC(string uic)
        {
            Result = new List<Vonat>();
            if (uic.Length > 5)
            {
                foreach (Vonat v in DataConnection.vonatLista)
                {
                    if (v.UIC.Contains(uic))
                    {
                        Result.Add(v);
                    }
                }
                return Result;
            }
            else
            {
                return Result;
            }
        }
        public static async Task<List<Vonat>> queryByPSz(string psz)
        {
            Result = new List<Vonat>();
            if (psz.Length > 3)
            {
                foreach(Vonat v in DataConnection.vonatLista)
                {
                    if (psz.Length < 4)
                    {
                        if (v.Palyaszam.Split(' ')[0].Contains(psz))
                        {
                            Result.Add(v);
                        }
                    }
                    else
                    {
                        if (v.Palyaszam.Contains(psz))
                        {
                            Result.Add(v);
                        }
                    }
                }
                return Result;
            }
            else
            {
                return Result;
            }
        }
        public static async Task<List<Vonat>> queryByTrainId(string id)
        {
            if (id.Length > 2)
            {
                foreach (Vonat v in DataConnection.vonatLista)
                {
                    if (v.Vonatszam.Contains(id))
                    {
                        Result.Add(v);
                    }
                }
                return Result;
            }
            else
            {
                return Result;
            }
        }
        public static async Task<List<Vonat>> queryByLocations()
        {
            Result = new List<Vonat>();
            Geolocator loc = new Geolocator();
            Geoposition pos;
            if (await Geolocator.RequestAccessAsync() == GeolocationAccessStatus.Allowed)
            {
                pos = await loc.GetGeopositionAsync();

                var minX = pos.Coordinate.Point.Position.Latitude - 0.01;
                var maxX = pos.Coordinate.Point.Position.Latitude + 0.01;
                var minY = pos.Coordinate.Point.Position.Longitude - 0.01;
                var maxY = pos.Coordinate.Point.Position.Longitude + 0.01;

                foreach(Vonat v in DataConnection.vonatLista)
                {
                    if (v.Latitude > minX && v.Latitude < maxX && v.Longitude > minY && v.Longitude < maxY)
                    {
                        Result.Add(v);
                    }
                }
            }
            return Result;
        }
    }

    public enum QueryType
    {
        Train_ID,
        Loc_No,
        UIC,
        Location
    }
}
