using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zadatak_1
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] routs = new int[1000];
            Random r = new Random();

            for (int i = 0; i < routs.Length; i++)
            {
                routs[i] = r.Next(1, 5000);
            }


        }
    }
}
