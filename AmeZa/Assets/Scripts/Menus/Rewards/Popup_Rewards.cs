using SeganX;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Rewards : GameState
{
    [SerializeField] private UiAvatar avatar = null;
    [SerializeField] private LocalText gemsLabel = null;
    [SerializeField] private LocalText bombsLabel = null;
    [SerializeField] private LocalText hammersLabel = null;
    [SerializeField] private LocalText missilesLabel = null;
    [SerializeField] private GameObject boosterContent = null;
    [SerializeField] private UiChest chest = null;
    [SerializeField] private RectTransform window = null;
    [SerializeField] private Button winClaimButton = null;

    private float rtime = 0;
    private int rgems = 0;
    private int rbombs = 0;
    private int rhammers = 0;
    private int rmissiles = 0;
    private bool doOpenChest = false;
    private System.Action onCloseFunc = null;

    public Popup_Rewards Setup(int ballId, int gems, int bombs, int hammers, int missiles, bool openChest = false, bool winClaim = false, System.Action onClose = null)
    {
        onCloseFunc = onClose;
        doOpenChest = openChest;

        if (ballId > 0)
        {
            var tmp = new ProfileData.AvatarData() { angle = Profile.Avatar.Angle };
            tmp.ballId = ballId;
            avatar.Setup(tmp);
        }
        else avatar.gameObject.SetActive(false);

        if (gems > 0) rgems = gems;
        else gemsLabel.gameObject.SetActive(false);
        if (bombs > 0) rbombs = bombs;
        else bombsLabel.gameObject.SetActive(false);
        if (hammers > 0) rhammers = hammers;
        else hammersLabel.gameObject.SetActive(false);
        if (missiles > 0) rmissiles = missiles;
        else missilesLabel.gameObject.SetActive(false);

        boosterContent.SetActive(bombs > 0 || hammers > 0 || missiles > 0);

        winClaimButton.gameObject.SetActive(winClaim && Online.Timer.GetRemainSeconds(GlobalConfig.Advertise.winClaim.timerId, GlobalConfig.Advertise.winClaim.interval) < 1);
        winClaimButton.onClick.AddListener(() =>
        {
            Game.Instance.OpenPopup<Popup_VideoAd>().Setup(GlobalConfig.Advertise.winClaim, "winclaim", success =>
            {
                if (success == false) return;
                rtime = 0;
                winClaimButton.gameObject.SetActive(false);
                Online.Timer.Set(GlobalConfig.Advertise.winClaim.timerId, GlobalConfig.Advertise.winClaim.interval);
                rgems += GlobalConfig.Advertise.winClaim.rewardGems;
                Profile.EarnGems(GlobalConfig.Advertise.winClaim.rewardGems);
                GlobalAnalytics.SourceGem(GlobalConfig.Advertise.winClaim.rewardGems, "winclaim");
            });
        });

        return this;
    }

    private IEnumerator Start()
    {
        UiShowHide.ShowAll(transform);

        if (doOpenChest)
        {
            //yield return new WaitForSeconds(0.5f);
            chest.Open();
        }

        yield return new WaitUntil(() => chest.IsOpened);
        yield return new WaitForSeconds(1);

        while (window.anchoredPosition.sqrMagnitude > 5)
        {
            window.anchoredPosition = Vector3.Lerp(window.anchoredPosition, Vector3.zero, Time.deltaTime * 10);
            yield return new WaitForEndOfFrame();
        }

        //if (rgems > 0)
        {
            var wait = new WaitForEndOfFrame();
            while (true)
            {
                if (rtime < 1)
                {
                    rtime += Time.deltaTime;
                    gemsLabel.SetText("+" + Mathf.RoundToInt(Mathf.Lerp(0, rgems, rtime)));
                    bombsLabel.SetText("+" + Mathf.RoundToInt(Mathf.Lerp(0, rbombs, rtime)));
                    hammersLabel.SetText("+" + Mathf.RoundToInt(Mathf.Lerp(0, rhammers, rtime)));
                    missilesLabel.SetText("+" + Mathf.RoundToInt(Mathf.Lerp(0, rmissiles, rtime)));
                }
                yield return wait;
            }

        }
    }

    public override void Back()
    {
        if (chest.IsOpened)
        {
            base.Back();
            onCloseFunc?.Invoke();
        }
        else chest.Open();
    }
}
