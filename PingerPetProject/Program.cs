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
            PingerCore pc = new PingerCore();
            pc.testInputHosts(10, "test name", "test loc");
            pc.testInputCheckingHosts(12, 1, true);
            pc.test();
            Console.ReadLine();            
        }
    }
}