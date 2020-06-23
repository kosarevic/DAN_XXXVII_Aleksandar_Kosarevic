using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zadatak_1
{
    class Truck
    {

        public int Route { get; set; }
        public int LoadTime { get; set; }

        public Truck()
        {
        }

        public Truck(int route, int loadTime)
        {
            Route = route;
            LoadTime = loadTime;
        }
    }
}
