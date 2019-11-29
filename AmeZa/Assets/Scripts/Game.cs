using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Game : GameManager<Game>
{
    // Use this for initialization
    private IEnumerator Start()
    {
        FirstInitialization();
        yield return new WaitForSeconds(0.1f);

        PurchaseSystem.Initialize(GlobalConfig.Instance.cafeBazaarKey, GlobalConfig.Socials.storeUrl, (succeed, msg) => Debug.Log("Purchase system initialized!"));

        Loading.Show();
        Profile.Sync(false, succss =>
        {
            Loading.Hide();
            OpenState<State_Main>();
        });
    }

    private void FirstInitialization()
    {
        if (PlayerPrefs.GetInt(name + ".Inited", 0) > 0) return;
        PlayerPrefs.SetInt(name + ".Inited", 1);

        // init timers
        Online.Timer.SetTimer(GlobalConfig.Timers.heart.id, GlobalConfig.Timers.heart.duration);
    }

    public static void SpendGems(int value, System.Action onSuccess)
    {
        if (Profile.SpendGems(value))
        {
            onSuccess();
        }
        else Instance.OpenPopup<Popup_Shop>().SetOnClose(() =>
        {
            if (Profile.SpendGems(value))
                onSuccess();
        });
    }
}
