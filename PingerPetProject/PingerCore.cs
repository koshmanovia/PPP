﻿using System;
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
                int intResult = (ManagePingerDataBase.LookUpPositivePingFromCheckingHosts(1) * 100) / ManagePingerDataBase.LookUpAllPingFromCheckingHosts(1);
                return intResult;
            }
        } 
        class Host
        {
            private string physLocationHost = default;//+
            private string hostName = default;//+
            private int idInDataBase = default;//-
            private string ipAddress = default;//-
            private bool positivePing = default;//??
            private long roadTrip = default;//-
            private int quallity = default;//-
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
                    return hostName = ManagePingerDataBase.LookUpHostNameFromHosts(idInDataBase);
                } //получение имени из базы данных
                set
                {
                    hostName = HostName;
                } //ввод имени в базу данных

            }
            public string PhysLocationHost
            {
                get
                {
                    return hostName = ManagePingerDataBase.LookUpPhysLocationHostFromHosts(idInDataBase);
                } //получение расположения из базы данных
                set
                {
                    physLocationHost = PhysLocationHost;
                } //ввод расположения в базу данных
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
