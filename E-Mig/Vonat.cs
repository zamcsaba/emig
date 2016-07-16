using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace E_Mig
{
    public class VonatDetails
    {
        private int _sebesseg;

        public int Sebesseg
        {
            get { return Sebesseg; }
            set { Sebesseg = value; }
        }

        private int _uic;

        public int Uic
        {
            get { return Uic; }
            set { Uic = value; }
        }

        private int _kiinduloallomas;

        public int KiinduloAllomas
        {
            get { return KiinduloAllomas; }
            set { KiinduloAllomas = value; }
        }

        private int _celallomas;

        public int Celallomas
        {
            get { return Celallomas; }
            set { Celallomas = value; }
        }

        private int _vonatszam;

        public int Vonatszam
        {
            get { return Vonatszam; }
            set { Vonatszam = value; }
        }

        public VonatDetails()
        {
            
        }

        private Dictionary<string, string[]> _menetrend = new Dictionary<string, string[]>
        {

        };
        public  Dictionary<string, string[]> Menetrend = new Dictionary<string, string[]>
        {

        };





    }

    public class Vonat
    {
        #region Fields
        private string _uic = "";
        private string _palyaszam = "";
        private double _lat = 0;
        private double _lon = 0;
        private string _vonatszam = "";
        private string _kiindulasiAllomas = "";
        private string _celallomas = "";
        private MozdonyTipus _mozdonyTipus = MozdonyTipus.DizelEgyeb;
        private string _vonatTipus = "";
        private string _icon = "";
        private string _iconSource = null;
        #endregion

        #region Properties
        public string UIC
        {
            get { return _uic; }
            set
            {
                _uic = value;
                _palyaszam = filterUIC(value);
                _mozdonyTipus = getMT(_palyaszam);
                _iconSource = getSource();
            }
        }
        public string Palyaszam
        {
            get { return filterUIC(_uic); }
            set { _palyaszam = value; }
        }
        public string Vonatszam
        {
            get { return _vonatszam; }
            set { _vonatszam = value; }
        }
        public double Longitude
        {
            get { return _lon; }
            set { _lon = value; }
        }
        public double Latitude
        {
            get { return _lat; }
            set { _lat = value; }
        }
        public string KiinduloAllomas
        {
            get { return _kiindulasiAllomas; }
            set { _kiindulasiAllomas = value; }
        }
        public string Celallomas
        {
            get { return _celallomas; }
            set { _celallomas = value; }
        }
        public MozdonyTipus MozdonyTipus
        {
            get { return _mozdonyTipus; }
            set { _mozdonyTipus = value; }
        }
        public string VonatTipus
        {
            get { return _vonatTipus; }
            set { _vonatTipus = value; }
        }
        public Geopoint Location
        {
            get { return new Geopoint(new BasicGeoposition() { Latitude = this.Latitude, Longitude = this.Longitude }); }
        }
        public Point Anchor
        {
            get { return new Point(0.5, 0.5); }
        }
        public string Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }
        public string IconSource
        {
            get
            {
                return getSource();
            }
        }
        #endregion

        //Helpers
        string filterUIC(string uic)
        {
            string str = uic.Remove(0, 4).Remove(7, 1);
            str = String.Format("{0} {1}", str.Substring(0, 4), str.Substring(4));
            if (str.StartsWith("0")) str = str.Remove(0, 1);
            return str;
        }
        string getSource()
        {
            string src = @"Images\";
            switch (_mozdonyTipus)
            {
                case MozdonyTipus.Dizel:
                    src += "Red" + "_" + _vonatTipus + "_" + getAngle().ToString() + ".png";
                    break;
                case MozdonyTipus.DizelMotor:
                    src += "Orange" + "_" + _vonatTipus + "_" + getAngle().ToString() + ".png";
                    break;
                case MozdonyTipus.DizelEgyeb:
                    src += "BG" + "_" + _vonatTipus + "_" + getAngle().ToString() + ".png";
                    break;
                case MozdonyTipus.ElektromosRegi:
                    src += "Blue" + "_" + _vonatTipus + "_" + getAngle().ToString() + ".png";
                    break;
                case MozdonyTipus.ElektromosUj:
                    src += "Grey" + "_" + _vonatTipus + "_" + getAngle().ToString() + ".png";
                    break;
                case MozdonyTipus.ElektromosMotor:
                    src += "Grey" + "_" + _vonatTipus + "_" + getAngle().ToString() + ".png";
                    break;
                case MozdonyTipus.ElektromosEgyeb:
                    src += "Grey" + "_" + _vonatTipus + "_" + getAngle().ToString() + ".png";
                    break;
                case MozdonyTipus.ElektromosSzocskeGigant:
                    src += "Blue" + "_" + _vonatTipus + "_" + getAngle().ToString() + ".png";
                    break;
                case MozdonyTipus.FlirtTalent:
                    src += "Yellow" + "_" + _vonatTipus + "_" + getAngle().ToString() + ".png";
                    break;
                default:
                    return @"Images\BaseIcon_0.png";
            }
            return src;
        }
        string getAngle()
        {
            if (_icon != "z-z-a" && _icon != "z-l-a" && _icon != "z-p-a" && _icon != "p-z-a" && _icon != "l-z-a")
            {
                string s = _icon.Split('-')[2];
                if (s == "000")
                {
                    return "0";
                }
                if (s.StartsWith("0"))
                {
                    s = s.Remove(0, 1);
                }
                return s;
            }
            else return "0";
        }
        MozdonyTipus getMT(string psz)
        {
            string psz1 = psz.Trim().Split(' ')[0];

            switch (psz1)
            {
                case "431":
                    return MozdonyTipus.ElektromosRegi;
                case "432":
                    return MozdonyTipus.ElektromosRegi;
                case "433":
                    return MozdonyTipus.ElektromosRegi;
                case "470":
                    return MozdonyTipus.ElektromosUj;
                case "480":
                    return MozdonyTipus.ElektromosUj;
                case "460":
                    return MozdonyTipus.ElektromosSzocskeGigant;
                case "630":
                    return MozdonyTipus.ElektromosSzocskeGigant;

                case "408":
                    return MozdonyTipus.Dizel;
                case "418":
                    return MozdonyTipus.Dizel;
                case "438":
                    return MozdonyTipus.Dizel;
                case "448":
                    return MozdonyTipus.Dizel;
                case "478":
                    return MozdonyTipus.Dizel;
                case "628":
                    return MozdonyTipus.Dizel;
                case "8276": //Mk45
                    return MozdonyTipus.Dizel;


                case "0414":
                    return MozdonyTipus.ElektromosMotor;
                case "0424":
                    return MozdonyTipus.ElektromosMotor;
                case "434":
                    return MozdonyTipus.ElektromosMotor;
                case "2105":
                    return MozdonyTipus.ElektromosMotor;
                case "8076":
                    return MozdonyTipus.ElektromosMotor;
                case "8005": //bdt vezérlő
                    return MozdonyTipus.ElektromosMotor;
                case "8007": //bdt vezérlő
                    return MozdonyTipus.ElektromosMotor;
                case "8027": //bdt vezérlő
                    return MozdonyTipus.ElektromosMotor;
                case "8028": //btzx vezérlő
                    return MozdonyTipus.ElektromosMotor;
                case "8055": //bydtee vezérlő
                    return MozdonyTipus.ElektromosMotor;
                case "8207": //bmxtz
                    return MozdonyTipus.ElektromosMotor;
                case "8227": //bdt vezérlő
                    return MozdonyTipus.ElektromosMotor;
                case "1136": //ikerBZ
                    return MozdonyTipus.DizelMotor;
                case "1416": //uzsgyi
                    return MozdonyTipus.ElektromosMotor;
                case "1426": //desiro
                    return MozdonyTipus.ElektromosMotor;


                case "127": //bzmot
                    return MozdonyTipus.DizelMotor;


                case "5341":
                    return MozdonyTipus.FlirtTalent;
                case "1415":
                    return MozdonyTipus.FlirtTalent;

                case "5342":
                    return MozdonyTipus.FlirtTalent;

                
                case "117": //bézé (0117)
                    return MozdonyTipus.DizelMotor;
               
                    
                default:
                    return MozdonyTipus.ElektromosEgyeb;
            }
        }
        public int Index
        {
            get;
            set;
        }
    }

   


    public enum MozdonyTipus
    {
        Dizel,
        DizelMotor,
        DizelEgyeb,
        ElektromosRegi,
        ElektromosUj,
        ElektromosMotor,
        ElektromosSzocskeGigant,
        ElektromosEgyeb,
        FlirtTalent
    }

}
