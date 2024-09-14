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
using MessagePack;
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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            Disconnect();
        }
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

    public async Task Disconnect()
    {
        byte[] content = MessagePackSerializer.Serialize(new MessageBase(playerManager.myPlayer.Id));
        byte[] result = MyUtility.SendMessageConverted(MyMessageType.DESTROY, content);
        socketManager.SendMessageToServer(result);
    }
    private async void OnDestroy()
    {
        await Disconnect();
    }
}

