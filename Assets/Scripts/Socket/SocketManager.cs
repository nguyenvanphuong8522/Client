using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MyLibrary;

public class SocketManager
{
    public Socket clientSocket;
    private IPEndPoint ipEndPoint;

    public SocketManager(string ipAddress, int port)
    {
        ipEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        clientSocket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    }

    public async Task InitSocket()
    {
        await clientSocket.ConnectAsync(ipEndPoint);
        SendMessageToServer(MyUtility.ConvertToDataRequestJson(JsonConvert.SerializeObject(new MessagePosition()), MyMessageType.CREATE));
    }

    public void StartListening(Action<string> handleMessage)
    {
        Task.Run(async () =>
        {
            while (true)
            {
                var buffer = new byte[1024];
                int receivedLength = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
                if (receivedLength == 0) return;

                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, receivedLength);
                handleMessage(receivedMessage);
            }
        });
    }

    public void SendMessageToServer(string message)
    {
        byte[] sendBuffer = Encoding.UTF8.GetBytes($"{message}@");
        clientSocket.Send(sendBuffer);
    }

    public void CloseConnection()
    {
        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
    }
}
