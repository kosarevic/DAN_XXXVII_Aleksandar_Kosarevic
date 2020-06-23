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
        public static object TheLock = new object();
        public static int counter = 0;

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

            for (int j = 1; j <= 10; j++)
            {
                t = new Thread(LoadTruck);
                t.Name = "Truck_" + j.ToString();
                t.Start();
            }

            while (counter != 10) { }

            int count = 0;

            foreach (Truck truck in LoadedTrucks)
            {
                truck.Route = bestRoutes[count];
                count++;
            }


            foreach (Truck truck in LoadedTrucks)
            {
                Thread t1 = new Thread(() => Destination(truck));
                t1.Start();
            }



            Console.ReadLine();
        }

        public static void LoadTruck()
        {
            Truck truck = new Truck();
            truck.LoadTime = r.Next(500, 5000);
            truck.Name = Thread.CurrentThread.Name;

            while (true)
            {
                if (counter % 2 == 0 && semaphore.CurrentCount == 2 || counter == 0)
                {
                    semaphore.Wait();

                    Console.WriteLine(truck.Name + " has started loading. ({0})", truck.LoadTime);
                    Thread.Sleep(truck.LoadTime);
                    LoadedTrucks.Add(truck);
                    Console.WriteLine(truck.Name + " has been loaded.");

                    semaphore.Release();

                    break;
                }
            }
            counter++;
        }

        public static void Destination(Truck truck)
        {
            int time = r.Next(500, 5000);
            Console.WriteLine(truck.Name + " has started {0} route, and will arrive at destination in: {1}", truck.Route, time);

            if (time <= 3000)
            {
                Thread.Sleep(time);
                Console.WriteLine("{0} has successfully reached its destination and is now begining to unload.", truck.Name);
                Thread.Sleep(truck.LoadTime / 2);
                Console.WriteLine("{0} has been successfully unloaded.", truck.Name);
            }
            else
            {
                Thread.Sleep(3001);
                Console.WriteLine("{0} has failed to deliver in time and is turning back.", truck.Name);
            }


        }
    }
}
