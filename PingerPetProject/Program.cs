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
            PingerCore.test();
            Console.ReadLine();
            //PingerOutputConsole.Begin(20);
        }
    }
   /*/убрать класс, тк все данные будут хранится в БД, перенести только обработку Качества
    class Host
    {
        public string hostName { get; set; }
        public byte pingIterator { get; set; }
        public bool hostStatus { get; set; }
        public int qualityLinkHost { get; set; }
        public string physLocationHost { get; set; }
        List<bool> QualityArray = new List<bool>();
        int maxSizeArrayQuality = 0;
        bool triggerFullArray = false;


        public Host(string name, string physLocation)
        {
            hostName = name;
            pingIterator = 0;
            hostStatus = true;
            physLocationHost = physLocation;
            qualityLinkHost = 0;
        }
        public int Quality(bool hostStatus)
        {
            double LinkTrue = 0;
            double LinkTrueTemp = 0;

            if (triggerFullArray == true)
            {
                if (maxSizeArrayQuality < 1000)
                {
                    QualityArray[maxSizeArrayQuality] = hostStatus;
                }
                if (maxSizeArrayQuality == 1000)
                {
                    QualityArray[maxSizeArrayQuality] = hostStatus;
                    maxSizeArrayQuality = -1;
                }
            }
            if (triggerFullArray == false)
            {
                if (QualityArray.Count < 1000)
                {
                    QualityArray.Add(hostStatus);
                }
                if (QualityArray.Count == 1000)
                {
                    triggerFullArray = true;
                    maxSizeArrayQuality = -1;
                    QualityArray.Add(hostStatus);
                }
            }
            for (int i = 0; i < QualityArray.Count; i++)
            {
                if (QualityArray[i] == true)
                {
                    LinkTrue++;
                }
            }
            double QualityArrayDouble = QualityArray.Count;
            LinkTrueTemp = LinkTrue / QualityArrayDouble * 100;
            maxSizeArrayQuality++;
            qualityLinkHost = (int)LinkTrueTemp;
            return qualityLinkHost;
        }
    }    
   */
}