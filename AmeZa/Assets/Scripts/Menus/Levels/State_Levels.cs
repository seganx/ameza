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
    [SerializeField] private Image themeImage = null;
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
            var reward = season.finalReward.GetResult();
            Profile.SetSeasonRewarded(season.Id, 1);
            Profile.EarnGems(reward.gems);
            Profile.Bombs += reward.bombs;
            Profile.Hammers += reward.hammers;
            Profile.Missiles += reward.missiles;
            Game.Instance.OpenPopup<Popup_Rewards>().Setup(0, reward.gems, reward.bombs, reward.hammers, reward.missiles, false, () =>
            {
                Rateus.Joy += 4;
                nextButton.onClick.Invoke();
            });
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
        if (season != null) themeImage.sprite = GlobalFactory.Theme.GetBackground(season.theme);

#if UNITY_EDITOR
        nextButton.SetInteractable(season != null);
        prevButton.SetInteractable(CurrentSeason > 0);
#else
        nextButton.SetInteractable(season != null && Profile.GetSeasonRewarded(season.Id) > 0);
        prevButton.SetInteractable(CurrentSeason > 0);
#endif
    }

    private void DisplayLevels()
    {
        content.RemoveChildrenBut(0);
        if (season == null) return;
        int enabledIndex = 0;
        for (int i = 0; i < season.levelCount; i++)
        {
            var canOpen = Profile.CanOpenLevel(season.Id, i);
            levelItem.Clone<UiLevelItem>().Setup(season, i, canOpen).gameObject.SetActive(true);
            if (canOpen) enabledIndex = i;
        }

        if (enabledIndex > 15)
            DelayCall(0.1f, () => content.SetAnchordPositionY((enabledIndex - 12) * 110 / 4));
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
