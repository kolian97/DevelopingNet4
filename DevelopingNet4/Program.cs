using System;
using System.Net.Sockets;
using System.Threading;

namespace DevelopingNet4
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                Server.RunServer(args[0]);
            }
            else if (args.Length == 3)
            {
                int clientPort = int.Parse(args[2]);
                Client.udpClientClient = new UdpClient(clientPort);

                new Thread(() => Client.ClientListener(args[0], args[1])).Start();
                Client.ClientSender(args[0], args[1]);
            }
            else
            {
                Console.WriteLine("Для запуска сервера введите ник-нейм как параметр запуска приложения");
                Console.WriteLine("Для запуска клиента введите ник-нейм, IP сервера и порт как параметры запуска приложения");
            }
        }
    }
}



