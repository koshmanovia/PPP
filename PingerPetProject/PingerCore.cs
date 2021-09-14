using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace PingerPetProject
{
    #region инициализация таблиц
    [Table(Name = "Hosts")]
    internal class Hosts
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, Name = "hostID")]
        public int hostID { get; set; }
        
        [Column]
        public string hostName { get; set; }
        
        [Column]
        public string physLocationHost { get; set; }
    }
    [Table(Name = "CheckingHosts")]
    internal class CheckingHosts
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int iteration_num { get; set; }
        
        [Column(Name = "hostID")]
        public int hostID { get; set; }
        
        [Column]
        public bool hostStatus { get; set; }
    }
    #endregion
    class PingerCore
    {
        private Ping Pinger = new Ping();
        private PingOptions options = new PingOptions(128, dontFragment:true);//перенести в конструкторы управление ttl
        private int timeOutHostPing = 3000;//перенести в конструкторы управление временем пинга

        private static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\AppData\Host.mdf;Integrated Security=True";
        private static DataContext db = new DataContext(connectionString);
        private Table<Hosts> Hosts = db.GetTable<Hosts>();
        private Table<CheckingHosts> CheckingHosts = db.GetTable<CheckingHosts>();

        private bool needInsertHosts = true;

        private Thread connectionsLiveMonitor = default;

        //сделать нормальный конструктор
        public List<(string hostName, string ipAddress, ushort Ping, string Description)> massPing (List<(string HostName,string physLocationHost)> hostsLoop)//Переписать метод чтобы целял данные из конструктора
        {
            var returnList = new List<(string hostName, string ipAddress, ushort Ping, string Description)>();
            if (needInsertHosts != false)
            {
                InsertDataInHosts(hostsLoop);
                needInsertHosts = false;
            }

            for (int i = 0; i < hostsLoop.Count; i++)
            {
                (string, bool) tepmPingData;
                connectionsLiveMonitor = new Thread(new ThreadStart(CheckingNetConnetions)); //проверка во время выполнения пинга, есть ли сетевое соединение
                connectionsLiveMonitor.Start();
                tepmPingData = Ping(hostsLoop[i].HostName);//придумать как пропинговать все адреса и правильно заполнить бд и возвращаемый List
                InsertDataInCheckingHosts(i,tepmPingData.Item2);
            }
            return returnList;
        }
        private (string ,bool) Ping(string HostName)
        {
            string ipAddress = default;
            bool positivePing = default;
            (string, bool) ret = (default, default);
            try
            {
                PingReply replyInputDataHost = Pinger.Send(HostName, timeOutHostPing);
                if (replyInputDataHost.Status != IPStatus.Success)
                {
                    ipAddress = "not available";
                }
                else
                {
                    try
                    {
                        ipAddress = replyInputDataHost.Address.ToString();
                        positivePing = true;
                    }
                    catch (NullReferenceException)
                    {
                        ipAddress = "not available";
                    }
                }
            }
            catch (PingException)
            {
                ipAddress = "HOST NAME ERROR!";

            }
            catch (ArgumentException)
            {
                ipAddress = "HOST NAME ERROR!";
            }
            return (ipAddress, positivePing);
        }
        private void InsertDataInHosts(List<(string hostName,string physLocationHost)> hostsLoop)
        {
            for (int i = 0; i < hostsLoop.Count; i++)
            {
                Hosts host = new Hosts { hostName = hostsLoop[i].hostName, physLocationHost = hostsLoop[i].physLocationHost };
                db.GetTable<Hosts>().InsertOnSubmit(host);
            }
            db.SubmitChanges();
        }
        private void InsertDataInCheckingHosts(int hostID, bool hostStatus)
         {
            CheckingHosts checkinghosts = new CheckingHosts {  hostID = hostID, hostStatus = hostStatus };            
            db.GetTable<CheckingHosts>().InsertOnSubmit(checkinghosts);
            db.SubmitChanges();
         }
         public void ConsoleCheckDataInDataBase()
         {            
            foreach (var host in Hosts)
            {Console.WriteLine("{0} \t{1} \t{2}", host.hostID, host.hostName, host.physLocationHost); }
            Console.WriteLine();
            foreach (var checkinghost in CheckingHosts)
            {Console.WriteLine("{0} \t{1} \t{2}", checkinghost.iteration_num, checkinghost.hostID,  checkinghost.hostStatus); }
         }
        private void CheckingNetConnetions()
         {
             if (!NetworkInterface.GetIsNetworkAvailable())
             {
                 throw new Exception("Нет сети проверить соединение!");
             }
         }
    }
}
