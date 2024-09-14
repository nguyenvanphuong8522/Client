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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Disconnect();
        }
    }

    public async Task Disconnect()
    {
        byte[] content = MessagePackSerializer.Serialize(new MessageBase(playerManager.myPlayer.Id));
        byte[] result = MyUtility.ConvertFinalMessageToBytes(MyMessageType.DESTROY, content);
        await Task.Run(() => socketManager.SendMessageToServer(result));
        await UniTask.SwitchToMainThread();
        playerManager.DestroyAllPlayers();
        socketManager.CloseConnection();
        await UniTask.SwitchToThreadPool();
    }
    private async Task OnDestroy()
    {
          await Disconnect() ;
    }
}

