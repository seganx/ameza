using SeganX;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Lose : GameState
{
    [SerializeField] private LocalText scoreLabel = null;
    [SerializeField] private LocalText descLabel = null;
    [SerializeField] private Button inviteButton = null;
    [SerializeField] private Button replayButton = null;
    [SerializeField] private LocalText baloon = null;

    private System.Action<System.Action> onReplayFunc = null;
    private System.Action onCloseFunc = null;

    private static int BaloonIndex
    {
        get { return PlayerPrefs.GetInt("Popup_Lose.BaloonIndex", 0); }
        set { PlayerPrefs.SetInt("Popup_Lose.BaloonIndex", value); }
    }

    public Popup_Lose Setup(System.Action<System.Action> onReplay, System.Action onClose)
    {
        onReplayFunc = onReplay;
        onCloseFunc = onClose;
        return this;
    }

    private IEnumerator Start()
    {
        scoreLabel.SetText("0");

        int totalBalls = PlayModel.result.totalBalls + PlayModel.level.startBallCount;
        descLabel.SetFormatedText(PlayModel.result.totalTurn.ToString(), PlayModel.result.totalBlocks.ToString(), totalBalls);

        inviteButton.onClick.AddListener(() =>
        {
            var str = string.Format(GlobalConfig.Socials.invitationText, Profile.Nickname, GlobalConfig.Market.storeUrl);
            SocialAndSharing.ShareText(str);
        });

        replayButton.onClick.AddListener(() =>
        {
            onReplayFunc(() =>
            {
                base.Back();
                UIBackground.Hide();
                Game.Instance.ClosePopup(true);
                Game.Instance.OpenState<State_Playing>();
            });
        });

        baloon.gameObject.SetActive(false);
        UiShowHide.ShowAll(transform);
        yield return new WaitForSeconds(0.5f);

        // Incentive  text
        {
            var index = BaloonIndex++ % 25;
            var incentiveStr = LocalizationService.Get(111090 + index);
            baloon.SetText(incentiveStr);
            baloon.gameObject.SetActive(true);
        }

        if (PlayModel.level.totoalLevelNumber > 5)
            Plankton.Ad.ShowInterstitial(GlobalConfig.Advertise.levels);

        float t = 0;
        float curscore = 0, maxscore = PlayModel.GetScore();
        var wait = new WaitForEndOfFrame();
        while (t < 1)
        {
            t += Time.deltaTime * 0.75f;
            curscore = Mathf.Lerp(0, maxscore, t);
            scoreLabel.SetText(Mathf.RoundToInt(curscore).ToString());
            yield return wait;
        }
    }

    public override void Back()
    {
        base.Back();
        onCloseFunc?.Invoke();
    }
}
