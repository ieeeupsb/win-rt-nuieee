using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace NuIEEE
{
    public class API
    {
        public enum Actions {
            CheckIfSomeoneIsPresent,
            GetLastTimestamp
        }

        private const string BASE_URI = "http://nuieee.fe.up.pt/motion/";
        private const string REQUEST_FORMAT = ".php";

        public static async Task<T> GetAsync<T>(Actions action)
        {
            string url = BASE_URI + action.ToString() + REQUEST_FORMAT;

            HttpWebRequest request = (HttpWebRequest)WebRequest.CreateHttp(new Uri(url));
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";

            var postStream = await request.GetRequestStreamAsync();
            HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync();

            using (Stream streamResponse = response.GetResponseStream())
            using (StreamReader streamRead = new StreamReader(streamResponse, Encoding.UTF8))
            {
                var responseString = streamRead.ReadToEnd();

                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
                return (T) jsonSerializer.ReadObject(new MemoryStream(Encoding.Unicode.GetBytes(responseString)));
            }
        }
    }
}
