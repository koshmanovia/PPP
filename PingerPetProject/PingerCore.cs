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
    #region ДОБАВИТЬ ОПИСАНИЕ
    [Table(Name = "Host")]
    internal class Host
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int hostID { get; set; }
        [Column(Name = "hostID")]
        public string hostName { get; set; }
        [Column]
        public string physLocationHost { get; set; }
    }
    #endregion
    static class PingerCore
    {
        static private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\AppData\Host.mdf;Integrated Security=True";

        public static void test()
        {
            DataContext db = new DataContext(connectionString);
            // Получаем таблицу пользователей
            Table<Host> Host = db.GetTable<Host>();
            foreach (var hosts in Host)
            {
                Console.WriteLine("{0} \t{1} \t{2}", hosts.hostID, hosts.hostName, hosts.physLocationHost);
            }
        }
    }
}
