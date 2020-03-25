using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeganX;

public class Popup_LeagueLose : GameState
{
    [SerializeField] private LocalText scoreLabel = null;
    [SerializeField] private LocalText descLabel = null;
    [SerializeField] private Button inviteButton = null;
    [SerializeField] private Button replayButton = null;
    [SerializeField] private LocalText baloon = null;

    private static int BaloonIndex
    {
        get { return PlayerPrefs.GetInt("Popup_LeagueLose.BaloonIndex", 0); }
        set { PlayerPrefs.SetInt("Popup_LeagueLose.BaloonIndex", value); }
    }

    private IEnumerator Start()
    {
        scoreLabel.SetText("0");

        int totalBalls = PlayModel.stats.totalBalls + PlayModel.level.startBallCount;
        descLabel.SetFormatedText(PlayModel.stats.totalTurn.ToString(), PlayModel.stats.totalBlocks.ToString(), totalBalls);

        inviteButton.onClick.AddListener(() =>
        {
            var str = GlobalConfig.Socials.storeUrl + string.Format(LocalizationService.Get(111063), Profile.Username);
            SocialAndSharing.ShareText(str);
        });

        replayButton.onClick.AddListener(() =>
        {
            if (LeagueLogics.SetPlayerModel())
            {
                base.Back();
                UIBackground.Hide();
                game.ClosePopup(true);
                game.OpenState<State_Playing>();
            }
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

        float t = 0;
        float curscore = 0, maxscore = PlayModel.GetLeagueScore();
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
        Game.Instance.OpenState<State_Main>(true);
    }
}
