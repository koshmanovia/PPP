using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Linq;
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
        private static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\AppData\Host.mdf;Integrated Security=True";
        private static DataContext db = new DataContext(connectionString);
        private Table<Hosts> Hosts = db.GetTable<Hosts>();
        private Table<CheckingHosts> CheckingHosts = db.GetTable<CheckingHosts>();

        private static class ManagePingerDataBase
        {
            public static void InsertDataInHosts(string _hostName, string _physLocationHost)
            {
                Hosts host = new Hosts { hostName = _hostName, physLocationHost = _physLocationHost };
                db.GetTable<Hosts>().InsertOnSubmit(host);
                db.SubmitChanges();
            } 
            public static void InsertDataInCheckingHosts(int hostID, bool hostStatus)
            {
                CheckingHosts checkinghosts = new CheckingHosts { hostID = hostID, hostStatus = hostStatus };
                db.GetTable<CheckingHosts>().InsertOnSubmit(checkinghosts);
                db.SubmitChanges();
            }
            public static void GetHostNameFromHosts(int id)
            {
                //наполнить
            }
            public static void GetPhysLocationHostFromHosts(int id)
            {
               //наполнить
            }
            public static void GetAllPingFromCheckingHosts(int id)
            {
                //наполнить
            }
            public static void GetPositivePingFromCheckingHosts(int id)
            {
                //наполнить
            }
        }
 
        class Host
        {
            private string physLocationHost = default;
            private string hostName = default;
            private string ipAddress = default;
            private bool positivePing = default;
            private long roadTrip = default;
            private int quallity = default;
            private int idInDataBase = default;
            public string HostName
            {
                get
                {
                    string sqlExpression = $"SELECT hostName FROM Hosts WHERE hostID = \'{idInDataBase}\'";
                    return hostName;
                } //получение имени из базы данных
                set
                {
                    hostName = HostName;
                } //ввод имени в базу данных

            }
            public string PhysLocationHost
            {
                get;//получение расположения из базы данных
                set;//ввод расположения в базу данных
            }
            private Ping Pinger = new Ping();
            private PingOptions options = new PingOptions(128, dontFragment: true);//перенести в конструкторы управление ttl
            private int timeOutHostPing = 3000;//перенести в конструкторы управление временем пинга
            
            private Thread threadForPing = default;
            //сделать нормальный конструктор
            public void Ping()
            {
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
                            if (replyInputDataHost.RoundtripTime == 0)
                            {
                                roadTrip = replyInputDataHost.RoundtripTime + 1;
                            }
                            else
                            {
                                roadTrip = replyInputDataHost.RoundtripTime;
                            }
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
                finally
                {
                    //(ipAddress, positivePing, tempRoadTrip);
                } 
            }
        }

        public void TestGetDataFromHosts(int idInDataBase)
        {
            ManagePingerDataBase.InsertDataInHosts("t1", "t2");
            ManagePingerDataBase.InsertDataInHosts("t1", "t2");
            ManagePingerDataBase.InsertDataInHosts("t1", "t2");
            ManagePingerDataBase.InsertDataInHosts("t1", "t2");
            Console.WriteLine(Hosts.ToString());
            Console.WriteLine();
            ConsoleCheckDataInDataBase();
            var subset = from s in Hosts where 
        }

        private void CheckingNetConnetions()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                throw new Exception("Нет сети -- проверить соединение!");
            }
        }
        public void ConsoleCheckDataInDataBase()
        {
            foreach (var host in Hosts)
            { Console.WriteLine("{0} \t{1} \t{2}", host.hostID, host.hostName, host.physLocationHost); }
            Console.WriteLine();
            foreach (var checkinghost in CheckingHosts)
            { Console.WriteLine("{0} \t{1} \t{2}", checkinghost.iteration_num, checkinghost.hostID, checkinghost.hostStatus); }
        }
    }
    
}
