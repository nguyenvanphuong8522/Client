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

    #region Init
    static IPEndPoint ipEndPoint = new(IPAddress.Parse("192.168.1.25"), 8522);

    private Socket clientSockets;

    private SpawnManager spawnManager;

    private List<Player> listOfPlayer = new List<Player>();

    private Player myPlayer;

    private bool canMove;

    //private bool isConnected;

    private string userName;

    private string password;

    private List<Category> categories = new List<Category>();

    [SerializeField] private SignInUi ui;
    private void Awake()
    {
        spawnManager = GetComponent<SpawnManager>();
    }

    public void SetNamePassword(string username, string password)
    {
        this.userName = username;
        this.password = password;
    }


    #endregion

    #region Call API
    private HttpClient _httpClient;

    public async Task CallApi()
    {
        _httpClient = new HttpClient();
        await CallRestApiAsync();

    }

    public async Task CallRestApiAsync()
    {
        string url = "https://localhost:7087/api/category";

        HttpResponseMessage response = await _httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            categories = JsonConvert.DeserializeObject<List<Category>>(responseBody);
        }
        else
        {
            Debug.LogError($"failed: {response.StatusCode}");
        }
    }
    #endregion



    private async void Start()
    {
        if (categories.Count == 0)
        {
            await CallApi();
        }
    }

    public async void Connect()
    {
        bool exists = categories.Any(c => c.UserName == userName && c.Password == password);

        if (exists)
        {
            ui.Hide();
            await InitSocket();
            Task taskWaitConnect = WaitConnect();
            return;
        }
        Debug.LogError("Invalid User or Password");
    }

    private async Task InitSocket()
    {
        clientSockets = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        await clientSockets.ConnectAsync(ipEndPoint);
        //isConnected = true;

        string result = JsonConvert.SerializeObject(new MyVector3());
        SendMessageToServer(result);
    }

    private async Task WaitConnect()
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
        Debug.Log(message);
        if (Equals(message, '@')) return;
        MyVector3 dataNewPlayer = JsonConvert.DeserializeObject<MyVector3>(message);

        if (dataNewPlayer != null)
        {
            if (dataNewPlayer.type == RequestType.CREATE)
            {
                Player newPlayer = CreatePlayer(dataNewPlayer);
                return;
            }
            else if(dataNewPlayer.type == RequestType.POSITION)
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

    private void SendMessageToServer(string message)
    {
        var sendBuffer = Encoding.UTF8.GetBytes(message + '@');
        clientSockets.Send(sendBuffer);
    }

    private void Update()
    {
        if (!canMove) return;
        myPlayer.moveHorizontal = Input.GetAxis("Horizontal");
        myPlayer.moveVertical = Input.GetAxis("Vertical");

        string result = ConvertToMyVector3(myPlayer, RequestType.POSITION);
        SendMessageToServer(result);

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
            canMove = true;
            return newPlayer;
        }
        return null;
    }

    private bool HasPlayer(int id)
    {
        Player player = listOfPlayer.Find(x => x.Id == id);
        return player != null ? true : false;
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
