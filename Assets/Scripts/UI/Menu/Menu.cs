using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour, IPanel
{
    [SerializeField] private Button btnSignIn;

    [SerializeField] private Button btnSignUp;

    private UiController uiController;

    private void Start()
    {
        uiController = UiController.instance;
        btnSignIn.onClick.AddListener(OnBtnSignIn);
        btnSignUp.onClick.AddListener(OnBtnSignUp);
    }

    private void OnBtnSignIn()
    {
        uiController.ShowPanelSignIn();
    }

    private void OnBtnSignUp()
    {
        uiController.ShowPanelSignUp();
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
