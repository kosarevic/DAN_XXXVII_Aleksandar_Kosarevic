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
        public static SemaphoreSlim semaphore = new SemaphoreSlim(2, 2);
        public static List<Truck> LoadedTrucks = new List<Truck>();

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
            Thread t = new Thread(LoadTruck);

            for (int i = 1; i <= 10; i++)
            {
                t = new Thread(LoadTruck);
                t.Name = "Truck_" + i.ToString();
                t.Start();
            }

            t.Join();
            Thread.Sleep(100);

            int count = 0;

            foreach (Truck truck in LoadedTrucks)
            {
                truck.Route = bestRoutes[count];
                count++;
                Console.WriteLine(truck.Route);
            }

            for (int i = 0; i < LoadedTrucks.Count(); i++)
            {
                Thread t1 = new Thread(Destination);
                t1.Start();
            }

            Console.ReadLine();
        }

        public static void LoadTruck()
        {
            Truck truck = new Truck();
            truck.LoadTime = r.Next(500, 2000);

            semaphore.Wait();

            LoadedTrucks.Add(truck);
            Thread.Sleep(truck.LoadTime);
            Console.WriteLine(Thread.CurrentThread.Name);

            semaphore.Release();
            
        }

        public static void Destination()
        {
            Console.WriteLine();
        }
    }
}
