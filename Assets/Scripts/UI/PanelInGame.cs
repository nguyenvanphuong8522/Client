using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelInGame : MonoBehaviour, IPanel
{
    [SerializeField] private Button btnChat;
    [SerializeField] private Button btnDisconnect;

    [SerializeField] private PanelChat panelChat;

    [SerializeField] private Client client;

    private void Awake()
    {
        btnDisconnect.onClick.AddListener(() => {
            client.Disconnect();
            panelChat.Hide();
            UiController.instance.ShowPanelSignIn();
            Hide();
        });
        btnChat.onClick.AddListener(panelChat.Show);
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
