using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;

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
        [Column]
        public int hostID { get; set; }
        [Column(Name = "hostID")]
        public bool hostStatus { get; set; }
    }
    #endregion
    static class PingerCore
    {
        static private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\AppData\Host.mdf;Integrated Security=True";
       
        public static void test()
        {
            DataContext db = new DataContext(connectionString);
            Table<Hosts> Hosts = db.GetTable<Hosts>();
            Table<CheckingHosts> CheckingHosts = db.GetTable<CheckingHosts>();
            foreach (var host in Hosts)
            {Console.WriteLine("{0} \t{1} \t{2}", host.hostID, host.hostName, host.physLocationHost); }
            Console.WriteLine();
            foreach (var checkinghost in CheckingHosts)
            //checkinghost.hostStatus - возвращает все запросы true хотя в бд не так, почему?
            { Console.WriteLine("{0} \t{1} \t{2}", checkinghost.iteration_num, checkinghost.hostID, checkinghost.hostStatus); }
        }
    }
}
