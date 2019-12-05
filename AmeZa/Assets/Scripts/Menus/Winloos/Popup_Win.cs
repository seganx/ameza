using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Win : GameState
{
    [SerializeField] private GameObject[] stars = null;
    [SerializeField] private LocalText desc = null;
    [SerializeField] private Button rewardButton = null;

    private System.Action nextTaskFunc = null;

    private void Awake()
    {
        foreach (var star in stars)
            star.SetActive(false);
    }

    // Use this for initialization
    private IEnumerator Start()
    {
        var rewards = PlayModel.GetReward();
        int rewardStars = PlayModel.GetRewardStars();

        // verify that player has been passed this level
        if (Profile.IsLevelPassed(PlayModel.level.season, PlayModel.level.index))
        {
            rewards.gems /= 2;
            rewards.bombs = rewards.hammers = rewards.missiles = 0;
        }

        Profile.SetLevelStars(PlayModel.level.season, PlayModel.level.index, rewardStars);

        int totalBalls = PlayModel.stats.totalBalls + PlayModel.level.startBallCount;
        desc.SetFormatedText(PlayModel.stats.totalTurn.ToString(), PlayModel.stats.totalBlocks.ToString(), totalBalls);

        rewardButton.gameObject.SetActive(rewards.exist);
        rewardButton.onClick.AddListener(() =>
        {
            rewardButton.gameObject.SetActive(false);
            Profile.EarnGems(rewards.gems);
            Profile.Bombs += rewards.bombs;
            Profile.Hammers += rewards.hammers;
            Profile.Missiles += rewards.missiles;
            Game.Instance.OpenPopup<Popup_Rewards>().Setup(0, rewards.gems, rewards.bombs, rewards.hammers, rewards.missiles, true);
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
    }

    public Popup_Win SetNextTask(System.Action nextTask)
    {
        nextTaskFunc = nextTask;
        return this;
    }

    public override void Back()
    {
        base.Back();
        if (nextTaskFunc != null)
            nextTaskFunc();
    }
}
