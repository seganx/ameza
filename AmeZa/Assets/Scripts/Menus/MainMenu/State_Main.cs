using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class State_Main : GameState
{
    [SerializeField] private Button onlineButton = null;
    [SerializeField] private Button levelsButton = null;
    [SerializeField] private Button shopButton = null;

    private void Start()
    {
        onlineButton.onClick.AddListener(() =>
        {
            gameManager.OpenPopup<Popup_Rewards>().Setup(100, 0);
            gameManager.OpenState<State_SelectLeague>();
        });

        levelsButton.onClick.AddListener(() => gameManager.OpenState<State_Levels>());
        shopButton.onClick.AddListener(() => { gameManager.OpenPopup<Popup_Shop>(); });

        UIBackground.Show();
        UiShowHide.ShowAll(transform);
    }

    public override void Back()
    {
        gameManager.OpenPopup<Popup_Confirm>().Setup(111004, true, true, yes =>
        {
            if (yes)
                Application.Quit();
        });
    }
}
