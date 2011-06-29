using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Filtrete
{
    public class D3M50
    {

        #region Init
        public D3M50()
        {
        }

        public D3M50(string IPAddress)
        {
            _deviceIP = IPAddress;
        }
        #endregion

        #region Constants - Device HTTP URLs
        private const string HTTP = "http://";
        private const string URI_SYS = "/sys";
        private const string URI_TEMP = "/tstat/temp";
        private const string URI_TMODE = "/tstat/tmode";
        private const string URI_FMODE = "/tstat/fmode";
        private const string URI_OVERWRITE = "/tstat/override";
        private const string URI_HOLD = "/tstat/hold";
        private const string URI_TSTAT = "/tstat/";
        #endregion

        #region Structures
        public struct DeviceInfoDetails
        {
            public string UUID;
            public string API_Version;
            public string Firmware_Version;
            public string WLan_Fw_Version;
        }
        #endregion

        #region Enumerators
        public enum UnitofMeasure { Celsius = 0, Fahrenheit }
        public enum Mode { Off = 0, Heat, Cool, Auto }
        public enum FanMode { Auto = 0, AutoCirculate, On }
        #endregion

        #region Globals
        private UnitofMeasure _unit = UnitofMeasure.Celsius;
        private string _deviceIP = "";
        #endregion

        #region Properties

        public string DeviceIP
        {
            get
            {
                return _deviceIP;
            }
            set
            {
                _deviceIP = value;
            }
        }

        public DeviceInfoDetails DeviceInfo
        {
            get
            {
                string jsonBuffer = HttpGet(HTTP + _deviceIP + URI_SYS);
                Dictionary<string, string> parms = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonBuffer);

                DeviceInfoDetails devInfo;
                devInfo.API_Version = parms["api_version"];
                devInfo.Firmware_Version = parms["fw_version"];
                devInfo.UUID = parms["uuid"];
                devInfo.WLan_Fw_Version = parms["wlan_fw_version"];

                return devInfo;
            }
        }

        public float Temperature
        {
            get
            {
                string jsonBuffer = HttpGet(HTTP + _deviceIP + URI_TEMP);
                Dictionary<string, string> parms = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonBuffer);

                if (_unit == UnitofMeasure.Fahrenheit)
                    return float.Parse(parms["temp"]);
                else
                    return (float.Parse(parms["temp"]) - 32) / float.Parse("1.8");
            }
        }


        public Mode Thermostat
        {
            get
            {
                string jsonBuffer = HttpGet(HTTP + _deviceIP + URI_TMODE);
                Dictionary<string, string> parms = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonBuffer);

                return (Mode)int.Parse(parms["tmode"]);
            }
            set
            {
                Dictionary<string, object> parms = new Dictionary<string, object>();
                parms.Add("tmode", ((int)value));

                HttpPost(parms, HTTP + _deviceIP + URI_TMODE);
            }
        }

        public FanMode Fan
        {
            get
            {
                string jsonBuffer = HttpGet(HTTP + _deviceIP + URI_FMODE);
                Dictionary<string, string> parms = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonBuffer);

                return (FanMode)int.Parse(parms["fmode"]);
            }
            set
            {
                Dictionary<string, object> parms = new Dictionary<string, object>();
                parms.Add("fmode", ((int)value));

                HttpPost(parms, HTTP + _deviceIP + URI_FMODE);
            }
        }

        public bool Override
        {
            get
            {
                string jsonBuffer = HttpGet(HTTP + _deviceIP + URI_OVERWRITE);
                Dictionary<string, string> parms = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonBuffer);

                if (parms["hold"] == "0")
                    return false;
                else
                    return true;
            }
        }

        public bool Hold
        {
            get
            {
                string jsonBuffer = HttpGet(HTTP + _deviceIP + URI_HOLD);
                Dictionary<string, string> parms = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonBuffer);

                if (parms["hold"] == "0")
                    return false;
                else
                    return true;
            }
            set
            {
                Dictionary<string, object> parms = new Dictionary<string, object>();
                if (value == true)
                    parms.Add("hold", 1);
                else
                    parms.Add("hold", 0);

                HttpPost(parms, HTTP + _deviceIP + URI_HOLD);
            }
        }

        public float TargetHeat
        {
            get
            {
                string jsonBuffer = HttpGet(HTTP + _deviceIP + URI_TSTAT);
                Dictionary<string, string> parms = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonBuffer);

                float temp = 0;

                if (_unit == UnitofMeasure.Fahrenheit)
                    temp = (float)float.Parse(parms["t_heat"]);
                else
                    temp = (float.Parse(parms["t_heat"]) - 32) / (float)1.8;

                return temp;

            }
            set
            {
                Dictionary<string, object> parms = new Dictionary<string, object>();

                float temp = 0;

                if (_unit == UnitofMeasure.Fahrenheit)
                    temp = value;
                else
                    temp = ((value * 9) / 5) + 32;

                parms.Add("t_heat", ((float)temp));
                HttpPost(parms, HTTP + _deviceIP + URI_TSTAT);
            }
        }

        public float TargetCool
        {
            get
            {
                string jsonBuffer = HttpGet(HTTP + _deviceIP + URI_TSTAT);
                Dictionary<string, string> parms = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonBuffer);

                float temp = 0;

                if (_unit == UnitofMeasure.Fahrenheit)
                    temp = (float)float.Parse(parms["t_cool"]);
                else
                    temp = (float.Parse(parms["t_cool"]) - 32) / (float)1.8;

                return temp;
            }
            set
            {
                Dictionary<string, object> parms = new Dictionary<string, object>();
                float temp = 0;

                if (_unit == UnitofMeasure.Fahrenheit)
                    temp = value;
                else
                    temp = ((value * 9) / 5) + 32;

                parms.Add("t_cool", ((float)temp));

                HttpPost(parms, HTTP + _deviceIP + URI_TSTAT);
            }
        }

        public float AbsoluteHeat
        {
            get
            {
                string jsonBuffer = HttpGet(HTTP + _deviceIP + URI_TSTAT);
                Dictionary<string, string> parms = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonBuffer);

                float temp = 0;

                if (_unit == UnitofMeasure.Fahrenheit)
                    temp = (float)float.Parse(parms["a_heat"]);
                else
                    temp = (float.Parse(parms["a_heat"]) - 32) / (float)1.8;


                return temp;
            }
            set
            {
                Dictionary<string, object> parms = new Dictionary<string, object>();
                float temp = 0;

                if (_unit == UnitofMeasure.Fahrenheit)
                    temp = value;
                else
                    temp = ((value * 9) / 5) + 32;

                parms.Add("a_heat", ((float)temp));
                HttpPost(parms, HTTP + _deviceIP + URI_TSTAT);
            }
        }

        public float AbsoluteCool
        {
            get
            {
                string jsonBuffer = HttpGet(HTTP + _deviceIP + URI_TSTAT);
                Dictionary<string, string> parms = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonBuffer);

                float temp = 0;

                if (_unit == UnitofMeasure.Fahrenheit)
                    temp = (float)float.Parse(parms["a_cool"]);
                else
                    temp = (float.Parse(parms["a_cool"]) - 32) / (float)1.8;

                return temp;
            }
            set
            {
                Dictionary<string, object> parms = new Dictionary<string, object>();
                parms.Add("a_cool", ((float)value));

                HttpPost(parms, HTTP + _deviceIP + URI_TSTAT);
            }
        }

        public UnitofMeasure Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }

        #endregion

        #region Helpers

        private string HttpGet(string URI)
        {
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(URI);
            myReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)myReq.GetResponse();

            WebResponse response = myReq.GetResponse();

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();

            return responseFromServer;

        }

        private void HttpPost(Dictionary<string, object> Parms, string URI)
        {

            string json = ParmsToJson(Parms);
            byte[] byteArray = Encoding.UTF8.GetBytes(json);

            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(URI);
            myReq.Method = "POST";
            myReq.ContentLength = byteArray.Length;

            Stream dataStream = myReq.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            HttpWebResponse WebResp = (HttpWebResponse)myReq.GetResponse();
            WebResponse response = myReq.GetResponse();

            Stream dataStream2 = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream2);
            string responseFromServer = reader.ReadToEnd();

        }

        private string ParmsToJson(Dictionary<string, object> Parms)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            JsonWriter jsonWriter = new JsonTextWriter(sw);

            jsonWriter.WriteStartObject();

            foreach (string Key in Parms.Keys)
            {
                jsonWriter.WritePropertyName(Key);
                jsonWriter.WriteValue(Parms[Key]);
            }

            jsonWriter.WriteEndObject();

            return sb.ToString();

        }
      
        #endregion

    }
}
