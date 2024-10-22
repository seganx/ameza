using SeganX;
using System;
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

        resetButton.onClick.AddListener(() => Game.Instance.OpenPopup<Popup_Confirm>().Setup(111061, true, true, yes =>
        {
            if (yes)
                Profile.Reset();
        }));

        surveyButton.onClick.AddListener(() => Application.OpenURL(GlobalConfig.Socials.contactSurveyUrl));

        supportButton.onClick.AddListener(() => SocialAndSharing.SendEmail(
            GlobalConfig.Socials.contactEmailUrl,
            "Support - " + Application.identifier + " - " + Application.version,
            "OS:" + SystemInfo.operatingSystem + "|<br>Model:" + SystemInfo.deviceModel + "|<br>Username:" + Profile.Nickname + "|<br>DeviceId:" + Core.DeviceId + "|<br>Group:" + GlobalConfig.Group + "|_____________________<br><br><br><br>"));

        purchasedButton.onClick.AddListener(() =>
        {
            Loading.Show();
            Plankton.Billing.GetPurchases((success, list) =>
            {
                Loading.Hide();
                if (success)
                {
                    if (list.Count > 0)
                    {
                        var item = list[0];
                        if (item.status == Plankton.Billing.PurchaseStatus.Purchased)
                        {
                            ShopLogic.Purchased(item.sku, item.token, () => { });
                        }
                    }
                    else Game.Instance.OpenPopup<Popup_Confirm>().Setup(111062, true, false, null);
                }
            });
        });

        UiShowHide.ShowAll(transform);
    }
}
