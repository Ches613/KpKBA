using Scada.Comm.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KBAConnTest
{
    class Program
    {

        private static Laser laser;
        private static StatusPack laserStat;


        static void Main(string[] args)
        {

             laser = new Laser("10.7.66.127", 3490);

             laserStat = new StatusPack();

            while (true) {


               
                   laserStat = laser.getStatus();

                   Console.WriteLine("UM = " + laser.reqActualNum(1));
                    Console.WriteLine("okPrintCount = " + laserStat.okPrintCount);

                Console.ReadKey();
               // Thread.Sleep(1000);

            }

        }
    }
}
