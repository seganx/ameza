using SeganX;
using UnityEngine;
using UnityEngine.UI;

public class State_Levels : GameState
{
    private enum SeasonState { Null, Progressing, CanClaimReward, Completed }

    [SerializeField] private LocalText title = null;
    [SerializeField] private Button chestButton = null;
    [SerializeField] private Animation chestAnimation = null;
    [SerializeField] private Image progressImage = null;
    [SerializeField] private GameObject progressHighlight = null;
    [SerializeField] private GameObject claimReward = null;
    [SerializeField] private Image themeImage = null;
    [SerializeField] private UiLevelItem levelItem = null;
    [SerializeField] private GameObject comingSoon = null;
    [SerializeField] private Button nextButton = null;
    [SerializeField] private Button prevButton = null;
    [SerializeField] private UiTutorial tutorial = null;

    private RectTransform content = null;
    private SeasonModel season = null;
    private float seasonProgress = 0;
    private SeasonState seasonState = SeasonState.Null;

    private void Awake()
    {
        content = levelItem.transform.parent.AsRectTransform();
        levelItem.gameObject.SetActive(false);
    }

    private void Start()
    {
        season = GlobalFactory.Seasons.Get(CurrentSeason);

        chestButton.onClick.AddListener(() =>
        {
            if (seasonState == SeasonState.CanClaimReward)
            {
                var reward = season.finalReward.GetResult();
                Profile.SetSeasonRewarded(season.id, 1);
                Profile.EarnGems(reward.gems);
                Profile.SetBombs(Profile.Bombs.value + reward.bombs);
                Profile.SetHammers(Profile.Hammers.value + reward.hammers);
                Profile.SetMissiles(Profile.Missiles.value + reward.missiles);
                Game.Instance.OpenPopup<Popup_Rewards>().Setup(0, reward.gems, reward.bombs, reward.hammers, reward.missiles, false, false, () => Rateus.AddJoy(4, () => nextButton.onClick.Invoke()));
                GlobalAnalytics.SourceGem(reward.gems, "season");
            }
            else tutorial.Display(0, false, 111041, null);
        });

        nextButton.onClick.AddListener(() =>
        {
            season = GlobalFactory.Seasons.Get(++CurrentSeason);
            UpdateVisual();
            DisplayLevels();
        });

        prevButton.onClick.AddListener(() =>
        {
            var prevSeason = GlobalFactory.Seasons.Get(CurrentSeason - 1);
            if (prevSeason != null)
            {
                CurrentSeason--;
                season = prevSeason;
                UpdateVisual();
                DisplayLevels();
            }
        });

        UpdateVisual();
        DisplayLevels();
        UIBackground.Show();
        UiShowHide.ShowAll(transform);
    }

    private void UpdateVisual()
    {
        var openedLevels = Profile.GetSeasonProgress(CurrentSeason).value;

        if (season == null)
        {
            seasonState = SeasonState.Null;
        }
        else
        {
            seasonProgress = openedLevels / (float)season.levelCount;
            if (openedLevels < season.levelCount)
                seasonState = SeasonState.Progressing;
            else if (Profile.GetSeasonRewarded(CurrentSeason).value < 1)
                seasonState = SeasonState.CanClaimReward;
            else
                seasonState = SeasonState.Completed;

            GlobalFactory.Theme.Select(season.id);
            themeImage.sprite = GlobalFactory.Theme.Selected.previewBackground;
        }

        title.SetFormatedText(CurrentSeason + 1);
        chestButton.gameObject.SetActive(seasonState == SeasonState.Progressing || seasonState == SeasonState.CanClaimReward);
        claimReward.SetActive(seasonState == SeasonState.CanClaimReward);
        comingSoon.SetActive(seasonState == SeasonState.Null);

        progressImage.fillAmount = 0.1f + 0.8f * seasonProgress;
        progressHighlight.SetActive(seasonProgress > 0.7f);

        if (seasonProgress > 0.8f)
        {
            chestAnimation.Play();
        }
        else
        {
            chestAnimation.Stop();
            chestAnimation.transform.AsRectTransform().localRotation = Quaternion.identity;
        }

        CheckTutorial();


        if (GlobalConfig.DebugMode)
        {
            nextButton.SetInteractable(season != null);
            prevButton.SetInteractable(CurrentSeason > 0);
        }
        else
        {
            nextButton.SetInteractable(season != null && Profile.GetSeasonRewarded(season.id).value > 0);
            prevButton.SetInteractable(CurrentSeason > 0);
        }
    }

    private void DisplayLevels()
    {
        content.RemoveChildrenBut(0);
        if (season == null) return;
        int enabledIndex = 0;
        for (int i = 0; i < season.levelCount; i++)
        {
            var canOpen = Profile.CanOpenLevel(season.id, i).value;
            levelItem.Clone<UiLevelItem>().Setup(season, i, canOpen).gameObject.SetActive(true);
            if (canOpen) enabledIndex = i;
        }

        if (enabledIndex > 15)
            DelayCall(0.1f, () => content.SetAnchordPositionY((enabledIndex - 12) * 110 / 4));
    }

    private void CheckTutorial()
    {
        if (seasonState == SeasonState.Progressing)
        {
            bool displayed = false;

            if (CurrentSeason == 0)
                displayed = tutorial.Display(0.5f, true, 111041, () => tutorial.Display(0, true, 111054, () => tutorial.Display(0, true, 111055, null)));
            //else if (CurrentSeason == 1)
            //    displayed = tutorial.Display(1, true, 111039, () => tutorial.Display(0, true, 111040, null));
            //else if (CurrentSeason == 2)
            //    displayed = tutorial.Display(1, true, 111050, () => tutorial.Display(0, true, 111051, () => tutorial.Display(0, true, 111052, null)));
            else if (CurrentSeason == 3)
                displayed = tutorial.Display(0.5f, true, 111042, () => tutorial.Display(0, true, 111043, null));
            //else
            //    displayed = tutorial.Display(1, true, 111056, () => tutorial.Display(0, true, 111057, null));

            if (displayed == false)
                tutorial.DisplayJoke(1);
        }
        else if (seasonState == SeasonState.CanClaimReward)
            tutorial.Display(0.5f, false, 111035, () => tutorial.Display(0, true, 111036, null));
    }

    public override void Back()
    {
        if (tutorial.Hide()) return;
        base.Back();
    }

    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    public static int CurrentSeason
    {
        get { return PlayerPrefs.GetInt("State_Levels.CurrentSeason", 0); }
        set { PlayerPrefs.SetInt("State_Levels.CurrentSeason", value); }
    }
}
