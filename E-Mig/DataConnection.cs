using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.Networking.Connectivity;

namespace E_Mig
{
    public class DataConnection
    {
        public static List<Vonat> vonatLista = new List<Vonat>();
        static StringBuilder vonatokHtml = new StringBuilder();
        static string sessionId;
        static string sqlId;

        public static async Task<string> getSessionId()
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://iemig.mav-trakcio.hu/netr/emig.aspx/");
            request.Headers.Date = DateTime.Now.Subtract(new TimeSpan(10, 0, 0));
            request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.1; WOW64; rv:11.0) Gecko/20100101 Firefox/11.0");
            HttpResponseMessage response = await client.SendAsync(request);
            string s = await response.Content.ReadAsStringAsync();
            if (s != null)
            {
                Match m = new Regex(@"gSessionId=(.*?);").Match(s);
                if (m.Success)
                {
                    sessionId = m.Groups[1].ToString();
                    return sessionId;
                }
                else return "";
            }
            else return "";
        }
        static async Task getSqlId(string s)
        {
            Object obj;
            Object obj1;
            obj = new Uri(@"http://iemig.mav-trakcio.hu/netr/emig.aspx");
            obj1 = new Dictionary<string, string>();
            ((Dictionary<string, string>)obj1).Add("u", "public");
            ((Dictionary<string, string>)obj1).Add("s", s); //s!!
            ((Dictionary<string, string>)obj1).Add("t", "publicsandr");
            ((Dictionary<string, string>)obj1).Add("q", "Q5");
            ((Dictionary<string, string>)obj1).Add("lt", "SqlCreate");
            ((Dictionary<string, string>)obj1).Add("w", "null");
            ((Dictionary<string, string>)obj1).Add("c", "null");
            ((Dictionary<string, string>)obj1).Add("o", "null");
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> i in (Dictionary<string, string>)obj1)
            {
                KeyValuePair<string, string> entry = new KeyValuePair<string, string>();
                entry = i;
                if (sb.Length != 0) { sb.Append('&'); }
                sb.Append(WebUtility.UrlEncode(i.Key));
                sb.Append('=');
                sb.Append(WebUtility.UrlEncode(i.Value));
            }
            obj1 = null;

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://iemig.mav-trakcio.hu/netr/emig.aspx" + "?" + sb.ToString());
            request.Headers.Date = DateTime.Now.Subtract(new TimeSpan(10, 0, 0));
            request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.1; WOW64; rv:11.0) Gecko/20100101 Firefox/11.0");
            request.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            request.Headers.AcceptEncoding.ParseAdd("gzip, deflate");
            request.Headers.AcceptLanguage.ParseAdd("hu,en-us;q=0.7,en;q=0.3");
            request.Headers.Referrer = new Uri("http://iemig.mav-trakcio.hu/5.0/index.html");
            HttpResponseMessage response = await client.SendAsync(request);


            string xy = await response.Content.ReadAsStringAsync();
            if (!String.IsNullOrEmpty(xy))
            {
                Match mtch = new Regex("<sqlid>(.*?)</sqlid>").Match(xy);
                if (mtch.Success)
                {
                    sqlId = mtch.Groups[1].ToString();
                }
            }
        }
        static async Task VonatBetoltes(string sess, string sql)
        {
            StringBuilder req = new StringBuilder();
            vonatokHtml = new StringBuilder();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("u", "public");
            dict.Add("s", sess);
            dict.Add("t", "publicrspec");
            dict.Add("q", sql);
            dict.Add("f", "publicmlist");

            foreach (KeyValuePair<string, string> i in dict)
            {
                if (req.Length != 0) { req.Append('&'); }
                req.Append(WebUtility.UrlEncode(i.Key));
                req.Append('=');
                req.Append(WebUtility.UrlEncode(i.Value));
            }
  
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://iemig.mav-trakcio.hu/netr/emig.aspx" + "?" + req.ToString());
            request.Headers.Date = DateTime.Now.Subtract(new TimeSpan(10, 0, 0));
            request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.1; WOW64; rv:11.0) Gecko/20100101 Firefox/11.0");
            request.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            request.Headers.AcceptEncoding.ParseAdd("gzip, deflate");
            request.Headers.AcceptLanguage.ParseAdd("hu,en-us;q=0.7,en;q=0.3");
            request.Headers.Referrer = new Uri("http://iemig.mav-trakcio.hu/5.0/index.html");
            HttpResponseMessage response = await client.SendAsync(request);
            vonatokHtml.Append(await response.Content.ReadAsStringAsync());
        }
        static void VonatListaLoad()
        {
            vonatLista = new List<Vonat>();
            MatchCollection match = new Regex("<Mozdony (.*?)></Mozdony>").Matches(vonatokHtml.ToString());
            int count = 0;
            foreach (Match m in match)
            {
                Vonat vonat = new Vonat();
                string str = m.Groups[1].ToString();
                Match m1 = new Regex("lat=\"(.*?)\"").Match(str);
                vonat.Latitude = Convert.ToDouble(m1.Groups[1].ToString()) / 1000000d;
                m1 = new Regex("lng=\"(.*?)\"").Match(str);
                vonat.Longitude = Convert.ToDouble(m1.Groups[1].ToString()) / 1000000d;
                m1 = new Regex("vonatszam=\"(.*?)\"").Match(str);
                vonat.Vonatszam = m1.Groups[1].ToString();
                m1 = new Regex("Induló állomás:</td><td>(.*?)<").Match(str);
                if (m1.Success) vonat.KiinduloAllomas = m1.Groups[1].ToString();
                m1 = new Regex("Érkező állomás:</td><td>(.*?)<").Match(str);
                if (m1.Success) vonat.Celallomas = m1.Groups[1].ToString();
                m1 = new Regex("icon=\"(.*?)\"").Match(str);
                if (m1.Success) vonat.Icon = m1.Groups[1].ToString();
                m1 = new Regex("tipus=\"(.*?)\"").Match(str);
                if (m1.Success) vonat.VonatTipus = m1.Groups[1].ToString();
                m1 = new Regex("uic=\"(.*?)\"").Match(str);
                vonat.UIC = m1.Groups[1].ToString();
                vonat.Index = count;
                vonatLista.Add(vonat);
                count++;
            }
        }
        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
        //static async Task<List<Vonat>> loadMetrans()
        //{
        //    List<Vonat> meTrains = new List<Vonat>();
        //    object o = HttpWebRequest.CreateHttp("http://poloha.vozu.cz/init-vehicles/public");
        //    ((HttpWebRequest)o).Method = "GET";
        //    ((HttpWebRequest)o).Headers["UserAgent"] = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36";
        //    object o2 = await ((HttpWebRequest)o).GetResponseAsync();
        //    string data = await new StreamReader(((HttpWebResponse)o2).GetResponseStream()).ReadToEndAsync();
            

        //    Regex r = new Regex("");
        //}

        public static async Task<List<Vonat>> Vonatok()
        {
            await getSessionId();
            await getSqlId(sessionId);
            await VonatBetoltes(sessionId, sqlId);
            VonatListaLoad();
            vonatokHtml = null;
            return vonatLista;
        }
    }
}
