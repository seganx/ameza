using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBackground : MonoBehaviour
{
    [SerializeField] private UiAvatar avatar = null;
    [SerializeField] private LocalText nickNameLabel = null;
    [SerializeField] private LocalText gems = null;
    [SerializeField] private LocalText heartsCount = null;
    [SerializeField] private Text heartsTime = null;
    [SerializeField] private GameObject heartsFull = null;
    [SerializeField] private Animation backgroundAnimator = null;
    [SerializeField] private Button heartsButton = null;
    [SerializeField] private Button gemsButton = null;
    [SerializeField] private Button profileButton = null;
    [SerializeField] private Button updateButton = null;
    [SerializeField] private Button settingsButton = null;

    // Use this for initialization
    private IEnumerator Start()
    {
        AudioManager.PlayMusic(0, 0.5f, 2, 2);

        instance = this;
        transform.SetAsFirstSibling();

        foreach (AnimationState state in backgroundAnimator)
        {
            state.time = Random.Range(0, 120);
            break;
        }

        heartsButton.onClick.AddListener(() => { if (Profile.Hearts < GlobalConfig.ProfilePreset.heats) Game.Instance.OpenPopup<Popup_BuyHearts>(); });
        gemsButton.onClick.AddListener(() => { Game.Instance.OpenPopup<Popup_Shop>(); });
        profileButton.onClick.AddListener(() => { Game.Instance.OpenPopup<Popup_Profile>(); });
        updateButton.onClick.AddListener(() => { Application.OpenURL(GlobalConfig.Socials.storeUrl); });
        settingsButton.onClick.AddListener(() => { Game.Instance.OpenPopup<Popup_Settings>(); });

        UiShowHide.ShowAll(transform);

        var wait = new WaitForSeconds(0.5f);
        while (true)
        {
            avatar.Setup(Profile.Avatar.Current);
            updateButton.gameObject.SetActive(GlobalConfig.Update != GlobalConfig.Data.Update.Null);
            nickNameLabel.SetText(Profile.Nickname);
            gems.SetText(Profile.Gems.ToString("#,0"));
            heartsCount.SetText(Profile.Hearts.ToString());

            if (Profile.Hearts < GlobalConfig.ProfilePreset.heats)
            {
                int seconds = Online.Timer.GetRemainSeconds(GlobalConfig.Timers.heart.id, GlobalConfig.Timers.heart.duration);
                heartsTime.text = (seconds / 60).ToString("00") + ":" + (seconds % 60).ToString("00");
                heartsTime.gameObject.SetActive(true);
                heartsFull.gameObject.SetActive(false);
            }
            else
            {
                heartsTime.gameObject.SetActive(false);
                heartsFull.gameObject.SetActive(true);
            }
            yield return wait;
        }
    }

    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    private static UIBackground instance = null;

    public static void Show()
    {
        if (instance == null)
            instance = Resources.Load<UIBackground>("Menus/UIBackground").Clone<UIBackground>(Game.Instance.canvas.transform);
    }

    public static void Hide()
    {
        if (instance != null)
        {
            UiShowHide.HideAll(instance.transform);
            Destroy(instance.gameObject, 1);
            instance = null;
        }
    }
}
