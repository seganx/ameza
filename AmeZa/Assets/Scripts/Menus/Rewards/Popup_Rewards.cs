using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Rewards : GameState
{
    [SerializeField] private UiAvatar avatar = null;
    [SerializeField] private GameObject gemsObject = null;
    [SerializeField] private LocalText gemsLabel = null;
    [SerializeField] private UiChest chest = null;
    [SerializeField] private RectTransform window = null;

    private int rgems = 0;
    private bool doOpenChest = false;
    private System.Action onCloseFunc = null;

    public Popup_Rewards Setup(int gems, int ballId, bool openChest = false, System.Action onClose = null)
    {
        onCloseFunc = onClose;
        doOpenChest = openChest;

        if (gems > 0)
            rgems = gems;
        else
            gemsObject.SetActive(false);

        if (ballId > 0)
        {
            var tmp = new ProfileData.AvatarData() { angle = Profile.Avatar.angle };
            tmp.ballId = ballId;
            avatar.Setup(tmp);
        }
        else avatar.gameObject.SetActive(false);

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

        if (rgems > 0)
        {
            float t = 0;
            var wait = new WaitForEndOfFrame();
            while (t < 1)
            {
                t += Time.deltaTime;
                gemsLabel.SetText(Mathf.RoundToInt(Mathf.Lerp(0, rgems, t)).ToString());
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
