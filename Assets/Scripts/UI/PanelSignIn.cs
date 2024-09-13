using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelSignIn : MonoBehaviour, IPanel
{
    [SerializeField] private Button btnClose;

    [SerializeField] private ApiClient apiClient;
    [SerializeField] private Client client;

    private InputFieldPanel inputFieldPanel;

    [SerializeField] private Button btnSignIn;
     
    private void Awake()
    {
        inputFieldPanel = GetComponentInChildren<InputFieldPanel>(true);
    }

    private void Start()
    {
        btnClose.onClick.AddListener(Hide);
        btnSignIn.onClick.AddListener(OnBtnSignIn);
    }

    private void OnBtnSignIn()
    {
        string _userName = inputFieldPanel.UserName;
        string _password = inputFieldPanel.Password;
        //Debug.Log($"Username: {_userName} - Password: {_password}");
        apiClient.SetNamePassword(_userName, _password);
        client.Connect();
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
