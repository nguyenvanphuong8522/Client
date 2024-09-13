using Cysharp.Threading.Tasks;
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
    public PlayerManager playerManager;

    public bool canPlay;

    private ApiClient apiClient;

    public MessageHandler messageHandler;

    public SocketManager socketManager;


    private void Awake()
    {
        apiClient = GetComponent<ApiClient>();
    }

    public async void Connect()
    {
        bool exists = await apiClient.Login();

        if (exists)
        {
            await socketManager.InitSocket();
            Task taskWaitConnect = socketManager.WaitReceiveRequest();
            UiController.instance.ShowPanelInGame();
            return;
        }
        Debug.LogError("Invalid User or Password");
    }

    public void Disconnect()
    {
        string content = MyUtility.ConvertToMessagePosition(playerManager.myPlayer.Id, new MyVector3());
        socketManager.SendMessageToServer(MyUtility.ConvertToDataRequestJson(content, MyMessageType.DESTROY));
        socketManager.CloseConnection();
    }
    private void OnDestroy()
    {
        Disconnect();
    }
}

