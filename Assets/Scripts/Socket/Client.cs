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
    static IPEndPoint ipEndPoint;

    public PlayerManager playerManager;

    public bool canPlay;

    [SerializeField] private PanelChat chatRoom;

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
        newMessagePosition.id = playerManager.myPlayer.Id;
        MyDataRequest dataRequest = new MyDataRequest();
        dataRequest.Content = JsonConvert.SerializeObject(newMessagePosition);
        dataRequest.Type = MyMessageType.DESTROY;
        socketManager.SendMessageToServer(JsonConvert.SerializeObject(dataRequest));
        socketManager.CloseConnection();
    }
}

