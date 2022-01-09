using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
namespace PingerPetProject
{
    class Pinger
    {
        static void Main()
        {
            var pc = new PingerCore();
            pc.TestGetDataFromHosts(0);
            Console.Read();
        }
    }
}