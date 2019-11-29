using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_Levels : GameState
{
    [SerializeField] private LocalText title = null;
    [SerializeField] private GameObject chestObject = null;
    [SerializeField] private Button claimRewardButton = null;
    [SerializeField] private UiLevelItem levelItem = null;
    [SerializeField] private GameObject comingSoon = null;
    [SerializeField] private Button nextButton = null;
    [SerializeField] private Button prevButton = null;

    private RectTransform content = null;
    private SeasonConfig season = null;

    private void Awake()
    {
        content = levelItem.transform.parent.AsRectTransform();
        levelItem.gameObject.SetActive(false);
    }

    private void Start()
    {
        season = GlobalFactory.Seasons.Get(CurrentSeason);

        claimRewardButton.onClick.AddListener(() =>
        {
            Profile.SetSeasonRewarded(season.Id, 1);
            Profile.EarnGems(season.rewardGems);
            Game.Instance.OpenPopup<Popup_Rewards>().Setup(0, season.rewardGems, 0, 0, 0, false, () => nextButton.onClick.Invoke());
            UpdateVisual();
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
        title.SetFormatedText(CurrentSeason + 1);
        var rewarded = Profile.GetSeasonRewarded(CurrentSeason);
        chestObject.gameObject.SetActive(rewarded < 1);
        var canClaimReward = season == null ? false : Profile.GetLevelStars(CurrentSeason, season.levelCount - 1) > 0;
        claimRewardButton.gameObject.SetActive(canClaimReward);
        comingSoon.SetActive(season == null);
        nextButton.SetInteractable(season != null);
        prevButton.SetInteractable(CurrentSeason > 0);
    }

    private void DisplayLevels()
    {
        content.RemoveChildrenBut(0);
        if (season == null) return;
        for (int i = 0; i < season.levelCount; i++)
            levelItem.Clone<UiLevelItem>().Setup(season, i).gameObject.SetActive(true);
    }

    ////////////////////////////////////////////////////////////
    /// STATIC MEMBERS
    ////////////////////////////////////////////////////////////
    private static int CurrentSeason
    {
        get { return PlayerPrefs.GetInt("State_Levels.CurrentSeason", 0); }
        set { PlayerPrefs.SetInt("State_Levels.CurrentSeason", value); }
    }
}
