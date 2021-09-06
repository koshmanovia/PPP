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
            pc.InsertDataInHosts("four", "any_loc");
           
            //for(int i = 0; i < 11; i++)
                pc.InsertDataInCheckingHosts(0, true);

            pc.test();
            Console.ReadLine();            
        }
    }
}