using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace sqli1
{
    class SQLi_testcase_by_genetic
    {
        public int[,] generate_testcase_by_genetic(int round, string uri, string method)
        {

            Manage_Gen mg = new Manage_Gen(uri, method);
            
            //**********************************
            ServicePointManager.UseNagleAlgorithm = true;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.CheckCertificateRevocationList = true;
           // ServicePointManager.DefaultConnectionLimit = ServicePointManager.DefaultPersistentConnectionLimit;
            ServicePointManager.DefaultConnectionLimit =6000;//300 gen per 200 round of genetics
            //**********************************

            Gen[] children = new Gen[2];

            int[] parent_index = new int[] { -1,-1};

            HttpWebResponse response;
            int sqli = 0;
            int Nsqli = 0;
            int notset = 0;
            int[,] statistic = new int[3, round];

            //*****************************************
            for (int i = 0; i < round; i++)
            {
               // for(int c=0;c<mg.n;c++)
                mg.Mutation(mg);//mutate 30% of gens.
                

                for (int k = 0; k < mg.n ; k++)
                {
                   // Console.WriteLine(k);
                    try
                    {
                        response = mg.GetResponse(mg.gen_pool[k].httprequest);
                        //******************
                        if (response.Headers.Get("Sqli") == null)
                            notset++;
                        else if (response.Headers.Get("Sqli") == "SQLi!")
                            sqli++;
                        else if (response.Headers.Get("Sqli") == "No SQLi!")
                            Nsqli++;
                        //******************
                        mg.gen_pool[k].suitability = mg.update_suitability(k, response);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("exception accured during update suitability: " + e.ToString());
                    }
                }

                //************************************
                Console.WriteLine("number of sqli" + sqli);
                Console.WriteLine("number of Not sqli" + Nsqli);
                Console.WriteLine("number of allal badal" + notset);
                statistic[0, i] = sqli;
                statistic[1, i] = Nsqli;
                statistic[2, i] = notset;

                sqli = Nsqli = notset = 0;
                //*************************************
                //Console.WriteLine("finish getting response");

                //sort gen-pool by their suitability
                for (int j = 0; j < mg.n / 2; j++)
                {
                   // Console.WriteLine(j + "choose parent");
                    parent_index = mg.Choose_Parent();

                    // Console.WriteLine("p1: "+parent_index[0] + "  p2: "+ parent_index[1]);
                    if (parent_index[0] == -1 || parent_index[1] == -1)
                    {
                        //something wrongs happened
                        Console.WriteLine("choose parent be ga raft");
                    }
                    else
                    {
                       // Console.WriteLine("before merge parents");
                        children = mg.Merg_parents(mg.gen_pool[parent_index[0]], mg.gen_pool[parent_index[1]]);
                    }
                  //  Console.WriteLine("merginf successfully");
                    mg.update_gens(children);
                }

            }//end genetic rounds

          for (int k = 0; k < mg.n; k++)
              mg.fill_empty_header(mg.gen_pool[k]);

            Console.WriteLine("******************************************");
            for (int i = 0; i < mg.n; i++)
            {
                if (mg.gen_pool[i] == null)
                {
                    Console.WriteLine("gen  " + i + " : is empty");
                }
                else
                {
                    Console.WriteLine("gen  " + i + " : is ");
                    if (mg.gen_pool[i].mutated_header[0])
                        Console.WriteLine("referer : " + mg.gen_pool[i].httprequest.Referer);

                    if (mg.gen_pool[i].mutated_header[1])
                    Console.WriteLine("Cookie : " + mg.gen_pool[i].httprequest.Headers["Cookie"]);

                    if (mg.gen_pool[i].mutated_header[2])
                        Console.WriteLine("user-aganet : " + mg.gen_pool[i].httprequest.UserAgent);
                }
            }
            Console.WriteLine("*********");

         
            /*
            for (int i=0;i<mg.n;i++)
            {
                if(mg.gen_pool[i]!= null)
                {
                    if (mg.gen_pool[i].httprequest.Headers.Get("Sqli") == null)
                        notset++;
                    else if (mg.gen_pool[i].httprequest.Headers.Get("Sqli") == "SQLi!")
                        sqli++;
                    else if (mg.gen_pool[i].httprequest.Headers.Get("Sqli") == "No SQLi!")
                        Nsqli++;
                }
            }
            Console.WriteLine("number of sqli" + sqli);
            Console.WriteLine("number of Not sqli" + Nsqli);
            Console.WriteLine("number of allal badal" + notset);
            */

            for (int j=0;j<round;j++)
            {
                Console.WriteLine("number of sqli" + statistic[0,j]);
                Console.WriteLine("number of Not sqli" + statistic[1, j]);
                Console.WriteLine("number of allal badal" + statistic[2, j]);
                Console.WriteLine("*********");
            }

            return statistic;
        }//end function

        public int[] Analyze_Gens(Gen[] gen_pool)
        {
            int[] statistic = new int[3];

            return statistic;
        }
    }
}