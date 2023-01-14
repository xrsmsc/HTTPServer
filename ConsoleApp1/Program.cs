using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            TcpListener serverSocket = null;
            try
            {

                IPAddress localAddress = IPAddress.Parse("127.0.0.1");
                serverSocket = new TcpListener(localAddress, 80);

                serverSocket.Start();

                while (true)
                {

                    TcpClient client = serverSocket.AcceptTcpClient();

                    Thread clientThread = new Thread(ClientHandler);
                    clientThread.Start(client);

                }

            }

            catch (SocketException e)
            {


            }

            finally
            {

                serverSocket.Stop();

            }

        }

        static void ClientHandler(object o)
        {

            StringBuilder sb = new StringBuilder();

            TcpClient client = o as TcpClient;
            NetworkStream stream = client.GetStream();

            int i;
            Byte[] bytes = new Byte[256];

            while (stream.DataAvailable)
            {
                i = stream.Read(bytes, 0, bytes.Length);
                string message = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                sb.Append(message);
            }


            Console.WriteLine($"Recieved message : {sb} {DateTime.Now}");

            System.IO.DriveInfo di = new System.IO.DriveInfo(@"D:\Users\Levin\Documents\1Hochschule\FH Esslingen\HTTP");   //Set default filepath
            System.IO.DirectoryInfo dirInfo = di.RootDirectory;

            System.IO.FileInfo[] fileNames = dirInfo.GetFiles("*.*");

            //fi.Refresh

            var x = sb.ToString().Split(' ');

            if (x.Length <= 1)
            {
                return;
            }

            var resRel = x[1].Replace("/", "\\");
            


            Console.WriteLine("resRel: " + resRel);


            //D: \Users\Levin\Documents\1Hochschule\FH Esslingen\HTTPServer\HTTPServer
            //C: \Users\lesowt00\source\repos\


            //var res = Path.Combine(@"D:\Users\Levin\Documents\1Hochschule\FH Esslingen\HTTPServer\HTTPServer", resRel);

            var res = @"D:\Users\Levin\Documents\1Hochschule\FH Esslingen\HTTP";


            if (resRel.EndsWith("\\"))
            {
                res = Path.Combine(res, "index.html");

            }

            else
            {

                resRel =
                Path.GetFileName(resRel);
                res = Path.Combine(res, resRel);

            }

            Console.WriteLine("res: " + res);







            if (File.Exists(res))
            {

                if (res.EndsWith(".html"))
                {


                    using (FileStream sr = new FileStream(res, FileMode.Open))
                    {

                        Byte[] byt = new Byte[256];

                        sr.Read(byt, 0, byt.Length);

                       
                        string message = System.Text.Encoding.ASCII.GetString(byt, 0, byt.Length);

                        Console.WriteLine(message.Length);

                        Byte[] data = Encoding.ASCII.GetBytes($"HTTP/1.1 200 OK\nContent-length:{message.Length}\nContent-Type: text/html\n\n{message}"); // Infos aus Filestream


                        Console.WriteLine(message);



                        stream.Write(data, 0, data.Length);
                        stream.Dispose();
                        client.Close();


                    }


                }


                else if (res.EndsWith(".ico"))
                {


                    using (FileStream fs = new FileStream(res, FileMode.Open))
                    {


                        var data = Encoding.ASCII.GetBytes($"HTTP/1.1 200 OK\nContent-length:{fs.Length}\nContent-Type: image/x-icon\n\n"); // Infos aus Filestream




                        stream.Write(data, 0, data.Length);

                        Byte[] pic = new byte[256];
                       
                        
                        while((i = fs.Read(pic,0,pic.Length))!= 0)
                        {


                            fs.Read(pic, 0, pic.Length);

                        }


                        stream.Write(pic, 0, pic.Length);
                        stream.Dispose();
                        client.Close();


                    }


                }






            }
            else
            {

                Byte[] response = Encoding.ASCII.GetBytes($"HTTP/1.1 200 FileNotAvailable\n");
                Console.WriteLine($"Sende Antwort {Environment.NewLine}");
                stream.Write(response, 0, response.Length);

                Console.WriteLine($"Deine Datei gibbet nich {Environment.NewLine}");

            }
        }



    }
}
