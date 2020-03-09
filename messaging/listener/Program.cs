using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using StackExchange.Redis;

public class UDPListener
{
    private const int ListenPort = 11000;

    private static void StartListener()
    {
        IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, ListenPort);
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

        var listener = new UdpClient(groupEP);
        try
        {
            while (true)
            {
                Console.WriteLine("Waiting for broadcast");
                byte[] bytes = listener.Receive(ref groupEP);

                Console.WriteLine($"Received broadcast from {groupEP} :");

                string message = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                Console.WriteLine($" {message}");

                redis.GetSubscriber().Publish("incoming", message);
                Console.WriteLine("Relayed incoming message to Redis 'incoming' channel");
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            listener.Close();
        }
    }

    public static void Main()
    {
        StartListener();
    }
}