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
            ConsolePinger test = new ConsolePinger();
            int inputHostTimeoute = 3000;
            test.InputHostData();
            int windowHeightNum = test.getListHost().Count() + 5;
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
            PingOptions options = new PingOptions();
            options.DontFragment = true;
            String HostName = "";
            List<String> tempHostName = new List<String>();
            List<String> tempIpAddress = new List<String>();
            List<String> tempDescription = new List<String>();
            List<int> tempQualityHost = new List<int>();
            List<long> tempRoadTrip = new List<long>();
            outputDataPinger line = new outputDataPinger();
            Console.Clear();
            Console.Write(" \n      Идет обработка доступности адресов, пожалуйста подождите...");
            for (long iterator = 1; ; iterator++)
            {

                if (NetworkInterface.GetIsNetworkAvailable())
                {

                    for (int i = 0; i < addressHost.Count; i++)
                    {
                        Host tempListHost = addressHost[i];
                        HostName = tempListHost.hostName;
                        try
                        {
                            PingReply ReplyInputDataHost = Pinger.Send(HostName, timeoutHost);
                            if (ReplyInputDataHost.Status != IPStatus.Success)
                            {
                                tempHostName.Add(HostName);
                                tempIpAddress.Add("not available");
                                tempRoadTrip.Add(ReplyInputDataHost.RoundtripTime);
                                tempDescription.Add(tempListHost.physLocationHost);
                                tempQualityHost.Add(tempListHost.Quality(false));
                            }
                            else
                            {
                                try
                                {
                                    tempHostName.Add(HostName);
                                    tempIpAddress.Add(ReplyInputDataHost.Address.ToString());
                                    tempRoadTrip.Add(ReplyInputDataHost.RoundtripTime);
                                    tempDescription.Add(tempListHost.physLocationHost);
                                    tempQualityHost.Add(tempListHost.Quality(true));
                                }
                                catch (NullReferenceException)
                                {
                                    tempHostName.Add(HostName);
                                    tempIpAddress.Add("not available");
                                    tempRoadTrip.Add(0);
                                    tempDescription.Add(tempListHost.physLocationHost);
                                    tempQualityHost.Add(tempListHost.Quality(false));
                                }
                            }
                        }
                        catch (PingException)
                        {
                            tempHostName.Add(HostName);
                            tempIpAddress.Add("HOST NAME ERROR!");
                            tempRoadTrip.Add(0);
                            tempDescription.Add(tempListHost.physLocationHost);
                            tempQualityHost.Add(tempListHost.Quality(false));
                        }
                        catch (ArgumentException)
                        {
                            tempHostName.Add(HostName);
                            tempIpAddress.Add("HOST NAME ERROR!");
                            tempRoadTrip.Add(0);
                            tempDescription.Add(tempListHost.physLocationHost);
                            tempQualityHost.Add(tempListHost.Quality(false));
                        }
                        /*//////////////////////////////////////////////////////////////////////
                        ///  РАЗОБРАТЬСЯ С РАБОТОЙ ИСКЛЮЧЕНИЙ, что и когда перехватывают       
                        catch (ObjectDisposedException)
                        {
                        tempHostName.Add(HostName);
                        tempIpAddress.Add("ObjectDisposedException!");
                        tempRoadTrip.Add(999);
                        tempDescription.Add(tempListHost.physLocationHost);
                        tempQualityHost.Add(tempListHost.Quality(false));
                        }
                        catch (InvalidOperationException)
                        {
                        tempHostName.Add(HostName);
                        tempIpAddress.Add("InvalidOperationException");
                        tempRoadTrip.Add(999);
                        tempDescription.Add(tempListHost.physLocationHost);
                        tempQualityHost.Add(tempListHost.Quality(false));
                        }                    
                        //////////////////////////////////////////////////////////////////*/
                    }
                    Console.Clear();
                    line.writeHeadTable();
                    for (int i = 0; i < addressHost.Count; i++)
                    {
                        line.writeTextColor(tempHostName[i], tempIpAddress[i], tempRoadTrip[i], tempDescription[i], tempQualityHost[i]);
                        //Thread.Sleep(4);
                    }
                    tempHostName.Clear();
                    tempIpAddress.Clear();
                    tempRoadTrip.Clear();
                    tempDescription.Clear();
                    tempQualityHost.Clear();
                    //Thread.Sleep(250);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Clear();
                    Console.WriteLine(" \n \n \n \n \n \n             Связи нет! \n      ПРОВЕРЬТЕ СЕТЕВОЕ ПОДКЛЮЧЕНИЕ!pin");
                }
                Console.WriteLine("\nКоличество итераций = " + iterator);
                Thread.Sleep(1500);
            }
        }
    }
    class Host
    {
        public string hostName { get; set; }
        public byte pingIterator { get; set; }
        public bool hostStatus { get; set; }
        public int qualityLinkHost { get; set; }
        public string physLocationHost { get; set; }
        List<bool> QualityArray = new List<bool>();
        int maxSizeArrayQuality = 0;
        bool triggerFullArray = false;


        public Host(string name, string physLocation)
        {
            hostName = name;
            pingIterator = 0;
            hostStatus = true;
            physLocationHost = physLocation;
            qualityLinkHost = 0;
        }
        public int Quality(bool hostStatus)
        {
            double LinkTrue = 0;
            double LinkTrueTemp = 0;

            if (triggerFullArray == true)
            {
                if (maxSizeArrayQuality < 1000)
                {
                    QualityArray[maxSizeArrayQuality] = hostStatus;
                }
                if (maxSizeArrayQuality == 1000)
                {
                    QualityArray[maxSizeArrayQuality] = hostStatus;
                    maxSizeArrayQuality = -1;
                }
            }
            if (triggerFullArray == false)
            {
                if (QualityArray.Count < 1000)
                {
                    QualityArray.Add(hostStatus);
                }
                if (QualityArray.Count == 1000)
                {
                    triggerFullArray = true;
                    maxSizeArrayQuality = -1;
                    QualityArray.Add(hostStatus);
                }
            }
            for (int i = 0; i < QualityArray.Count; i++)
            {
                if (QualityArray[i] == true)
                {
                    LinkTrue++;
                }
            }
            double QualityArrayDouble = QualityArray.Count;
            LinkTrueTemp = LinkTrue / QualityArrayDouble * 100;
            maxSizeArrayQuality++;
            qualityLinkHost = (int)LinkTrueTemp;
            return qualityLinkHost;
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
            writeCharLine(5);
            Console.Write("Description");
            writeCharLine(43);
            Console.WriteLine();
        }
        public void writeTextColor(String hostName, String ipAddress, long roadTrip, String description, int qualityHost)
        {
            int LengthHostName = hostName.Length;
            int Lengthdescription = description.Length;
            int LengthipAddress = ipAddress.Length;
            String strRoadTrip = roadTrip.ToString();
            int LengthroadTrip = strRoadTrip.Length;
            String stringQualityHost = qualityHost.ToString() + "%";
            if (stringQualityHost.Length < 4)
            {
                if (stringQualityHost.Length < 3)
                {
                    stringQualityHost = "  " + stringQualityHost;
                }
                else
                {
                    stringQualityHost = " " + stringQualityHost;
                }
            }

            Console.BackgroundColor = ConsoleColor.Black;
            if (ipAddress == "not available")
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.DarkRed;


                //МНЕ СТЫДНО ЗА ЭТОТ КУСОК, Я ЕГО ПЕРЕПЕШУЮ ОБЕЩАЮ!<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

                string fileName = Environment.CurrentDirectory + "\\Data\\log.txt";
                FileStream aFile = new FileStream(fileName, FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(aFile);
                aFile.Seek(0, SeekOrigin.End);
                sw.WriteLine(DateTime.Now + " - " + hostName + " Недоступен");
                sw.Close();

            }
            else if (ipAddress == "HOST NAME ERROR!")
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Red;



                /*/МНЕ СТЫДНО ЗА ЭТОТ КУСОК, Я ЕГО ПЕРЕПЕШУЮ ОБЕЩАЮ!<<<<<ТУТ СКОРЕЕ ВСЕГО НЕ НАДО ЛОГИРОВАТЬ<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
                string fileName = Environment.CurrentDirectory + "\\Data\\log.txt";
                FileStream aFile = new FileStream(fileName, FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(aFile);
                aFile.Seek(0, SeekOrigin.End);
                sw.WriteLine(DateTime.Now + " - " + hostName);
                sw.Close();*/
            }
            else if (roadTrip == 0)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                roadTrip = 1;
            }
            else if (roadTrip < 3)
                Console.ForegroundColor = ConsoleColor.Cyan;
            else if (roadTrip < 21)
                Console.ForegroundColor = ConsoleColor.DarkCyan;
            else if (roadTrip < 41)
                Console.ForegroundColor = ConsoleColor.Green;
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
                Console.Write(stringQualityHost);
                writeCharLine(6);
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
                Console.Write(stringQualityHost);
                writeCharLine(6);

            }
            if (Lengthdescription > 55)
            {
                Console.Write(description.Substring(0, 53) + "..");
                Console.WriteLine();
            }
            else
            {
                Console.Write(description);
                Console.WriteLine();
            }
            Console.ResetColor();
        }
    }
    class ConsolePinger
    {
        List<Host> ListHost = new List<Host>();
        string command = "";
        public void InputHostData()//переименуй процедуру
        {
            //вывод данных из файла на экран, пронумерованным списком

            for (bool check = true; check == true;)
            {
                command = Console.ReadLine();
                switch (command)
                {
                    //добавить редактирование данных по строке
                    case "disp":
                        try
                        {
                            displayFileData();
                        }
                        catch (DirectoryNotFoundException)
                        {
                            Console.WriteLine("Файл отсутсвует");
                        }
                        break;

                    case "start":
                        readFile();
                        check = false;
                        break;

                    case "add":
                        enterByHand();//ввод данных вручную
                        readFile();
                        check = false;
                        break;

                    case "add -r":
                        enterByHand();//ввод данных вручную
                        readFile();
                        check = false;
                        break;

                    case "add -b":
                        enterByHand();//ввод данных вручную
                        readFile();
                        check = false;
                        break;

                    case "test":
                        Ping pingSender = new Ping();
                        PingOptions options = new PingOptions();

                        // Use the default Ttl value which is 128,
                        // but change the fragmentation behavior.
                        options.DontFragment = true;

                        // Create a buffer of 32 bytes of data to be transmitted.
                        string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                        byte[] buffer = Encoding.ASCII.GetBytes(data);
                        int timeout = 120;
                        PingReply reply = pingSender.Send("10.10.10.10", timeout, buffer, options);
                        if (reply.Status == IPStatus.Success)
                        {
                            Console.WriteLine("Address: {0}", reply.Address.ToString());
                            Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                            Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
                            Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                            Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);
                        }
                        else
                        {
                            Console.WriteLine("Address: {0}", reply.Address.ToString());
                            Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                            //Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
                            // Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                            Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);
                        }
                        reply = pingSender.Send("8.8.8.8", timeout, buffer, options);
                        if (reply.Status == IPStatus.Success)
                        {
                            Console.WriteLine("Address: {0}", reply.Address.ToString());
                            Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);
                            Console.WriteLine("Time to live: {0}", reply.Options.Ttl);
                            Console.WriteLine("Don't fragment: {0}", reply.Options.DontFragment);
                            Console.WriteLine("Buffer size: {0}", reply.Buffer.Length);
                        }
                        break;

                    case "help":
                        Console.WriteLine("Наберите команду для продолжения");
                        Console.WriteLine("disp    - Вывести на экран содержимое файла \"HostDataBase.txt\"");
                        Console.WriteLine("start   - Запуск Пингера, по данным файла\"HostDataBase.txt\"");
                        Console.WriteLine("add     - для записи еще данных в конец файла, не стирая данные");
                        Console.WriteLine("add -r  - для удаления данных из файла и записи их в ручную через консоль");
                        Console.WriteLine("add -b  - будет сделан backup в \\%root_program_folder%\\Data\\backup\\%date%.txt");
                        break;

                    default:
                        Console.WriteLine("Команда введена не верно, повторите ввод \n");
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
                    separator = j;
                    if (poolLineCharFromFile[j] == ' ')
                    {
                        break;
                    }
                }

                //Если не стоит пробел после адреса хоста, своеобразная обработка исключений.
                if (separator == poolLineCharFromFile.Length - 1)
                    separator = poolLineCharFromFile.Length;


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
                    tempHostName = "\'null\'";
                if (tempDescription == "")
                    tempDescription = "\'null\'";
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
/*class Logfile
{
    public void writeErr(string catErr )
    {
        //продумать условия, при включенном пингере 23.00 - 1.00, чтобы проверял время и если число изменилось, создавал новый файл и писал уже в него
        //c фиксацией времени и даты с созданием каталогов \месяц_год\число.тхт > в самом документе %время% %имя хоста% %текст ошибки%
        if (catErr == "Warning")
        // запись в файл информации о предупреждении высокого пинга 
        else if (catErr == "not available")
        // запись в файл информации о недоступности хоста c фиксацией времени и даты
        else return;
    }     
}*/
/* 
 *
 *  займись наконец ООП и раскидай классы по файлам!
 *
 *  переработай имена в более корректные и убодные не будь ленивой задницей, думай как их сделать более удобными постоянно!!!
 *
 *  добавь ведение лога с привязкой времени 
 *
 *  сделать потоки, один для вывода на экран сообщений, второй для выхода из цикла(нажатием кнопки, вводом сообщения...??????)
 *
 *  создание отдельного класса inputHostName, тянуть из файла(xml\txt), плюс отдельная процедура для редакирования данных. 50% сделано, доделай до конца
 *  
 *  Обработка полностью пустой строки в процедуре ReadFile()
 *  
 *  последним добавить отправку сообщений на эл.почту (продумать как правильно сделать, чтобы не спамить)
 */