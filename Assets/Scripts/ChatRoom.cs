using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatRoom : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button btnSend;
    [SerializeField] private Client client;
    [SerializeField] private Text textBoard;
    public string allMessage;

    private void Awake()
    {
        btnSend.onClick.AddListener(OnSendMessage);
    }

    public void UpdateTextBoard(string newMessage)
    {
        textBoard.text += $"{newMessage}\n";
    }

    private void OnSendMessage()
    {
        string message = inputField.text;
        //Debug.Log(inputField.text);
        client.SendMessageToServer(message);
    }
}
