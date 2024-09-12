using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using MyLibrary;
public class Client : MonoBehaviour
{
    static IPEndPoint ipEndPoint;

    private Socket clientSockets;

    private SpawnManager spawnManager;

    private List<Player> listOfPlayer;

    public Player myPlayer;

    public bool canPlay;

    [SerializeField] private PanelChat chatRoom;

    private ApiClient apiClient;

    private void Awake()
    {
        apiClient = GetComponent<ApiClient>();
        listOfPlayer = new List<Player>();
        spawnManager = GetComponent<SpawnManager>();
    }

    public async void Connect()
    {
        bool exists = await apiClient.Login();

        if (exists)
        {
            await InitSocket();
            Task taskWaitConnect = WaitReceiveRequest();
            UiController.instance.ShowPanelInGame();
            return;
        }
        Debug.LogError("Invalid User or Password");
    }

    private async Task InitSocket()
    {
        ipEndPoint = new(IPAddress.Parse("192.168.1.25"), 8522);
        clientSockets = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        await clientSockets.ConnectAsync(ipEndPoint);

        string result = JsonConvert.SerializeObject(new MessagePosition());
        MyDataRequest newDataRequest = new MyDataRequest();
        newDataRequest.Type = MyMessageType.CREATE;
        newDataRequest.Content = result;
        SendMessageToServer(JsonConvert.SerializeObject(newDataRequest));
    }

    private async Task WaitReceiveRequest()
    {
        while (true)
        {
            var buffer = new byte[1024];
            int messageCode = await clientSockets.ReceiveAsync(buffer, SocketFlags.None);
            string messageReceived = Encoding.UTF8.GetString(buffer, 0, messageCode);

            if (messageCode == 0) return;
            HandleManyMessage(messageReceived);
        }
    }
    private async Task HandleOneMessage(string message)
    {
        if (Equals(message, '@')) return;
        MyDataRequest data = JsonConvert.DeserializeObject<MyDataRequest>(message);


        switch (data.Type)
        {
            case MyMessageType.CREATE:
                MessagePosition dataNewPlayer = JsonConvert.DeserializeObject<MessagePosition>(data.Content);
                Player newPlayer = CreatePlayer(dataNewPlayer);
                break;
            case MyMessageType.POSITION:
                MessagePosition newMessagePosition = JsonConvert.DeserializeObject<MessagePosition>(data.Content);
                int _id = newMessagePosition.id;
                Player player = listOfPlayer.Find(x => x.Id == _id);
                if (player != null && player.Id != myPlayer.Id)
                {
                    await UniTask.SwitchToMainThread();
                    player.UpdatePosition(new Vector3(newMessagePosition.Position.x, newMessagePosition.Position.y, newMessagePosition.Position.z));
                    await UniTask.SwitchToThreadPool();
                }
                break;
            case MyMessageType.TEXT:
                MessageText messageText = JsonConvert.DeserializeObject<MessageText>(data.Content);

                chatRoom.UpdateContentChatBox(messageText.text);
                break;
            default:
                break;
        }
    }

    private void HandleManyMessage(string messages)
    {
        foreach (string message in MyUtility.StringSplitArray(messages))
        {
            var t = HandleOneMessage(message);
        }
    }

    public void SendMessageToServer(string message)
    {
        var sendBuffer = Encoding.UTF8.GetBytes($"{message}@");
        clientSockets.Send(sendBuffer);
    }

    public string ConvertToMessagePosition(Player player, MyMessageType type)
    {
        Vector3 curPos = player.transform.position;
        MessagePosition MessagePosition = new MessagePosition(player.Id, ClientUtility.ConvertToMyvector3(curPos));
        MessagePosition.id = player.Id;

        MyDataRequest newDataRequest = new MyDataRequest();
        newDataRequest.Type = type;
        newDataRequest.Content = JsonConvert.SerializeObject(MessagePosition);

        return JsonConvert.SerializeObject(newDataRequest);
    }


    private Player CreatePlayer(MessagePosition data)
    {
        if (!HasPlayer(data.id))
        {
            Vector3 newPos = new Vector3(data.Position.x, data.Position.y, data.Position.z);
            Player newPlayer = spawnManager.GetPrefab(0, newPos);
            newPlayer.Id = data.id;

            if (myPlayer == null)
            {
                myPlayer = newPlayer;
            }
            listOfPlayer.Add(newPlayer);
            Debug.LogWarning("Created New Player");
            canPlay = true;
            return newPlayer;
        }
        return null;
    }

    private bool HasPlayer(int id)
    {
        Player player = listOfPlayer.Find(x => x.Id == id);
        return player != null;
    }

    private string ConvertToJson(Player player)
    {
        Vector3 curPos = player.transform.position;
        MessagePosition newMessagePosition = new MessagePosition(player.Id, ClientUtility.ConvertToMyvector3(curPos));
        string result = JsonConvert.SerializeObject(newMessagePosition);
        return result;
    }

    private void OnDestroy()
    {
        MessagePosition newMessagePosition = new MessagePosition();
        newMessagePosition.id = myPlayer.Id;
        MyDataRequest dataRequest = new MyDataRequest();
        dataRequest.Content = JsonConvert.SerializeObject(newMessagePosition);
        dataRequest.Type = MyMessageType.DESTROY;
        SendMessageToServer(JsonConvert.SerializeObject(dataRequest));
        clientSockets.Shutdown(SocketShutdown.Both);
    }
}
