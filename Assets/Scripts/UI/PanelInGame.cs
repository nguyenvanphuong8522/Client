using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelInGame : MonoBehaviour, IPanel
{
    [SerializeField] private Button btnChat;

    [SerializeField] private PanelChat panelChat;

    private void Awake()
    {
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
