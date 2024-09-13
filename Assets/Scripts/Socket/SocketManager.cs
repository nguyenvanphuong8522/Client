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
        //string content = JsonConvert.SerializeObject(new MessagePosition());
        //byte[] mainData = MyUtility.ConvertToDataRequestByte(content, MyMessageType.CREATE);
        //int length = mainData.Length;
        //byte[] byteLenght = BitConverter.GetBytes(length);
        //byte[] combinedArray = byteLenght.Concat(mainData).ToArray();
        



    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SendMessageToServer();
        }
    }
    public void SendMessageToServer()
    {
        byte[] byteLength1 = new byte[10];
        byte[] intBytes1 = BitConverter.GetBytes(60);
        Buffer.BlockCopy(intBytes1, 0, byteLength1, 0, intBytes1.Length);

        byte[] byteLength2 = new byte[20];
        byte[] intBytes2 = BitConverter.GetBytes(60);
        Buffer.BlockCopy(intBytes2, 0, byteLength2, 0, intBytes2.Length);

        // Tạo mảng byte kết hợp
        byte[] combinedArray = new byte[byteLength1.Length + byteLength2.Length];

        // Sao chép mảng byteLength1 vào combinedArray
        Buffer.BlockCopy(byteLength1, 0, combinedArray, 0, byteLength1.Length);

        // Sao chép mảng byteLength2 vào combinedArray, bắt đầu từ vị trí tiếp theo sau byteLength1
        Buffer.BlockCopy(byteLength2, 0, combinedArray, byteLength1.Length, byteLength2.Length);

        // In kích thước mảng để kiểm tra
        Debug.Log($"combinedArray.Length: {combinedArray.Length}");
        SendMessageToServer(combinedArray);
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
