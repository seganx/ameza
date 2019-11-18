using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeganX;

public class UIBackground : MonoBehaviour
{
    [SerializeField] private UiAvatar avatar = null;
    [SerializeField] private LocalText nickNameLabel = null;
    [SerializeField] private LocalText gems = null;
    [SerializeField] private LocalText heartsTime = null;
    [SerializeField] private LocalText heartsCount = null;
    [SerializeField] Animation backgroundAnimator = null;
    [SerializeField] Button heartsButton = null;
    [SerializeField] Button gemsButton = null;
    [SerializeField] Button profileButton = null;
    [SerializeField] Button settingsButton = null;

    // Use this for initialization
    private IEnumerator Start()
    {
        instance = this;
        transform.SetAsFirstSibling();

        foreach (AnimationState state in backgroundAnimator)
        {
            state.time = Random.Range(0, 120);
            break;
        }

        heartsButton.onClick.AddListener(() => { Game.Instance.OpenPopup<Popup_BuyHearts>(); });
        gemsButton.onClick.AddListener(() => { Game.Instance.OpenPopup<Popup_Shop>(); });
        profileButton.onClick.AddListener(() => { Game.Instance.OpenPopup<Popup_Profile>(); });
        settingsButton.onClick.AddListener(() => { Game.Instance.OpenPopup<Popup_Settings>(); });


        UiShowHide.ShowAll(transform);

        var wait = new WaitForSeconds(0.5f);
        while (true)
        {
            avatar.Setup(Profile.Avatar);
            nickNameLabel.SetText(Profile.Nickname);
            heartsCount.SetText(Profile.Hearts.ToString());
            gems.SetText(Profile.Gems.ToString("#,0"));
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
