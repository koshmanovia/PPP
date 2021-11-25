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
            #region методы открытия и закрытия подключения к БД
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
            static SqlConnection connect = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\AppData\Host.mdf;Integrated Security=True");
            public static void InsertDataInHosts(string _hostName, string _physLocationHost)
            {
                SqlCommand command = new SqlCommand("INSERT INTO Hosts VALUES(@hostName, @physLocationHost)", connect);
                command.Parameters.AddWithValue("@hostName", _hostName);
                command.Parameters.AddWithValue("@physLocationHost", _physLocationHost);
                command.Connection.Open();
                command.ExecuteNonQuery();
                command.Connection.Close();
            } 
            public static void InsertDataInCheckingHosts(int _hostID, bool _hostStatus)
            {
                SqlCommand command = new SqlCommand("INSERT INTO CheckingHosts VALUES(@hostID, @hostStatus)", connect);
                command.Parameters.AddWithValue("@hostID", _hostID);
                command.Parameters.AddWithValue("@hostStatus", _hostStatus);
                command.Connection.Open();
                command.ExecuteNonQuery();
                command.Connection.Close();
            }
            public static string LookUpHostNameFromHosts(int hostID)
            {
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
                        Size = 32,
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(param);
                    // Выполнить хранимую процедуру,
                    command.ExecuteNonQuery();
                    // Возвратить выходной параметр.
                    hostName = (string)command.Parameters["@hostName"].Value;                  
                }               
                CloseConnection();
                return hostName;
            }
            public static string LookUpPhysLocationHostFromHosts(int hostID)
            {
                OpenConnection();
                string physLocationHost;
                // Установить имя хранимой процедуры.
                using (SqlCommand command = new SqlCommand("GetPhysLocationHostFromHosts", _sqlConnection))
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
                        ParameterName = "@physLocationHost",
                        SqlDbType = SqlDbType.Char,
                        Size = 32,
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(param);
                    // Выполнить хранимую процедуру,
                    command.ExecuteNonQuery();
                    // Возвратить выходной параметр.
                    physLocationHost = (string)command.Parameters["@physLocationHost"].Value;
                }
                CloseConnection();
                return physLocationHost;
            }
            public static int LookUpAllPingFromCheckingHosts(int hostID)
            {               
                SqlConnection con;
                con = new SqlConnection();
                con.ConnectionString = connectionString;
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.Parameters.AddWithValue("@hostID", hostID);
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT count(*) from CheckingHosts where hostID = @hostID";
                var result = cmd.ExecuteScalar();
                int intResult = Convert.ToInt32(result);
                con.Close();
                return intResult;
            }
            public static int LookUpPositivePingFromCheckingHosts(int hostID)
            {
                SqlConnection con;
                con = new SqlConnection();
                con.ConnectionString = connectionString;
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.Parameters.AddWithValue("@hostID", hostID);
                cmd.Parameters.AddWithValue("@hostStatus", true);
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT count(*) from CheckingHosts where hostID = @hostID AND hostStatus = @hostStatus";
                var result = cmd.ExecuteScalar();
                int intResult = Convert.ToInt32(result);
                con.Close();
                return intResult;
            }
            public static int QuallityOnHostId(int hostId)
            {                
                int intResult = (ManagePingerDataBase.LookUpPositivePingFromCheckingHosts(hostId) * 100) / ManagePingerDataBase.LookUpAllPingFromCheckingHosts(hostId);
                return intResult;
            }
        } 
        class Host
        {
            readonly private int idInDataBase = default;//передается через конструктор
            private string ipAddress = default;//вычисляется в пинге           
            private long roadTrip = default;//вычисляется в пинге
            private int timeOutHostPing = 3000;//стандартное значение времени пинга 
            private static int TTL = 128; //стандартное время жизни пакета пинга
            private bool checkHost = false;
            #region работа с переменными
            public int Quallity 
            {
                get
                {                    
                    return ManagePingerDataBase.QuallityOnHostId(idInDataBase);
                }
            }
            
            public string HostName
            {
                get
                {                    
                    return ManagePingerDataBase.LookUpHostNameFromHosts(idInDataBase);
                }
            }
            public string PhysLocationHost
            {
                get
                {
                    return ManagePingerDataBase.LookUpPhysLocationHostFromHosts(idInDataBase);
                } //получение расположения из базы данных
            }
            #endregion
            #region конструкторы

            #endregion
            private Ping Pinger = new Ping();
            private PingOptions options = new PingOptions(TTL, dontFragment: true);//перенести в конструкторы управление ttl           
            private Thread threadForPing = default;
            
            public void Ping()
            {
                try
                {
                    PingReply replyInputDataHost = Pinger.Send(HostName, timeOutHostPing);
                    if (replyInputDataHost.Status != IPStatus.Success)
                    {
                        ipAddress = "not available";
                        roadTrip = 0;
                    }
                    else
                    {
                        try
                        {
                            ipAddress = replyInputDataHost.Address.ToString();
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
                            roadTrip = 0;
                        }
                    }
                }
                catch (PingException)
                {                   
                    ipAddress = "HOST NAME ERROR!";
                    roadTrip = 0;

                }
                catch (ArgumentException)
                {                    
                    ipAddress = "HOST NAME ERROR!";
                    roadTrip = 0;
                }
                finally
                {
                    ManagePingerDataBase.InsertDataInCheckingHosts(idInDataBase, checkHost);
                } 
            }
        }
 #region тестовая процедура
        public void TestGetDataFromHosts(int idInDataBase)
        {
            //ManagePingerDataBase.InsertDataInHosts("t1", "t11");
            //ManagePingerDataBase.InsertDataInHosts("t2", "t22");
            //ManagePingerDataBase.InsertDataInHosts("t3", "t33");
            //ManagePingerDataBase.InsertDataInHosts("t4", "t44");
            //ManagePingerDataBase.InsertDataInCheckingHosts(0, true);
            //ManagePingerDataBase.InsertDataInCheckingHosts(1, true);
            //ManagePingerDataBase.InsertDataInCheckingHosts(2, true);
            //ManagePingerDataBase.InsertDataInCheckingHosts(1, true);
            //ManagePingerDataBase.InsertDataInCheckingHosts(0, true);
            //ManagePingerDataBase.InsertDataInCheckingHosts(3, true);
            //ManagePingerDataBase.InsertDataInCheckingHosts(2, true);
            //ManagePingerDataBase.InsertDataInCheckingHosts(0, true);
            //for (int j = 0; j < 2160; j++)
            //{
            //    if (j < 812)
            //    {
            //        ManagePingerDataBase.InsertDataInCheckingHosts(1, true);
            //    }
            //    else
            //    {
            //        ManagePingerDataBase.InsertDataInCheckingHosts(1, false);
            //    }
            //}
            //Console.WriteLine("Input Data completed");
            //int i = default;
            //i = ManagePingerDataBase.LookUpAllPingFromCheckingHosts(1);
            //Console.WriteLine($"All number ping {i}");
            //i = ManagePingerDataBase.LookUpPositivePingFromCheckingHosts(1);
            //Console.WriteLine($"All number positive ping {i}");            
            //i = ManagePingerDataBase.QuallityOnHostId(1);
            //Console.WriteLine($"quality hostId(1) = {i}");
            //Console.WriteLine();            
        }
 #endregion
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
