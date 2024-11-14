using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace DevelopingNet4
{
    public class Server
    {
        private static UdpClient udpClient;
        private static Dictionary<string, IPEndPoint> clients = new Dictionary<string, IPEndPoint>();
        public static void RunServer(string name)
        {
            udpClient = new UdpClient(12345);
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            Console.WriteLine("UDP Сервер ожидает сообщений...");

            while (true)
            {
                try
                {
                    byte[] receiveBytes = udpClient.Receive(ref remoteEndPoint);
                    string receivedData = Encoding.ASCII.GetString(receiveBytes);
                    var message = Message.FromJson(receivedData);

                    Console.WriteLine($"Получено сообщение от {message.FromName} для {message.ToName} ({message.Date}): {message.Text}");

                    string replyMessage = ProcessMessage(message, remoteEndPoint, name);
                    SendReply(replyMessage, remoteEndPoint, name);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при обработке сообщения: " + ex.Message);
                }
            }
        }

        private static string ProcessMessage(Message message, IPEndPoint remoteEndPoint, string serverName)
        {
            string replyMessage = "";

            if (message.ToName == "Server")
            {
                switch (message.Text)
                {
                    case "register":
                        clients[message.FromName] = remoteEndPoint;
                        replyMessage = "registered";
                        break;
                    case "delete":
                        clients.Remove(message.FromName);
                        replyMessage = "deleted";
                        break;
                    case "list":
                        replyMessage = string.Join("|", clients.Keys);
                        break;
                }
            }
            else if (clients.TryGetValue(message.ToName, out IPEndPoint clientEndPoint))
            {
                ForwardMessage(message, clientEndPoint);
                replyMessage = "sent";
            }
            else
            {
                replyMessage = "user not found";
            }

            return replyMessage;
        }
        private static void ForwardMessage(Message message, IPEndPoint clientEndPoint)
        {
            var forwardMessage = new Message
            {
                Date = DateTime.Now,
                FromName = message.FromName,
                Text = message.Text
            }.ToJson();

            byte[] forwardBytes = Encoding.ASCII.GetBytes(forwardMessage);
            udpClient.Send(forwardBytes, forwardBytes.Length, clientEndPoint);
            Console.WriteLine("Ответ переслан.");
        }

        private static void SendReply(string replyMessage, IPEndPoint remoteEndPoint, string serverName)
        {
            var reply = new Message
            {
                Date = DateTime.Now,
                FromName = serverName,
                Text = replyMessage
            }.ToJson();
            byte[] replyBytes = Encoding.ASCII.GetBytes(reply);
            udpClient.Send(replyBytes, replyBytes.Length, remoteEndPoint);
            Console.WriteLine("Ответ отправлен.");
        }
    }
}
