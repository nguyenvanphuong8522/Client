using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MyLibrary;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class SocketManager :MonoBehaviour
{
    public Socket clientSocket;
    private IPEndPoint ipEndPoint;
    public MessageHandler messageHandler;

    private void Awake()
    {
        ipEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.25"), 8522);
        clientSocket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    }

    public async Task InitSocket()
    {
        await clientSocket.ConnectAsync(ipEndPoint);
        SendMessageToServer(MyUtility.ConvertToDataRequestJson(JsonConvert.SerializeObject(new MessagePosition()), MyMessageType.CREATE));
    }

    public async Task WaitReceiveRequest()
    {
        while (true)
        {
            var buffer = new byte[1024];
            int messageCode = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
            string messageReceived = Encoding.UTF8.GetString(buffer, 0, messageCode);

            if (messageCode == 0) return;
            messageHandler.HandleManyMessage(messageReceived);
        }
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
