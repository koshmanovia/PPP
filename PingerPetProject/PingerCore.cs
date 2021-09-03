using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Data;

namespace PingerPetProject
{
    #region инициализация таблиц
    [Table(Name = "Hosts")]
    internal class Hosts
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int hostID { get; set; }
        
        [Column(Name = "hostID")]
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
        static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\AppData\Host.mdf;Integrated Security=True";
        static DataContext db = new DataContext(connectionString);
        Table<Hosts> Hosts = db.GetTable<Hosts>();
        Table<CheckingHosts> CheckingHosts = db.GetTable<CheckingHosts>();
        public void test()
        {            
            foreach (var host in Hosts)
            {Console.WriteLine("{0} \t{1} \t{2}", host.hostID, host.hostName, host.physLocationHost); }
            Console.WriteLine();
            foreach (var checkinghost in CheckingHosts)
            {Console.WriteLine("{0} \t{1} \t{2}", checkinghost.iteration_num, checkinghost.hostID,  checkinghost.hostStatus); }
        }
        public void testInputHosts(int hostID, string hostName, string physLocationHost)
        {
            string sqlExpression = $"INSERT INTO Hosts (hostID, hostName,physLocationHost) VALUES ({hostID}, '{hostName}','{physLocationHost}'))";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                //int number = command.ExecuteNonQuery();
               // Console.WriteLine("Добавлено объектов: {0}", number);
            }
        }
        public void testInputCheckingHosts(int iteration_num, int hostID, bool hostStatus)
        {
            string sqlExpression = $"INSERT INTO Hosts (hostID, hostName,physLocationHost) VALUES ({iteration_num}, {hostID},{hostStatus}))";
                db.();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                //int number = command.ExecuteNonQuery();
                // Console.WriteLine("Добавлено объектов: {0}", number);

        }
         
    }
}
