using SeganX;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIBackground : MonoBehaviour
{
    [SerializeField] private UiAvatar avatar = null;
    [SerializeField] private LocalText nickNameLabel = null;
    [SerializeField] private LocalText gems = null;
    [SerializeField] private LocalText energyLabel = null;
    [SerializeField] private Animation backgroundAnimator = null;
    [SerializeField] private Button heartsButton = null;
    [SerializeField] private Button gemsButton = null;
    [SerializeField] private Button profileButton = null;
    [SerializeField] private Button updateButton = null;
    [SerializeField] private Button settingsButton = null;

    // Use this for initialization
    private IEnumerator Start()
    {
        AudioManager.SetMusicVolumFactor(0.7f);

        instance = this;
        transform.SetAsFirstSibling();

        foreach (AnimationState state in backgroundAnimator)
        {
            state.time = Random.Range(0, 120);
            break;
        }

        heartsButton.onClick.AddListener(() => { if (Profile.Energy.value < GlobalConfig.ProfilePreset.energy) Game.Instance.OpenPopup<Popup_BuyHearts>(); });
        gemsButton.onClick.AddListener(() => { Game.Instance.OpenPopup<Popup_Shop>(); });
        profileButton.onClick.AddListener(() => { Game.Instance.OpenPopup<Popup_Profile>(); });
        updateButton.onClick.AddListener(() => { Application.OpenURL(GlobalConfig.Market.storeUrl); });
        settingsButton.onClick.AddListener(() => { Game.Instance.OpenPopup<Popup_Settings>(); });

        UiShowHide.ShowAll(transform);

        var wait = new WaitForSeconds(0.5f);
        while (true)
        {
            avatar.Setup(Profile.Avatar.Current);
            updateButton.gameObject.SetActive(GlobalConfig.Update.mode != GlobalConfig.Data.Update.Mode.Null);
            nickNameLabel.SetText(Profile.Nickname);
            gems.SetText(Profile.Gems.value.ToString("#,0"));

            if (Profile.Energy.value < 1)
            {
                int seconds = Online.Timer.GetRemainSeconds(Timers.Energy, GlobalConfig.Energy.interval);
                energyLabel.SetText((seconds / 60).ToString("00") + ":" + (seconds % 60).ToString("00"));
            }
            else
            {
                energyLabel.SetText($"{Profile.Energy.value}/{GlobalConfig.ProfilePreset.energy}");
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
            instance = Resources.Load<UIBackground>("Menus/UIBackground").Clone<UIBackground>(Game.Instance.Canvas.transform);
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
