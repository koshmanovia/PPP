using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace PingerPetProject
{
    static class PingerCore
    {
        static string connectionString = @"C:\MyProg\VS\projects\PPP\PingerPetProject\Host.mdf";
        public static void test()
        {
            DataContext db = new DataContext(connectionString);

            // Получаем таблицу пользователей
            Table<Host> Host = db.GetTable<Host>();

            foreach (var hosts in Host)
            {
                Console.WriteLine("{0} \t{1} \t{2}\n", hosts.hostID, hosts.hostName, hosts.physLocationHost);
            }

            Console.Read();
        }
    }
    [Table(Name = "Host")]
    public class Host
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public  int hostID { get; set; }
        [Column(Name = "hostID")]
        public string hostName { get; set; }
        [Column]
        public string physLocationHost { get; set; }
    }
}
