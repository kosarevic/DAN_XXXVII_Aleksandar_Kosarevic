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
    /// <summary>
    /// Application simulates threading in C#, using truck deliveries as example.
    /// </summary>
    class Program
    {
        //Static variables needed for application functionality.
        public static Random r = new Random();
        public static SemaphoreSlim semaphore = new SemaphoreSlim(2, 2);
        public static List<Truck> LoadedTrucks = new List<Truck>();

        static void Main(string[] args)
        {
            //Array stores random numbers representing possible routes for truck to reach destination.
            int[] routes = new int[1000];
            //List stores 10 best routes (lowest number) for trucks to take.
            List<int> bestRoutes = new List<int>();
            //Stopwatch measures time for routes generating process.
            Stopwatch s = new Stopwatch();
            s.Start();
            //Random numbers are being generated and stored in array.
            for (int i = 0; i < routes.Length; i++)
            {
                routes[i] = r.Next(1, 5000);
            }
            //Generated numbers are being stored in text file.
            using (StreamWriter sw = new StreamWriter("..//..//Files/Routes.txt"))
            {
                foreach (int i in routes)
                {
                    sw.WriteLine(i);
                }
            }
            //Same numbers from text file are pulled back and separated by condition (n%3=0).
            string line = "";
            using (StreamReader sr = new StreamReader("..//..//Files/Routes.txt"))
            {
                //If time needed for separating process takes more than 3000ms, application stops file reading.
                while ((line = sr.ReadLine()) != null && s.Elapsed.Milliseconds < 3001)
                {
                    int route = int.Parse(line);
                    if (route % 3 == 0)
                    {
                        bestRoutes.Add(route);
                    }
                }
                //After separation, numbers are sorted in a list.
                bestRoutes.Sort();

                if (s.Elapsed.Milliseconds < 3001)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Shortest routes are generated successfully. (time: {0} ms)\n", s.ElapsedMilliseconds);
                    Console.ResetColor();
                    s.Stop();
                }
            }

            //Condition added if stopwatch exceeds 3000ms, alternative process takes place.
            if (s.Elapsed.Milliseconds > 3000)
            {
                int randomRoutes = r.Next(10, 1000);

                using (StreamReader sr = new StreamReader("..//..//Files/Routes.txt"))
                {
                    for (int i = 0; i < randomRoutes; i++)
                    {
                        line = sr.ReadLine();
                        bestRoutes.Add(int.Parse(line));
                    }

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Shortest routes are not generated successfully. (time: {0} ms)\n", s.ElapsedMilliseconds);
                    Console.ResetColor();
                    s.Stop();
                }
            }
            //Trucks loading process starts here.
            for (int j = 1; j <= 10; j++)
            {
                //Two threads are generated simultaniously, by specification.
                Thread t = new Thread(LoadTruck);
                t.Name = "Truck_" + j.ToString();
                Thread t1 = new Thread(LoadTruck);
                t1.Name = "Truck_" + (j + 1).ToString();
                //Both generated threads are initiated immediately when created.
                t.Start();
                t1.Start();
                t.Join();
                t1.Join();
                j += 1;
            }

            Console.WriteLine();

            int count = 0;
            //Best route (first 10 routes in list) is assigned to each truck in order.
            foreach (Truck truck in LoadedTrucks)
            {
                truck.Route = bestRoutes[count];
                count++;
            }
            //All trucks (in form of threads) are initiated simultaniously.
            foreach (Truck truck in LoadedTrucks)
            {
                Thread t = new Thread(() => Destination(truck));
                t.Start();
            }

            Console.ReadLine();
        }
        /// <summary>
        /// Method simulates truck loading in pairs.
        /// </summary>
        public static void LoadTruck()
        {
            //New object is made, its name and random generated load time are assigned to it.
            Truck truck = new Truck();
            truck.LoadTime = r.Next(500, 5000);
            truck.Name = Thread.CurrentThread.Name;
            //Semaphore threading control allows only 2 threads at a time to preforme following segment of code.
            semaphore.Wait();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(truck.Name + " has started loading. (load time: {0} ms)", truck.LoadTime);
            Thread.Sleep((int)truck.LoadTime);
            LoadedTrucks.Add(truck);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(truck.Name + " has been loaded.");
            Console.ResetColor();
            //Semaphore releses 2 threads when they complete process above.
            semaphore.Release();
        }
        /// <summary>
        /// Method simulates trucks destination managment.
        /// </summary>
        /// <param name="truck"></param>
        public static void Destination(Truck truck)
        {
            //Small thread sleep added for better console view.
            if (truck.Name == "Truck_10") Thread.Sleep(5);
            //Destination arrival time assigned to each truck randomly.
            int time = r.Next(500, 5000);
            Console.WriteLine(truck.Name + " has started {0} route, and will arrive at destination in: {1} ms.", truck.Route, time);
            //New line added on console to separate unrelated output.
            if (truck.Name == "Truck_10") Console.WriteLine();
            //Condition separates trucks by arrival time, as specified in project description.
            if (time <= 3000)
            {
                Thread.Sleep(time);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("{0} has successfully reached its destination and is now begining to unload. (unload time: {1} ms)", truck.Name, (int)(truck.LoadTime / 1.5));
                Thread.Sleep((int)(truck.LoadTime / 1.5));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("{0} has been successfully unloaded.", truck.Name);
            }
            else
            {
                Thread.Sleep(3001);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} has failed to deliver in time and is turning back.", truck.Name);
            }


        }
    }
}
