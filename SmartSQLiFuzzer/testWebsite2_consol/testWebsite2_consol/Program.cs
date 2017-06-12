using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace testWebsite2_consol
{
    class Program
    {
        static void Main(string[] args)
        {
            //WebServer ws = new WebServer(SendResponse, "http://localhost:2995/Default.aspx/");
           
            WebServer ws = new WebServer(SendResponse, "http://localhost:8080/organic/");
          
            //WebServer ws = new WebServer(SendResponse, "http://3rtechnologies.net/");

            ws.Run();
            Console.WriteLine("A simple webserver. Press a key to quit.");
            Console.ReadKey();
            ws.Stop();
            
        }

        public static string SendResponse(System.Net.HttpListenerRequest request)
        {
            return string.Format("<HTML><BODY>My web page.<br>{0}   <p><input type='submit' value='Отправить'></BODY></HTML>", DateTime.Now);

        }
    }
}
