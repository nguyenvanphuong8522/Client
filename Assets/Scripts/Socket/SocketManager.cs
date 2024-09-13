using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MyLibrary;
using UnityEngine;

public class SocketManager :MonoBehaviour
{
    public Socket socket;
    private IPEndPoint ipEndPoint;
    public MessageHandler messageHandler;

    public async Task InitSocket()
    {
        ipEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.25"), 8522);
        socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        await socket.ConnectAsync(ipEndPoint);
        string content = JsonConvert.SerializeObject(new MessagePosition());
        SendMessageToServer(MyUtility.ConvertToDataRequestJson(content, MyMessageType.CREATE));
    }

    public async Task WaitReceiveRequest()
    {
        while (true)
        {
            var buffer = new byte[1024];
            int messageCode = await socket.ReceiveAsync(buffer, SocketFlags.None);
            string messageReceived = Encoding.UTF8.GetString(buffer, 0, messageCode);

            if (messageCode == 0) return;
            messageHandler.HandleManyMessage(messageReceived);
        }
    }

    public async Task SendMessageToServer(string message)
    {
        byte[] sendBuffer = Encoding.UTF8.GetBytes($"{message}@");
        await socket.SendAsync(sendBuffer, SocketFlags.None);
    }
    public void SendMessageToServer(byte[] bytes)
    {
        socket.Send(bytes);
    }

    public void CloseConnection()
    {
        socket.Close();
    }
}
