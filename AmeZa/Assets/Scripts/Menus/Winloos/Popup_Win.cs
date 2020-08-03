using SeganX;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Win : GameState
{
    [SerializeField] private GameObject[] stars = null;
    [SerializeField] private LocalText desc = null;
    [SerializeField] private Button continueButton = null;
    [SerializeField] private Button nextLevelButton = null;
    [SerializeField] private Button goHomeButton = null;
    [SerializeField] private LocalText baloon = null;

    private System.Action<bool> nextTaskFunc = null;

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

        if (rewards.exist)
        {
            continueButton.gameObject.SetActive(true);
            nextLevelButton.gameObject.SetActive(false);
            goHomeButton.gameObject.SetActive(false);
        }
        else
        {
            continueButton.gameObject.SetActive(false);
            nextLevelButton.gameObject.SetActive(true);
            goHomeButton.gameObject.SetActive(true);
        }

        nextLevelButton.onClick.AddListener(() =>
        {
            base.Back();
            nextTaskFunc?.Invoke(true);
        });

        goHomeButton.onClick.AddListener(() =>
        {
            base.Back();
            nextTaskFunc?.Invoke(false);
        });

        continueButton.onClick.AddListener(() =>
        {
            continueButton.gameObject.SetActive(false);

            Profile.EarnGems(rewards.gems);
            Profile.Bombs += rewards.bombs;
            Profile.Hammers += rewards.hammers;
            Profile.Missiles += rewards.missiles;
            Game.Instance.OpenPopup<Popup_Rewards>().Setup(0, rewards.gems, rewards.bombs, rewards.hammers, rewards.missiles, true, () =>
            {
                continueButton.gameObject.SetActive(false);
                nextLevelButton.gameObject.SetActive(true);
                goHomeButton.gameObject.SetActive(true);
            });
            GlobalAnalytics.SourceGem(rewards.gems, "level");
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

    public Popup_Win SetNextTask(System.Action<bool> nextTask)
    {
        nextTaskFunc = nextTask;
        return this;
    }

    public override void Back()
    {
        if (continueButton.gameObject.activeSelf)
            continueButton.onClick.Invoke();
        else
            nextLevelButton.onClick.Invoke();
    }

    public override float PreClose()
    {
        return base.PreClose() * 2;
    }
}
