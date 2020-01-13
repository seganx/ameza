using SeganX;
using System.Collections;
using System.Collections.Generic;
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

    private int rgems = 0;
    private int rbombs = 0;
    private int rhammers = 0;
    private int rmissiles = 0;
    private bool doOpenChest = false;
    private System.Action onCloseFunc = null;

    public Popup_Rewards Setup(int ballId, int gems, int bombs, int hammers, int missiles, bool openChest = false, System.Action onClose = null)
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
        if (hammers > 0) rhammers= hammers;
        else hammersLabel.gameObject.SetActive(false);
        if (missiles > 0) rmissiles = missiles;
        else missilesLabel.gameObject.SetActive(false);

        boosterContent.SetActive(bombs > 0 || hammers > 0 || missiles > 0);

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

        window.GoToAnchordPosition(0, 0, 0, 10);
        yield return new WaitForSeconds(0.5f);

        //if (rgems > 0)
        {
            float t = 0;
            var wait = new WaitForEndOfFrame();
            while (t < 1)
            {
                t += Time.deltaTime;
                gemsLabel.SetText("+" + Mathf.RoundToInt(Mathf.Lerp(0, rgems, t)));
                bombsLabel.SetText("+" + Mathf.RoundToInt(Mathf.Lerp(0, rbombs, t)));
                hammersLabel.SetText("+" + Mathf.RoundToInt(Mathf.Lerp(0, rhammers, t)));
                missilesLabel.SetText("+" + Mathf.RoundToInt(Mathf.Lerp(0, rmissiles, t)));
                yield return wait;
            }

        }
    }

    public override void Back()
    {
        if (chest.IsOpened)
        {
            base.Back();
            if (onCloseFunc != null) onCloseFunc();
        }
        else chest.Open();
    }
}
