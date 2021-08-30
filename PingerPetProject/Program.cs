using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
namespace Pinger
{
    class Pinger
    {
        static void Main(string[] args)
        {
            InputHostNameTMP test = new InputHostNameTMP();  //rename!           
            int inputHostTimeoute = 3000;
            //горизонтальное выравнивание сделать<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            test.InputHostData();
            int windowHeightNum = test.getListHost().Count() + 2;
            if (windowHeightNum < 64)
                Console.WindowHeight = windowHeightNum;
            else
            {
                windowHeightNum = 63;
                Console.WindowHeight = windowHeightNum;
            }
            Console.WindowWidth = 120;
            PingerOutput startingAnalyze = new PingerOutput();
            startingAnalyze.CreateTableHost(test.getListHost(), inputHostTimeoute);
        }
    }
    class PingerOutput
    {
        public void CreateTableHost(List<Host> addressHost, int timeoutHost)
        {
            Ping Pinger = new Ping();
            String ipAddress = "";
            String HostName = "";
            String Description = "";
            long roadTrip = 0;
            List<String> tempHostName = new List<String>();
            List<String> tempIpAddress = new List<String>();
            List<String> tempDescription = new List<String>();
            List<long> tempRoadTrip = new List<long>();
            outputDataPinger line = new outputDataPinger();
            Console.Write(" \n      Идет обработка доступности адресов, пожалуйста подождите...");
            for (; ; )
            {

                for (int i = 0; i < addressHost.Count; i++)
                {
                    Host tempListHost = addressHost[i];
                    HostName = tempListHost.hostName;
                    try
                    {
                        PingReply ReplyInputDataHost = Pinger.Send(HostName, timeoutHost);
                        try
                        {
                            ipAddress = ReplyInputDataHost.Address.ToString();
                            roadTrip = ReplyInputDataHost.RoundtripTime;
                            Description = tempListHost.physLocationHost;
                            tempHostName.Add(HostName);
                            tempIpAddress.Add(ipAddress);
                            tempRoadTrip.Add(roadTrip);
                            tempDescription.Add(Description);
                        }
                        catch (NullReferenceException)
                        {
                            ipAddress = "not available";
                            Description = tempListHost.physLocationHost;
                            roadTrip = 0;
                            tempHostName.Add(HostName);
                            tempIpAddress.Add(ipAddress);
                            tempRoadTrip.Add(roadTrip);
                            tempDescription.Add(Description);
                        }
                    }
                    catch (PingException)
                    {
                        ipAddress = "HOST NAME ERROR!";
                        Description = tempListHost.physLocationHost;
                        roadTrip = 0;
                        tempHostName.Add(HostName);
                        tempIpAddress.Add(ipAddress);
                        tempRoadTrip.Add(roadTrip);
                        tempDescription.Add(Description);
                    }
                }
                Console.Clear();
                line.writeHeadTable();
                //Console.WriteLine();
                for (int i = 0; i < addressHost.Count; i++)
                {
                    line.writeTextColor(tempHostName[i], tempIpAddress[i], tempRoadTrip[i], tempDescription[i]);
                    // Console.WriteLine();
                    Thread.Sleep(4);
                }
                tempHostName.Clear();
                tempIpAddress.Clear();
                tempRoadTrip.Clear();
                Thread.Sleep(300);
                Console.ForegroundColor = ConsoleColor.Black;

            }
        }
    }
    class Host
    {
        public string hostName { get; set; }
        public byte pingIterator { get; set; }
        public bool hostStatus { get; set; }
        public byte qualityLinkHost { get; set; }
        public short breaksNumHost { get; set; }
        public string physLocationHost { get; set; }

        private short BreaksNumHost
        {
            get => breaksNumHost;
            set
            {
                if (breaksNumHost < 1000)
                    breaksNumHost = value;
            }
        }
        public Host(string name, string physLocation)
        {
            hostName = name;
            pingIterator = 0;
            hostStatus = true;
            qualityLinkHost = 100;
            breaksNumHost = 0;
            physLocationHost = physLocation;
        }
    }
    class outputDataPinger // весь вывод текстовых сообщений перенести сюда, и переименовать класс. вывод сделать процедурно, и правильно назвать процедуры, чтобы было понятно
    {

        public void writeCharLine(int inpLongNum, char inpChar)
        {
            for (int j = 0; j < inpLongNum; j++)
            {
                Console.Write(inpChar);
            }
        }
        public void writeCharLine(int inpLongNum)
        {
            char inpChar = ' ';
            for (int j = 0; j < inpLongNum; j++)
            {
                Console.Write(inpChar);
            }
        }
        public void writeHeadTable()
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            writeCharLine(2);
            Console.Write("DNS Address");
            writeCharLine(12);
            Console.Write("IP Address");
            writeCharLine(10);
            Console.Write("Ping");
            writeCharLine(4);
            Console.Write("Quality");
            writeCharLine(4);
            Console.Write("Description");
            writeCharLine(36);
            Console.WriteLine();
        }
        public void writeTextColor(String hostName, String ipAddress, long roadTrip, String description)
        {
            int LengthHostName = hostName.Length;
            int LengthipAddress = ipAddress.Length;
            String strRoadTrip = roadTrip.ToString();
            int LengthroadTrip = strRoadTrip.Length;
            Console.BackgroundColor = ConsoleColor.Black;
            if (ipAddress == "not available")
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.DarkRed;
            }
            else if (ipAddress == "HOST NAME ERROR!")
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Red;
            }
            else if (roadTrip == 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                roadTrip = 1;
            }
            else if (roadTrip < 3)
                Console.ForegroundColor = ConsoleColor.DarkCyan;
            else if (roadTrip < 21)
                Console.ForegroundColor = ConsoleColor.Cyan;
            else if (roadTrip < 41)
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            else if (roadTrip < 71)
                Console.ForegroundColor = ConsoleColor.Green;
            else if (roadTrip < 111)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if (roadTrip < 151)
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            else if (roadTrip < 251)
                Console.ForegroundColor = ConsoleColor.Red;
            else Console.ForegroundColor = ConsoleColor.DarkRed;

            if (LengthHostName > 20)
            {
                int tempNumipAddress = 21 - LengthipAddress;
                int tempNumroadTrip = 6 - LengthroadTrip;
                Console.Write("  " + hostName.Substring(0, 19) + "..  ");
                Console.Write(ipAddress);
                writeCharLine(tempNumipAddress);
                Console.Write(roadTrip);
                writeCharLine(tempNumroadTrip + 2);
                Console.Write(description);
                //для расширения просто посмотрть как смотрится в будущем все будет работать
                Console.Write("100%");
                writeCharLine(6);
                Console.WriteLine();
            }
            else
            {
                int tempNumHostName = 23 - LengthHostName;
                int tempNumipAddress = 21 - LengthipAddress;
                int tempNumroadTrip = 6 - LengthroadTrip;
                Console.Write("  " + hostName);
                writeCharLine(tempNumHostName);
                Console.Write(ipAddress);
                writeCharLine(tempNumipAddress);
                Console.Write(roadTrip);
                writeCharLine(tempNumroadTrip + 2);
                Console.Write(description);
                //для расширения просто посмотрть как смотрится  в будущем все будет работать       
                Console.Write("100%");
                writeCharLine(6);
                Console.WriteLine();
            }
            Console.ResetColor();
        }
    }
    class InputHostNameTMP //после написания класса, удалить TMP <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<    
    {
        List<Host> ListHost = new List<Host>();
        string command = "";
        public void InputHostData()//переименуй процедуру
        {
            //вывод данных из файла на экран, пронумерованным списком
            Console.WriteLine("Наберите команду для продолжения");
            Console.WriteLine("D   - Вывести на экран содержимое файла \"HostDataBase.txt\"");
            Console.WriteLine("R   - для чтения файла \"HostDataBase.txt\"");
            Console.WriteLine("W   - для записи еще данных в конец файла, не стирая данные");
            Console.WriteLine("RW  - для удаления данных из файла и записи их в ручную через консоль");
            Console.WriteLine("      Испольюзуя ключ -b будет сделан backup в \\%root_program_folder%\\Data\\backup\\%date%.txt");


            for (bool check = true; check == true;)
            {
                command = Console.ReadLine();
                switch (command)
                {
                    //добавить редактирование данных по строке
                    case "D":
                        displayFileData();
                        break;
                    case "R":
                        readFile();
                        check = false;
                        break;
                    case "W":
                        enterByHand();//ввод данных вручную
                        readFile();
                        check = false;
                        break;
                    case "RW":
                        enterByHand();//ввод данных вручную
                        readFile();
                        check = false;
                        break;
                    case "RW -b":
                        enterByHand();//ввод данных вручную
                        readFile();
                        check = false;
                        break;
                    default:
                        Console.WriteLine("Команда введена не верно, повторите ввод \n");
                        Console.WriteLine("Наберите команду для продолжения");
                        Console.WriteLine("R   - для чтения файла \"HostDataBase.txt\"");
                        Console.WriteLine("W   - для записи еще данных в конец файла, не стирая данные");
                        Console.WriteLine("RW  - для удаления данных из файла и записи их в ручную через консоль");
                        Console.WriteLine("      Испольюзуя ключ -b будет сделан backup в \\%root_program_folder%\\Data\\backup\\%date%.txt");
                        break;
                }
            }
        }
        public void enterByHand() //переписать, чтобы не спрашивал каждый раз "да/нет" а только "нет" для выхода
        {
            String hostName = "";
            String hostDescription = "";
            String reply = "";
            bool checkInp = true;
            while (checkInp == true)
            {
                Console.WriteLine("Введите имя хоста");
                hostName = Console.ReadLine();
                Console.WriteLine("Введите расположение введенного хоста");
                hostDescription = Console.ReadLine();
                createAndFillObjectHost(hostName, hostDescription);

                for (; ; )
                {
                    Console.WriteLine("Ввести еще один Host? \"да/нет\"");
                    reply = Console.ReadLine();
                    if (reply == "нет")
                    {
                        checkInp = false;
                        break;
                    }
                    else
                    if (reply == "да")
                        break;
                    else Console.WriteLine("ОШИБКА: Неверная команда, повторить ввод.");
                }
            }
        }
        public void readFile()
        {
            int separator = 0;
            String tempChar = "";
            String tempHostName = "";
            String tempDescription = "";
            List<String> poolLineFromFile = new List<String>();
            String tempLineFromFile = "";
            String projectPath = Environment.CurrentDirectory + "\\Data\\HostDataBase.txt";
            StreamReader fileReader = new StreamReader(projectPath);
            while (tempLineFromFile != null)
            {
                tempLineFromFile = fileReader.ReadLine();
                if (tempLineFromFile != null)
                    poolLineFromFile.Add(tempLineFromFile);
            }
            fileReader.Close();

            for (int i = 0; i < poolLineFromFile.Count; i++)
            {
                tempLineFromFile = poolLineFromFile[i];
                char[] poolLineCharFromFile = tempLineFromFile.ToCharArray();
                for (int j = 0; j < poolLineCharFromFile.Length; j++)
                {
                    if (poolLineCharFromFile[j] == ' ')
                    {
                        separator = j;
                        break;
                    }
                }
                tempHostName = ""; //Обнуление переменных
                tempDescription = "";
                for (int k = 0; k < separator; k++)
                {
                    tempChar = poolLineCharFromFile[k].ToString();
                    tempHostName = tempHostName + tempChar;
                }
                for (; separator < poolLineCharFromFile.Length; separator++)
                {
                    tempChar = poolLineCharFromFile[separator].ToString();
                    tempDescription = tempDescription + tempChar;
                }
                if (tempHostName == "")
                    tempHostName = "null";//да это костыль, отстаньте!
                if (tempDescription == "")
                    tempDescription = "null";//да это костыль, отстаньте!
                createAndFillObjectHost(tempHostName, tempDescription);
            }
        }
        public void createAndFillObjectHost(String HostName, String HostDescription)
        {
            Host newHost = new Host(HostName, HostDescription);
            ListHost.Add(newHost);
        }

        public List<Host> getListHost()
        {
            return ListHost;
        }

        public void displayFileData()
        {
            readFile();
            for (int i = 0; i < ListHost.Count(); i++)
            {
                Console.WriteLine(ListHost[i].hostName + " " + ListHost[i].physLocationHost);
            }
            Console.WriteLine();
            ListHost.Clear();
            InputHostData();
        }
    }
}