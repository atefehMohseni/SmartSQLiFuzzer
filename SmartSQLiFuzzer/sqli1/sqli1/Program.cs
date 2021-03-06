﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Threading.Tasks;

namespace sqli1
{
    public class RequestManager
    {
        ///my code!
        /// 
        static void Main()
        {
            
            // Console.WriteLine(SendRequest("http://localhost:57433/Default.aspx?'", "content", "GET", null, null, true));

            //  SendRequest("http://localhost:8080/organic/", "content", "GET", null, null, true);
            
            SQLi_testcase_by_genetic sqli_testcase = new SQLi_testcase_by_genetic();

            
            string uri;

            //*  uri = "http://www.healthcabins.com/2015/01/26/ott-communication-webmail-login.html";
            uri = "http://localhost:8080/organic/";

            // uri = "http://3rtechnologies.net/contact.php?id=1";

            //  uri = "http://testfire.net/bank/login.aspx";
            // uri = "http://localhost:57433/Default.aspx?";

            int round = 2;

            int[,] statistic = new int[3, round];
            int[,] avg = new int[3,round];

           
            for (int i = 0; i < 15; i++)
            {
                statistic = sqli_testcase.generate_testcase_by_genetic(round, uri, "GET");
                for (int k = 0; k < round; k++)
                {
                    avg[0, k] += statistic[0, k];
                    avg[1, k] += statistic[1, k];
                    avg[2, k] += statistic[2, k];
                }
                statistic = new int[3, round];
            }

            for (int k = 0; k < round; k++)
            {
                avg[0, k] = avg[0, k]/10;
                avg[1, k] = avg[1, k]/10;
                avg[2, k] = avg[2, k]/10;
            }

            for (int j = 0; j < round; j++)
            {
                Console.WriteLine("#######################");
                Console.WriteLine("avg number of sqli" + avg[0, j]);
                Console.WriteLine("avg number of Not sqli" + avg[1, j]);
                Console.WriteLine("avg number of Nothing detected" + avg[2, j]);
                Console.WriteLine("*********");
            }
            // Manage_Gen mg = new Manage_Gen(uri, "GET");

            //  string b = "";
            // for (int i=0;i<10;i++)
            //   Console.WriteLine( mg.MO_chr("s"));
            //   mg.obfuscation(mg.gen_pool[0]);

            /*
             Random r = new Random();
             for (int i = 1; i < 25; i++)
             {
                 int rand = r.Next(0, 2);
                 Console.WriteLine(rand);
                 int b = r.Next(20, 60);
                 Console.WriteLine(b);
             }
             */
        }

        /// <summary>
        /// defaul program ;)
        /// </summary>
        public static string LastResponse { protected set; get; }

        static CookieContainer cookies = new CookieContainer();

        internal string GetCookieValue(Uri SiteUri, string name)
        {
            Cookie cookie = cookies.GetCookies(SiteUri)[name];
            return (cookie == null) ? null : cookie.Value;
        }

        public static string GetResponseContent(HttpWebResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }
            Stream dataStream = null;
            StreamReader reader = null;
            string responseFromServer = null;

            try
            {
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                reader = new StreamReader(dataStream);
                // Read the content.
                responseFromServer = reader.ReadToEnd();
                // Cleanup the streams and the response.
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (dataStream != null)
                {
                    dataStream.Close();
                }
                response.Close();
            }
            LastResponse = responseFromServer;
            return responseFromServer;
        }

        public HttpWebResponse SendPOSTRequest(string uri, string content, string login, string password, bool allowAutoRedirect)
        {
            HttpWebRequest request = GeneratePOSTRequest(uri, content, login, password, allowAutoRedirect);
            return GetResponse(request);
        }

        public HttpWebResponse SendGETRequest(string uri, string login, string password, bool allowAutoRedirect)
        {
            HttpWebRequest request = GenerateGETRequest(uri, login, password, allowAutoRedirect);
            return GetResponse(request);
        }

        public static HttpWebResponse SendRequest(string uri, string content, string method, string login, string password, bool allowAutoRedirect)
        {
            HttpWebRequest request = GenerateRequest(uri, content, method, login, password, allowAutoRedirect);
            //Console.WriteLine("request created");
            return GetResponse(request);
        }

        public static HttpWebRequest GenerateGETRequest(string uri, string login, string password, bool allowAutoRedirect)
        {
            return GenerateRequest(uri, null, "GET", null, null, allowAutoRedirect);
        }

        public HttpWebRequest GeneratePOSTRequest(string uri, string content, string login, string password, bool allowAutoRedirect)
        {
            return GenerateRequest(uri, content, "POST", null, null, allowAutoRedirect);
        }

        internal static HttpWebRequest GenerateRequest(string uri, string content, string method, string login, string password, bool allowAutoRedirect)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }
            
            // Create a request using a URL that can receive a post. 
            HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(uri);
            request.Headers.Add("Cookie", "hello");
            // Set the Method property of the request to POST.
            request.Method = method;

            //
            Manage_Gen mg = new Manage_Gen(uri,method);
            /*
            Gen gen = new Gen();
            gen.httprequest = request;
             request= mg.behavoiur_mutation(gen).httprequest;

            // string userag = "aaa' or 1/*";
             //request.UserAgent = userag;

            string userreferer = "http://www.yaboukur.com";
            request.Referer = userreferer;
         
            // Set cookie container to maintain cookies

            request.CookieContainer = cookies;
          //  request.CookieContainer.Add(new Uri(uri), new Cookie("id", "1234\r ' or 1=1"));
            request.AllowAutoRedirect = allowAutoRedirect;
            // If login is empty use defaul credentials
            if (string.IsNullOrEmpty(login))
            {
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
            }
            else
            {
                request.Credentials = new NetworkCredential(login, password);
            }
            if (method == "POST")
            {
                // Convert POST data to a byte array.
                byte[] byteArray = Encoding.UTF8.GetBytes(content);
                // Set the ContentType property of the WebRequest.
                request.ContentType = "application/x-www-form-urlencoded";
                // Set the ContentLength property of the WebRequest.
                request.ContentLength = byteArray.Length;
                // Get the request stream.
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();
            }
            */
            return request;
        }

        internal static HttpWebResponse GetResponse(HttpWebRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                //
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.ContentLength);
                // Console.WriteLine("response stream "+response.GetResponseStream().ToString());
                //
                cookies.Add(response.Cookies);
                // Print the properties of each cookie.
                Console.WriteLine("\nCookies: ");
                foreach (Cookie cook in cookies.GetCookies(request.RequestUri))
                {
                    Console.WriteLine("Domain: {0}, String: {1}", cook.Domain, cook.ToString());
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine("Web exception occurred. Status code: {0}", ex.Status);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return response;
        }

        public static async void waitFun(int msec)
        {
            Console.WriteLine("system is waitting khaste shod!");
            await Task.Delay(1000);
            Console.WriteLine("waked UP :D !");
        }
    }
}
