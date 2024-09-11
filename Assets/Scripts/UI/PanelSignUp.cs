using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelSignUp : MonoBehaviour, IPanel
{
    [SerializeField] private ApiClient apiClient;

    [SerializeField] private Button btnSignUp;

    private InputFieldPanel inputFieldPanel;

    [SerializeField] private Button btnClose;


    private void Awake()
    {
        inputFieldPanel = GetComponentInChildren<InputFieldPanel>(true);
        btnClose.onClick.AddListener(Hide);
        btnSignUp.onClick.AddListener(OnSignUp);
    }
    private void OnSignUp()
    {
        string userName = inputFieldPanel.UserName;
        string password = inputFieldPanel.Password;
        if(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            Debug.Log("Empty username or password");
            return;
        }
        apiClient.SetNamePassword(userName, password);
        apiClient.SignUp();
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
