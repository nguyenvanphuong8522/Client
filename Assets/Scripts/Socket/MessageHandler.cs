using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MyLibrary;
using MessagePack;
using System;

public class MessageHandler : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PanelChat chatRoom;
    [SerializeField] private SocketManager socketManager;

    public MyMessageType ByteToType(byte value)
    {
        byte[] bytes = { value };
        return ByteToType(bytes);
    }
    public MyMessageType ByteToType(byte[] value)
    {
        MyMessageType type = MessagePackSerializer.Deserialize<MyMessageType>(value);
        return type;
    }

    public async Task HandleOneMessage(byte[] data, MyMessageType type)
    {
        switch (type)
        {
            case MyMessageType.CREATE:
                MessagePosition dataNewPlayer = MessagePackSerializer.Deserialize<MessagePosition>(data);
                playerManager.CreatePlayer(dataNewPlayer);
                break;

            case MyMessageType.POSITION:
                MessagePosition newMessagePosition = MessagePackSerializer.Deserialize<MessagePosition>(data);
                Player player = playerManager.HasPlayer(newMessagePosition.id) ? playerManager.listOfPlayer.Find(x => x.Id == newMessagePosition.id) : null;

                if (player != null && player.Id != playerManager.myPlayer.Id)
                {
                    await UniTask.SwitchToMainThread();
                    player.UpdatePosition(new Vector3(newMessagePosition.Position.x, newMessagePosition.Position.y, newMessagePosition.Position.z));
                    await UniTask.SwitchToThreadPool();
                }
                break;

            case MyMessageType.TEXT:
                MessageText messageText = MessagePackSerializer.Deserialize<MessageText>(data);
                chatRoom.UpdateContentChatBox(messageText.text);
                break;
            case MyMessageType.DESTROY:
                MessageBase messageDestroy = MessagePackSerializer.Deserialize<MessageBase>(data);
                Debug.Log($"Client[{messageDestroy.id}] disconnected!");
                await UniTask.SwitchToMainThread();
                playerManager.RemovePlayer(messageDestroy.id);
                await UniTask.SwitchToThreadPool();
                break;
            default:
                break;

        }
    }
}
