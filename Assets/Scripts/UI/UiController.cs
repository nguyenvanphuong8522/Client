using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiController : MonoBehaviour
{
    public static UiController instance;

    [SerializeField]  private Menu panelMenu;

    [SerializeField] private PanelSignIn panelSignIn;

    [SerializeField] private PanelSignUp panelSignUp;

    [SerializeField] private PanelInGame panelInGame;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(gameObject);
        
    }
    public void ShowPanelSignIn()
    {
        panelSignIn.Show();
    }

    public void ShowPanelSignUp()
    {
        panelSignUp.Show();
    }

    public void ShowPanelInGame()
    {
        panelMenu.Hide();
        panelInGame.Show();
    }

}
