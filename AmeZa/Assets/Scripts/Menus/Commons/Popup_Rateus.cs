using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Rateus : GameState
{
    [SerializeField] private GameObject[] stars = null;
    [SerializeField] private Button sendButton = null;

    private void Start()
    {
        UpdateVisual();
        sendButton.SetInteractable(false);
        UiShowHide.ShowAll(transform);

        sendButton.onClick.AddListener(() =>
        {
            if (Rateus.Current > 4)
                gameManager.OpenPopup<Popup_Confirm>().Setup(111014, true, false, ok => SocialAndSharing.RateUs(null, GlobalConfig.Socials.rateUrl));
            Back();
        });
    }

    public void SetRate(int value)
    {
        Rateus.Current = value;
        sendButton.SetInteractable(true);
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        for (int i = 0; i < stars.Length; i++)
            stars[i].SetActive(i < Rateus.Current);
    }

    public override void Back()
    {
        Rateus.Joy *= -1;
        base.Back();
    }
}


public static class Rateus
{
    public static int Current
    {
        get { return PlayerPrefs.GetInt("Rateing.Current", 1); }
        set { PlayerPrefs.SetInt("Rateing.Current", value); }
    }

    public static int Joy
    {
        get { return PlayerPrefs.GetInt("Rateing.Joy", 0); }
        set
        {
            PlayerPrefs.SetInt("Rateing.Joy", value);
            if (value >= 4 && Current < 2)
                Game.Instance.OpenPopup<Popup_Rateus>();
        }
    }
}
