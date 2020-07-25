using SeganX;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Rateus : GameState
{
    [SerializeField] private Button closeButton = null;
    [SerializeField] private GameObject[] stars = null;
    [SerializeField] private Button sendButton = null;

    private System.Action onCloseFunc = null;

    public Popup_Rateus Setup(System.Action onClose)
    {
        onCloseFunc = onClose;
        return this;
    }

    private void Start()
    {
        UpdateVisual();
        sendButton.SetInteractable(false);
        UiShowHide.ShowAll(transform);

        sendButton.onClick.AddListener(() =>
        {
            if (Rateus.Current > 4)
            {
                game.OpenPopup<Popup_Confirm>().Setup(111014, true, false, ok =>
                {
                    if (ok)
                        SocialAndSharing.RateUs(null, GlobalConfig.Socials.rateUrl);
                    Back();
                });
            }
            else
            {
                game.OpenPopup<Popup_Confirm>().Setup(111126, true, true, ok =>
                {
                    if (ok)
                        Application.OpenURL(GlobalConfig.Socials.contactSurveyUrl);
                    Back();
                });
            }
        });
    }

    public void SetRate(int value)
    {
        Rateus.Current = value;
        sendButton.SetInteractable(true);
        closeButton.SetInteractable(false);
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        for (int i = 0; i < stars.Length; i++)
            stars[i].SetActive(i < Rateus.Current);
    }

    public override void Back()
    {
        base.Back();
        onCloseFunc?.Invoke();
    }
}


public static class Rateus
{
    public static int Current
    {
        get { return PlayerPrefs.GetInt("Rateing.Current", 0); }
        set { PlayerPrefs.SetInt("Rateing.Current", value); }
    }

    private static int Joy
    {
        get { return PlayerPrefs.GetInt("Rateing.Joy", 0); }
        set { PlayerPrefs.SetInt("Rateing.Joy", value); }
    }

    public static void AddJoy(int value, Action nextTask = null)
    {
        Joy += value;
        if (Joy >= 4 && Current < 2)
        {
            Joy *= -1;
            Game.Instance.OpenPopup<Popup_Rateus>().Setup(nextTask);
        }
        else if (nextTask != null)
            nextTask();
    }

    [Console("test", "rateus")]
    public static void Test()
    {
        AddJoy(4);
    }
}
