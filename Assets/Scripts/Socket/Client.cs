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
        ipEndPoint = new(IPAddress.Parse("192.168.1.25"), 8522);
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
        clientSockets = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        await clientSockets.ConnectAsync(ipEndPoint);

        string result = JsonConvert.SerializeObject(new PlayerPosition());
        MyDataRequest newDataRequest = new MyDataRequest();
        newDataRequest.MyRequestType = MyMessageType.CREATE;
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
        MyDataRequest dataRequest = JsonConvert.DeserializeObject<MyDataRequest>(message);


        switch (dataRequest.MyRequestType)
        {
            case MyMessageType.CREATE:
                PlayerPosition dataNewPlayer = JsonConvert.DeserializeObject<PlayerPosition>(dataRequest.Content);
                Player newPlayer = CreatePlayer(dataNewPlayer);
                break;
            case MyMessageType.POSITION:
                PlayerPosition dataNewPlayer2 = JsonConvert.DeserializeObject<PlayerPosition>(dataRequest.Content);
                int _id = dataNewPlayer2.id;
                Player player = listOfPlayer.Find(x => x.Id == _id);
                if (player != null && player.Id != myPlayer.Id)
                {
                    await UniTask.SwitchToMainThread();
                    player.UpdatePosition(new Vector3(dataNewPlayer2.Vector3.x, dataNewPlayer2.Vector3.y, dataNewPlayer2.Vector3.z));
                    await UniTask.SwitchToThreadPool();
                }
                break;
            case MyMessageType.TEXT:
                chatRoom.UpdateContentChatBox(dataRequest.Content);
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

    private void Update()
    {
        if (!canPlay) return;
        myPlayer.horizontalInput = Input.GetAxis("Horizontal");
        myPlayer.verticalInput = Input.GetAxis("Vertical");
    }
    private void FixedUpdate()
    {
        if (!canPlay) return;
        if (myPlayer.horizontalInput != 0 || myPlayer.verticalInput != 0)
        {
            string result = ConvertToMyVector3(myPlayer, MyMessageType.POSITION);
            SendMessageToServer(result);
        }
    }

    public string ConvertToMyVector3(Player player, MyMessageType type)
    {
        Vector3 curPos = player.transform.position;
        PlayerPosition newVector3 = new PlayerPosition(player.Id, ConvertToMyvector3(curPos));
        newVector3.id = player.Id;
        MyDataRequest newDataRequest = new MyDataRequest();
        newDataRequest.MyRequestType = type;
        newDataRequest.Content = JsonConvert.SerializeObject(newVector3);
        return JsonConvert.SerializeObject(newDataRequest);
    }


    private Player CreatePlayer(PlayerPosition data)
    {
        if (!HasPlayer(data.id))
        {
            Vector3 newPos = new Vector3(data.Vector3.x, data.Vector3.y, data.Vector3.z);
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
        PlayerPosition newPlayerposition = new PlayerPosition(player.Id, ConvertToMyvector3(curPos));
        string result = JsonConvert.SerializeObject(newPlayerposition);
        return result;
    }

    public MyVector3 ConvertToMyvector3(Vector3 oldVector)
    {
        MyVector3 myVector3 = new MyVector3(oldVector.x, oldVector.y, oldVector.z);
        return myVector3;
    }

    private void OnDestroy()
    {
        PlayerPosition newPlayerPosition = new PlayerPosition();
        newPlayerPosition.id = myPlayer.Id;
        MyDataRequest dataRequest = new MyDataRequest();
        dataRequest.Content = JsonConvert.SerializeObject(newPlayerPosition);
        SendMessageToServer(JsonConvert.SerializeObject(dataRequest));
        clientSockets.Shutdown(SocketShutdown.Both);
    }
}
