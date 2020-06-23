using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zadatak_1
{
    class Program
    {
        public static Random r = new Random();
        public static SemaphoreSlim semaphore = new SemaphoreSlim(2);

        static void Main(string[] args)
        {
            int[] routes = new int[1000];

            for (int i = 0; i < routes.Length; i++)
            {
                routes[i] = r.Next(1, 5000);
            }

            List<int> bestRoutes = new List<int>();

            for (int i = 0; i < routes.Length; i++)
            {
                if (routes[i] % 3 == 0)
                {
                    bestRoutes.Add(routes[i]);
                }
            }

            bestRoutes.Sort();

            for (int i = 1; i <= 10; i++)
            {
                Thread t = new Thread(LoadTruck);
                t.Name = "Truck_" + i.ToString();
                t.Start();
            }

            Console.ReadLine();
        }

        public static void LoadTruck()
        {
            int time = r.Next(500, 5000);

            semaphore.Wait();

            Thread.Sleep(time);
            Console.WriteLine(Thread.CurrentThread.Name);

            semaphore.Release();
        }
    }
}
