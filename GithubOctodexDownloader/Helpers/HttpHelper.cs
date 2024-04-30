using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace GithubOctodexDownloader.Helpers
{
    public class HttpHelper
    {
        static HttpWebRequest myHttpWebRequest;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="fileName">不为空的时候，表示存图</param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(string url, string fileName = "")
        {
            try
            {
                myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                using (HttpWebResponse lxResponse = (HttpWebResponse)myHttpWebRequest.GetResponse())
                {
                    var stream = lxResponse.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        FileStream lxFS = File.Create(fileName);
                        await stream.CopyToAsync(lxFS);
                        lxFS.Close();
                        lxFS.Dispose();
                        stream.Dispose();
                    }

                    string text = reader.ReadToEnd();
                    Type type = typeof(T);
                    if (type == typeof(string))
                        return (T)Convert.ChangeType(text, typeof(T));
                    else
                        return JsonConvert.DeserializeObject<T>(text);
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
