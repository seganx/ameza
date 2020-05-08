using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Win : GameState
{
    [SerializeField] private GameObject[] stars = null;
    [SerializeField] private LocalText desc = null;
    [SerializeField] private Button continueButton = null;
    [SerializeField] private LocalText baloon = null;

    private System.Action nextTaskFunc = null;

    private static int BaloonIndex
    {
        get { return PlayerPrefs.GetInt("Popup_Win.BaloonIndex", 0); }
        set { PlayerPrefs.SetInt("Popup_Win.BaloonIndex", value); }
    }

    private void Awake()
    {
        foreach (var star in stars)
            star.SetActive(false);
        baloon.gameObject.SetActive(false);
    }

    // Use this for initialization
    private IEnumerator Start()
    {
        var rewards = PlayModel.GetReward();
        int rewardStars = PlayModel.GetRewardStars();

        // verify that player has been passed this level
        if (Profile.IsLevelPassed(PlayModel.level.season, PlayModel.level.index))
            rewards.gems = rewards.bombs = rewards.hammers = rewards.missiles = 0;

        Profile.SetLevelStars(PlayModel.level.season, PlayModel.level.index, rewardStars);

        int totalBalls = PlayModel.result.totalBalls + PlayModel.level.startBallCount;
        desc.SetFormatedText(PlayModel.result.totalTurn.ToString(), PlayModel.result.totalBlocks.ToString(), totalBalls);

        continueButton.onClick.AddListener(() =>
        {
            continueButton.gameObject.SetActive(false);
            if (rewards.exist)
            {
                Profile.EarnGems(rewards.gems);
                Profile.Bombs += rewards.bombs;
                Profile.Hammers += rewards.hammers;
                Profile.Missiles += rewards.missiles;
                Game.Instance.OpenPopup<Popup_Rewards>().Setup(0, rewards.gems, rewards.bombs, rewards.hammers, rewards.missiles, true, () =>
                {
                    base.Back();
                    if (nextTaskFunc != null)
                        nextTaskFunc();
                });
                GlobalAnalytics.Source(rewards.gems, "level");
            }
            else
            {
                base.Back();
                if (nextTaskFunc != null)
                    nextTaskFunc();
            }
        });

        UiShowHide.ShowAll(transform);
        var wait = new WaitForSeconds(0.3f);
        yield return wait;
        yield return wait;
        yield return wait;
        stars[0].SetActive(rewardStars > 0);
        yield return wait;
        stars[1].SetActive(rewardStars > 1);
        yield return wait;
        stars[2].SetActive(rewardStars > 2);
        yield return wait;

        // Incentive  text
        {
            var index = BaloonIndex++ % 25;
            var incentiveStr = LocalizationService.Get(111090 + index);
            if (BaloonIndex < 25 || Random.Range(0, 100) < rewardStars * 25)
            {
                baloon.SetText(incentiveStr);
                baloon.gameObject.SetActive(true);
            }
        }
    }

    public Popup_Win SetNextTask(System.Action nextTask)
    {
        nextTaskFunc = nextTask;
        return this;
    }

    public override void Back()
    {
        continueButton.onClick.Invoke();
    }
}
