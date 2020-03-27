using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Settings : GameState
{
    [SerializeField] private Toggle musicToggle = null;
    [SerializeField] private Toggle soundToggle = null;
    [SerializeField] private Button resetButton = null;
    [SerializeField] private Button surveyButton = null;
    [SerializeField] private Button supportButton = null;
    [SerializeField] private Button purchasedButton = null;

    private void Start()
    {
        musicToggle.isOn = AudioManager.MusicVolume < 1;
        soundToggle.isOn = AudioManager.SoundVolume < 1;
        musicToggle.onValueChanged.AddListener((on) => AudioManager.MusicVolume = on ? 0 : 100);
        soundToggle.onValueChanged.AddListener((on) => AudioManager.SoundVolume = on ? 0 : 100);

        resetButton.onClick.AddListener(() => game.OpenPopup<Popup_Confirm>().Setup(111061, true, true, yes =>
        {
            if (yes)
                Profile.Reset();
        }));

        surveyButton.onClick.AddListener(() => Application.OpenURL(GlobalConfig.Socials.contactSurveyUrl));

        supportButton.onClick.AddListener(() => SocialAndSharing.SendEmail(
            GlobalConfig.Socials.contactEmailUrl,
            "Support - " + Application.identifier + " - " + Application.version,
            "OS:" + SystemInfo.operatingSystem + "|<br>Model:" + SystemInfo.deviceModel + "|<br>Username:" + Profile.Username + "|<br>DeviceId:" + Core.DeviceId + "|<br>Group:" + GlobalAnalytics.Group + "|_____________________<br><br><br><br>"));

        purchasedButton.onClick.AddListener(() =>
        {
            Loading.Show();
            PurchaseSystem.QueryPurchases(PurchaseProvider.Bazaar, (success, json) =>
            {
                Loading.Hide();
                if (success)
                {
                    var data = JsonUtility.FromJson<PurchasedData>(json);
                    CheckPurchasedList(data, 0);
                }
            });
        });

        UiShowHide.ShowAll(transform);
    }

    private void CheckPurchasedList(PurchasedData data, int index)
    {
        if (data.list.Count > index)
            UiShopItem.Purchased(data.list[index].sku, data.list[index].token, () => CheckPurchasedList(data, index + 1));
        else
            game.OpenPopup<Popup_Confirm>().Setup(111062, true, false, null);
    }
}
