using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MyLibrary;
using UnityEngine;
using Unity.VisualScripting;
using Newtonsoft.Json.Linq;
using System.Linq;
using MessagePack;
using System.Collections;

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

        SayHiServer();

    }
    public void SayHiServer()
    {
        byte[] data = MessagePackSerializer.Serialize(new MessagePosition(1, new MyVector3()));
        Debug.Log(data.Length);
        byte[] mainData = MyUtility.ConvertFinalMessageToBytes(MyMessageType.CREATE, data);
        
        SendMessageToServer(mainData);
    }


    public async Task WaitReceiveRequest()
    {
        while (true)
        {

            byte[] buffer = new byte[13];
            await socket.ReceiveAsync(buffer, SocketFlags.None);

            int length = BitConverter.ToInt32(buffer, 0);

            byte[] byteType = new byte[1];
            await socket.ReceiveAsync(byteType, SocketFlags.None);

            MyMessageType type = messageHandler.ByteToType(byteType);

            byte[] mainData = new byte[length];
            int messageCode2 = await socket.ReceiveAsync(mainData, SocketFlags.None);

            await messageHandler.HandleOneMessage(mainData, type);
        }
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
