using SeganX;
using UnityEngine;
using UnityEngine.UI;

public class Popup_VideoAd : GameState
{
    [SerializeField] private Button cancelButton = null;

    public Popup_VideoAd Setup(FunAd.Segment segment, string placement, System.Action<bool> callback)
    {
        cancelButton.onClick.AddListener(() =>
        {
            base.Back();
#if UNITY_EDITOR
            callback(true);
#else
            callback(false);
#endif
        });

        GlobalAnalytics.Ad.Rewarded.Request(placement);
        FunAd.Rewarded.Request(segment, success =>
        {
            if (this != null && success)
            {
                base.Back();
                GlobalAnalytics.Ad.Rewarded.Show(placement);
                FunAd.Rewarded.Show(segment, callback);
            }
            else GlobalAnalytics.Ad.Rewarded.RequestFailed(placement);
        });

        return this;
    }

    private void Start()
    {
        UiShowHide.ShowAll(transform);
    }

    public override float PreClose()
    {
        return 0;
    }

    public override void Back()
    {
        cancelButton.onClick.Invoke();
    }
}
