using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Settings : GameState
{
    [SerializeField] private Toggle musicToggle = null;
    [SerializeField] private Toggle soundToggle = null;
    [SerializeField] private Button surveyButton = null;
    [SerializeField] private Button supportButton = null;

    private void Start()
    {
        musicToggle.isOn = AudioManager.MusicVolume < 1;
        soundToggle.isOn = AudioManager.SoundVolume < 1;
        musicToggle.onValueChanged.AddListener((on) => AudioManager.MusicVolume = on ? 0 : 100);
        soundToggle.onValueChanged.AddListener((on) => AudioManager.SoundVolume = on ? 0 : 100);
        surveyButton.onClick.AddListener(() => Application.OpenURL(GlobalConfig.Socials.contactSurveyUrl));
        supportButton.onClick.AddListener(() => SocialAndSharing.SendEmail(GlobalConfig.Socials.contactEmailUrl, "Support", "\n\n\n\n\n\n" + SystemInfo.operatingSystem + "\n" + SystemInfo.deviceModel + "\n" + Profile.Username + "\n" + Core.DeviceId));

        UiShowHide.ShowAll(transform);
    }
}
