using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignInUi : MonoBehaviour
{
    [SerializeField] private Client client;
    [SerializeField] private TMP_InputField inputFieldUserName;
    [SerializeField] private TMP_InputField inputFieldPassword;

    private string _userName;
    public string _password;

    [SerializeField] private Button btnSignIn;
    [SerializeField] private GameObject btnChatRoom;
     
    private void Awake()
    {
        btnSignIn.onClick.AddListener(OnBtnSignIn);
    }

    private void OnBtnSignIn()
    {
        _userName = inputFieldUserName.text;
        _password = inputFieldPassword.text;
        Debug.Log($"Username: {_userName} - Password: {_password}");
        client.SetNamePassword(_userName, _password);
        client.Connect();
    }
    public void ShowBtnChatRoom()
    {
        btnChatRoom.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
