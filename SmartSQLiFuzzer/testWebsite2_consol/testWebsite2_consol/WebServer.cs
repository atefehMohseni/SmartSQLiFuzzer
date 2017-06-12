using System;
using System.Net;
using System.Threading;
using System.Linq;
using System.Text;
using System.IO;


namespace testWebsite2_consol
{
    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();

        private readonly Func<HttpListenerRequest, string> _responderMethod;
                
        public WebServer(string[] prefixes, Func<HttpListenerRequest, string> method)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "Needs Windows XP SP2, Server 2003 or later.");

            // URI prefixes are required, for example 
            // "http://localhost:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // A responder method is required
            if (method == null)
                throw new ArgumentException("method");

            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);

            _responderMethod = method;
            _listener.Start();
            _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);
        }

        private static void ListenerCallback(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);

            Console.WriteLine( "New request.");

            HttpListenerContext context = listener.EndGetContext(result);
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            /*
            Console.WriteLine("cooookie : " + request.Headers["Cookie"]);
            Console.WriteLine("user-agannt:  "+ request.UserAgent);
            Console.WriteLine("referer:  " + request.UrlReferrer);
            */

           //  byte[] page = GetFile("1.html");
           // response.ContentLength64 = page.Length;

           // response.ContentLength64 = 1;//just example!

            // Stream output = response.OutputStream;
            //   output.Write(page, 0, page.Length);
            //output.Close();
            if (checkForSQLInjection(request))
                 response.Headers.Set("Sqli", "SQLi!");
            else
                response.Headers.Set("Sqli", "No SQLi!");

            // string responseString = "<HTML><BODY> H ello world!</BODY></HTML>";
            string responseString = "A";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
             output.Close();
            
            response.Close();
        }

        public static byte[] GetFile(string file)
        {
            if (!File.Exists(file)) return null;
            FileStream readIn = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[1024 * 1000];
            int nRead = readIn.Read(buffer, 0, 10240);
            int total = 0;
            while (nRead > 0)
            {
                total += nRead;
                nRead = readIn.Read(buffer, total, 10240);
            }
            readIn.Close();
            byte[] maxresponse_complete = new byte[total];
            System.Buffer.BlockCopy(buffer, 0, maxresponse_complete, 0, total);
            return maxresponse_complete;
        }

        public WebServer(Func<HttpListenerRequest, string> method, params string[] prefixes)
            : this(prefixes, method)
        { }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                string rstr = _responderMethod(ctx.Request);

                                byte[] buf = Encoding.UTF8.GetBytes(rstr);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            catch { } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }

        public static Boolean checkForSQLInjection(HttpListenerRequest request)
        {
            bool isSQLInjection = false;
            string userInput="";
            string[] sqlCheckList = { "--",
                                       ";--",
                                       ";",
                                       "/*",
                                       "*/",
                                        "@@",
                                        "@",
                                       "char",
                                       "nchar",
                                       "varchar",
                                       "nvarchar",
                                       "alter",
                                       "begin",
                                       "cast",
                                       "create",
                                       "cursor",
                                       "declare",
                                       "delete",
                                       "drop",
                                       "end",
                                       "exec",
                                       "execute",
                                       "fetch",
                                            "insert",
                                          "kill",
                                            "select",
                                           "sys",
                                            "sysobjects",
                                            "syscolumns",
                                           "table",
                                           "update"
                                       };

            ///
            /// check different hedaer of http
            /// referer, user-aganet- cookie
            ///
            for (int n = 1; n <= 3; n++)
            {
                switch(n)
                {
                    case 1:
                        if(request.UserAgent != null)
                        userInput = request.UserAgent.ToString();
                        break;
                    case 2:
                        if(request.UrlReferrer != null)
                        userInput = request.UrlReferrer.ToString();
                        break;
               //     case 3:
                 //       if(request.Cookies != null)//cookie or cookiecontainer?
                   //     userInput = request.Cookies.ToString();
                     //   break;
                }

                string CheckString="";
                if (userInput != "")
                {
                    CheckString = userInput.Replace("'", "''");
                    CheckString = userInput.Replace("\"", "\"\"");
                    for (int i = 0; i <= sqlCheckList.Length - 1; i++)
                    {
                       //  if ((CheckString.IndexOf(sqlCheckList[i],
                         //    StringComparison.OrdinalIgnoreCase) >= 0))
                      if(CheckString.Contains(sqlCheckList[i]))
                            { isSQLInjection = true; }
                    }
                }
            }
            return isSQLInjection;
        }
    }
}

