using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            List<int> bestRoutes = new List<int>();
            Stopwatch s = new Stopwatch();
            s.Start();

            for (int i = 0; i < routes.Length; i++)
            {
                routes[i] = r.Next(1, 5000);
            }

            using (StreamWriter sw = new StreamWriter("..//..//Files/Routes.txt"))
            {
                foreach (int i in routes)
                {
                    sw.WriteLine(i);
                }
            }
            
            string line = "";
            using (StreamReader sr = new StreamReader("..//..//Files/Routes.txt"))
            {
                while ((line = sr.ReadLine()) != null && s.Elapsed.Milliseconds < 3001)
                {
                    int route = int.Parse(line);
                    if (route % 3 == 0)
                    {
                        bestRoutes.Add(route);
                    }
                }
            }

            if(s.Elapsed.Milliseconds > 3000)
            {
                int randomRoutes = r.Next(10, 1000);


            }
            
            s.Stop();
            bestRoutes.Sort();

            for (int j = 1; j <= 10; j++)
            {
                Thread t = new Thread(LoadTruck);
                t.Name = "Truck_" + j.ToString();
                Thread t1 = new Thread(LoadTruck);
                t1.Name = "Truck_" + (j + 1).ToString();
                t.Start();
                t1.Start();
                t.Join();
                t1.Join();
                j += 1;
            }

            Console.WriteLine();

            int count = 0;

            foreach (Truck truck in LoadedTrucks)
            {
                truck.Route = bestRoutes[count];
                count++;
            }

            foreach (Truck truck in LoadedTrucks)
            {
                Thread t = new Thread(() => Destination(truck));
                t.Start();
            }

            Console.ReadLine();
        }

        public static void LoadTruck()
        {
            Truck truck = new Truck();
            truck.LoadTime = r.Next(500, 5000);
            truck.Name = Thread.CurrentThread.Name;

            semaphore.Wait();

            Console.WriteLine(truck.Name + " has started loading. ({0})", truck.LoadTime);
            Thread.Sleep(truck.LoadTime);
            LoadedTrucks.Add(truck);
            Console.WriteLine(truck.Name + " has been loaded.");

            semaphore.Release();

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
