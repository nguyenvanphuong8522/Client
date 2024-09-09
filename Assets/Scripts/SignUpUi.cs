using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUpUi : MonoBehaviour
{
    [SerializeField] private Client client;

    [SerializeField] private Button btnSignUp;

    [SerializeField] private TMP_InputField inputFieldUserName;
    [SerializeField] private TMP_InputField inputFieldPassword;

    private string userName;
    
    private string password;



    private void Awake()
    {
        btnSignUp.onClick.AddListener(OnSignUp);
    }
    private void OnSignUp()
    {
        userName = inputFieldUserName.text;
        password = inputFieldPassword.text;
        if(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            Debug.Log("Empty username or password");
            return;
        }
        client.PostData(userName, password);
    }
}
