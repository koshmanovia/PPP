using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Net.NetworkInformation;

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

        private static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\AppData\Host.mdf;Integrated Security=True";
        private static DataContext db = new DataContext(connectionString);
        private Table<Hosts> Hosts = db.GetTable<Hosts>();
        private Table<CheckingHosts> CheckingHosts = db.GetTable<CheckingHosts>();

        private bool needInsertHosts = true;

        public List<(string hostName, string ipAddress, ushort Ping, string Description)> StastPing (List<(string HostName,string physLocationHost)> hostsLoop)//Придумать нормальные имена массиву передаваему и методу
        {
            if (needInsertHosts != false)
            {
                InsertDataInHosts(hostsLoop);
            }
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
         public void test()
        {            
            foreach (var host in Hosts)
            {Console.WriteLine("{0} \t{1} \t{2}", host.hostID, host.hostName, host.physLocationHost); }
            Console.WriteLine();
            foreach (var checkinghost in CheckingHosts)
            {Console.WriteLine("{0} \t{1} \t{2}", checkinghost.iteration_num, checkinghost.hostID,  checkinghost.hostStatus); }
        }
    }
}
