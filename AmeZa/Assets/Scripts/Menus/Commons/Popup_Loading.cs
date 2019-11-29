using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Loading : GameState
{
    [SerializeField] private GameObject defaultObject = null;
    [SerializeField] private GameObject updateObject = null;
    [SerializeField] private Button updateButton = null;

    private void Start()
    {
        updateButton.onClick.AddListener(() =>
        {
            Application.OpenURL(GlobalConfig.Socials.storeUrl);
            Application.Quit();
        });

        if (instance != null)
            UiShowHide.ShowAll(transform);
    }

    private void Update()
    {
        updateObject.SetActive(GlobalConfig.ForceUpdate.whole);
        defaultObject.SetActive(GlobalConfig.ForceUpdate.whole == false);
        transform.SetAsLastSibling();
    }

    public override void Back()
    {
        // ;)
    }

    ///////////////////////////////////////////////////////////////////////////////////
    //  STATIC MEMBERS
    ///////////////////////////////////////////////////////////////////////////////////
    private static Popup_Loading instance = null;
    private static int count = 0;

    public static void Show()
    {
        count++;
        if (instance != null) return;
        instance = gameManager.OpenPopup<Popup_Loading>();
    }

    public static void Hide()
    {
        if (instance == null) return;
        if (GlobalConfig.ForceUpdate.whole) return;
        count--;
        if (count > 0) return;
        gameManager.Back(instance);
        instance = null;
    }
}


///////////////////////////////////////////////////////////////////////////////////
//  Helper class
///////////////////////////////////////////////////////////////////////////////////
public static class Loading
{
    public static void Show() { Popup_Loading.Show(); }
    public static void Hide() { Popup_Loading.Hide(); }
}