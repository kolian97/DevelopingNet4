using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using static DevelopingNet4.Message;

namespace DevelopingNet4
{
    public class Client
    {
        public static UdpClient udpClientClient = new UdpClient();
        public static void ClientListener(string name, string ip)
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), 12345);

            while (true)
            {
                try
                {
                    byte[] receiveBytes = udpClientClient.Receive(ref remoteEndPoint);
                    string receivedData = Encoding.ASCII.GetString(receiveBytes);
                    var messageReceived = Message.FromJson(receivedData);

                    Console.WriteLine($"Получено сообщение от {messageReceived.FromName} ({messageReceived.Date}): {messageReceived.Text}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при получении сообщения: " + ex.Message);
                }
            }
        }
        public static void ClientSender(string name, string ip)
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), 12345);
            while (true)
            {
                try
                {
                    Console.Write("Введите имя получателя и сообщение: ");
                    var input = Console.ReadLine().Split(' ', 2);
                    if (input.Length < 2) continue;
                    var message = new EncryptedMessageDecorator(new Message
                    {
                        Date = DateTime.Now,
                        FromName = name,
                        ToName = input[0],
                        Text = input[1]
                    }).ToJson();

                    byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                    udpClientClient.Send(messageBytes, messageBytes.Length, remoteEndPoint);
                    Console.WriteLine("Сообщение отправлено.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при отправке сообщения: " + ex.Message);
                }
            }
        }
    }
}