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
using System.Net.Http;

public class Client : MonoBehaviour
{
    static IPEndPoint ipEndPoint;

    private Socket clientSockets;

    private SpawnManager spawnManager;

    private List<Player> listOfPlayer;

    private Player myPlayer;

    private bool canPlay;

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

        string result = JsonConvert.SerializeObject(new MyVector3());
        SendMessageToServer(result);
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
    private string[] StringSplitArray(string message) => message.Split('@').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

    private async Task HandleOneMessage(string message)
    {
        if (Equals(message, '@')) return;
        if (message[0] != '{')
        {
            chatRoom.UpdateContentChatBox(message);
            Debug.Log(message);
        }
        MyVector3 dataNewPlayer = JsonConvert.DeserializeObject<MyVector3>(message);

        if (dataNewPlayer != null)
        {
            if (dataNewPlayer.type == RequestType.CREATE)
            {
                Player newPlayer = CreatePlayer(dataNewPlayer);
                return;
            }
            else if (dataNewPlayer.type == RequestType.POSITION)
            {
                int _id = dataNewPlayer.id;
                Player player = listOfPlayer.Find(x => x.Id == _id);
                if (player != null && player.Id != myPlayer.Id)
                {
                    await UniTask.SwitchToMainThread();
                    player.transform.position = new Vector3(dataNewPlayer.x, dataNewPlayer.y, dataNewPlayer.z);
                    await UniTask.SwitchToThreadPool();
                }
            }
        }
    }

    private void HandleManyMessage(string messages)
    {
        foreach (string message in StringSplitArray(messages))
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
        myPlayer.moveHorizontal = Input.GetAxis("Horizontal");
        myPlayer.moveVertical = Input.GetAxis("Vertical");
    }
    private void FixedUpdate()
    {
        if (!canPlay) return;
        if (myPlayer.moveHorizontal != 0 || myPlayer.moveVertical != 0)
        {
            string result = ConvertToMyVector3(myPlayer, RequestType.POSITION);
            SendMessageToServer(result);
        }
    }

    private string ConvertToMyVector3(Player player, RequestType type)
    {
        Vector3 curPos = player.transform.position;
        MyVector3 newVector3 = new MyVector3(curPos.x, curPos.y, curPos.z, type);
        newVector3.id = player.Id;
        return JsonConvert.SerializeObject(newVector3);
    }

    private Player CreatePlayer(MyVector3 data)
    {
        if (!HasPlayer(data.id))
        {
            Vector3 newPos = new Vector3(data.x, data.y, data.z);
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
        MyVector3 newVector3 = new MyVector3(curPos.x, curPos.y, curPos.z, RequestType.POSITION);
        newVector3.id = player.Id;
        string result = JsonConvert.SerializeObject(newVector3);
        return result;
    }

    private void OnDestroy()
    {
        MyVector3 newVector3 = new MyVector3(0, 0, 0, RequestType.DESTROY);
        newVector3.id = myPlayer.Id;
        SendMessageToServer(JsonConvert.SerializeObject(newVector3));
        clientSockets.Shutdown(SocketShutdown.Both);
    }
}
