using MessagePack;
using MyLibrary;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelChat : MonoBehaviour, IPanel
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button btnSend;
    [SerializeField] private Client client;
    [SerializeField] private Text chatBox;

    [SerializeField] private Button btnClose;

    private void Awake()
    {
        btnClose.onClick.AddListener(Hide);
        btnSend.onClick.AddListener(OnSendMessage);
    }

    public void UpdateContentChatBox(string newMessage)
    {
        chatBox.text += $"{newMessage}\n";
    }

    private void OnSendMessage()
    {
        string message = inputField.text;
        
        MessageText messageText = new MessageText(client.playerManager.myPlayer.Id, message);

        byte[] data = MessagePackSerializer.Serialize(messageText);

        client.socketManager.SendMessageToServer(client.messageHandler.SendMessageConverted(MyMessageType.TEXT, data));
    }


    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
