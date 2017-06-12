using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace sqli1
{
    public class Gen
    {
        public HttpWebRequest httprequest;

        public bool[] mutation_approach;//3 way of mutation: behavoiur,syntax,obfuscation
        public bool[] behaviour_changing;//3 different conditions: MO_OR, MO_and, MO_semi
        public bool[] syntax_repairing;//3 different conditions: MO_par, MO_cmt, MO_qot
        public bool[] obfuscation;//6 different conditions : MO_wsp,MO_chr, MO_html, MO_per, MO_bool, MO_keyword


        public bool[] mutated_header;//3 header considered : referer,user-agant, cookie

        public double suitability;

        public Gen(/*string uri*/)
        {
            // gen = (HttpWebRequest)HttpWebRequest.Create(uri);
            mutated_header = new bool[3];

            //refer to mutation category
            mutation_approach = new bool[3];
            
            //sub methode of each mutation category
            syntax_repairing = new bool[3];
            behaviour_changing = new bool[3];
            obfuscation = new bool[4];

            for (int i=0;i<2;i++)
            {
                mutated_header[i] = false;
                mutation_approach[i] = false;
                behaviour_changing[i] = false;
                syntax_repairing[i] = false;
                obfuscation[i] = false;
            }
            obfuscation[3] = false;
            suitability = 1;
        }
    }
}
