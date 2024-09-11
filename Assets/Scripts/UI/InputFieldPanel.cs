using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputFieldPanel : MonoBehaviour
{
    private string _userName;

    private string _password;

    [SerializeField] private TMP_InputField inputFieldUserName;
    [SerializeField] private TMP_InputField inputFieldPassword;

    public string UserName
    {
        get => _userName = inputFieldUserName.text;
    }

    public string Password
    {
        get => _password = inputFieldPassword.text;
    }
}
