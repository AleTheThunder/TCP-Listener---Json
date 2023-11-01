using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace TCP_Listener___Json
{
    class Program
    {
        static void Main()
        {
            TcpListener server = null;
            string password = "meinPasswort";

            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                int port = 8080;
                server = new TcpListener(localAddr, port);
                server.Start();

                Console.WriteLine($"Server gestartet auf {localAddr}:{port}");

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Verbunden mit einem Client");

                    NetworkStream stream = client.GetStream();

                    byte[] buffer = new byte[256];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string receivedPassword = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    if (receivedPassword == password)
                    {
                        Console.WriteLine("Passwort korrekt!");

                        // Sende JSON-Daten
                        Person person = new Person { Name = "Max", Alter = 30 };
                        string json = JsonConvert.SerializeObject(person);
                        byte[] jsonBytes = Encoding.ASCII.GetBytes(json);
                        stream.Write(jsonBytes, 0, jsonBytes.Length);
                    }
                    else
                    {
                        Console.WriteLine("Falsches Passwort!");
                        byte[] failureMessage = Encoding.ASCII.GetBytes("Falsches Passwort!");
                        stream.Write(failureMessage, 0, failureMessage.Length);
                    }

                    client.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server.Stop();
            }
        }

        public class Person
        {
            public string Name { get; set; }
            public int Alter { get; set; }
        }
    }
}
