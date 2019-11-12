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

        levelsButton.onClick.AddListener(() =>
        {
            PlayModel.ballId = Profile.Avatar.ballId;
            gameManager.OpenState<State_Playing>();
        });// { gameManager.OpenState<State_Levels>(); });
        shopButton.onClick.AddListener(() => { gameManager.OpenPopup<Popup_Shop>(); });

        UIBackground.Show();
        UiShowHide.ShowAll(transform);
    }
}
