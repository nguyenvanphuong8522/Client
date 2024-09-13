using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MyLibrary;

public class MessageHandler : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PanelChat chatRoom;


    public void HandleManyMessage(string messages)
    {
        foreach (string message in MyUtility.StringSplitArray(messages))
        {
            var _ = HandleOneMessage(message);
        }
    }

    private async Task HandleOneMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message) || message == "@") return;

        MyDataRequest data = JsonConvert.DeserializeObject<MyDataRequest>(message);

        switch (data.Type)
        {
            case MyMessageType.CREATE:
                MessagePosition dataNewPlayer = JsonConvert.DeserializeObject<MessagePosition>(data.Content);
                playerManager.CreatePlayer(dataNewPlayer);
                break;

            case MyMessageType.POSITION:
                MessagePosition newMessagePosition = JsonConvert.DeserializeObject<MessagePosition>(data.Content);
                Player player = playerManager.HasPlayer(newMessagePosition.id) ? playerManager.listOfPlayer.Find(x => x.Id == newMessagePosition.id) : null;

                if (player != null && player.Id != playerManager.myPlayer.Id)
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
        }
    }
}
