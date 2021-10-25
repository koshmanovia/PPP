using System;
using System.Collections.Generic;
using System.Data;
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
            #region Открытие и закрытие подключения к БД
            private static SqlConnection _sqlConnection = null;
            private static void OpenConnection()
            {
                _sqlConnection = new SqlConnection { ConnectionString = connectionString };
                _sqlConnection.Open();
            }
            private static void CloseConnection()
            {
                if (_sqlConnection?.State != ConnectionState.Closed)
                {
                    _sqlConnection?.Close();
                }
            }
            #endregion
            public static void InsertDataInHosts(string _hostName, string _physLocationHost)
            {
                Hosts host = new Hosts { hostName = _hostName, physLocationHost = _physLocationHost };
                db.GetTable<Hosts>().InsertOnSubmit(host);
                db.SubmitChanges();
            } 
            public static void InsertDataInCheckingHosts(int hostID, bool hostStatus)
            {
                //вносит изменения в table но не вносит в sql
                CheckingHosts checkinghosts = new CheckingHosts { hostID = hostID, hostStatus = hostStatus };
                db.GetTable<CheckingHosts>().InsertOnSubmit(checkinghosts);
                db.SubmitChanges();
            }
            public static string LookUpHostNameFromHosts(int hostID)
            {
                //работает
                //var _sqlConnection = new SqlConnection { ConnectionString = connectionString };
                //_sqlConnection.Open();
                OpenConnection();
                string hostName;
                // Установить имя хранимой процедуры.
                using (SqlCommand command = new SqlCommand("GetHostNameFromHosts", _sqlConnection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    // Входной параметр.
                    SqlParameter param = new SqlParameter
                    {
                        ParameterName = "@hostID",
                        SqlDbType = SqlDbType.Int,
                        Value = hostID,
                        Direction = ParameterDirection.Input
                    };
                    command.Parameters.Add(param);
                    // Выходной параметр,
                    param = new SqlParameter
                    {
                        ParameterName = "@hostName",
                        SqlDbType = SqlDbType.Char,
                        Size = 10,
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(param);
                    // Выполнить хранимую процедуру,
                    command.ExecuteNonQuery();
                    // Возвратить выходной параметр.
                    hostName = (string)command.Parameters["@hostName"].Value;                  
                }
                //_sqlConnection.Close();
                CloseConnection();
                return hostName;
            }
            public static void GetPhysLocationHostFromHosts(int hostID)
            {
               //наполнить
            }
            public static void GetAllPingFromCheckingHosts(int hostID)
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
            ManagePingerDataBase.InsertDataInCheckingHosts(0, true);
            ManagePingerDataBase.InsertDataInCheckingHosts(1, true);
            ManagePingerDataBase.InsertDataInCheckingHosts(2, true);
            ManagePingerDataBase.InsertDataInCheckingHosts(1, true);
            ManagePingerDataBase.InsertDataInCheckingHosts(0, true);
            ManagePingerDataBase.InsertDataInCheckingHosts(3, true);
            ManagePingerDataBase.InsertDataInCheckingHosts(2, true);
            ManagePingerDataBase.InsertDataInCheckingHosts(0, true);
            string s = ManagePingerDataBase.LookUpHostNameFromHosts(0);
            Console.WriteLine(s);
            Console.WriteLine(Hosts.ToString());
            Console.WriteLine();
            ConsoleCheckDataInDataBase();
           // var subset = from string s in Hosts where 
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
