﻿using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_LevelInfo : GameState
{
    [SerializeField] private LocalText title = null;
    [SerializeField] private Image[] ballImages = null;
    [SerializeField] private LocalText ballsLabel = null;
    [SerializeField] private GameObject targetItemsDesc = null;
    [SerializeField] private GameObject allitemsDesc = null;
    [SerializeField] private Text targetItem0Label = null;
    [SerializeField] private Text targetItem1Label = null;
    [SerializeField] private Text targetBallsLabel = null;
    [SerializeField] private Text targetBlocksLabel = null;
    [SerializeField] private Button startButton = null;

    public Popup_LevelInfo Setup(SeasonConfig season, int index)
    {
        var levelmodel = season.GetLevelModel(index);

        title.SetFormatedText(season.Id + 1, index + 1);
        ballsLabel.SetFormatedText(levelmodel.startBallCount);
        allitemsDesc.SetActive(levelmodel.IsTargetExist == false);
        targetItemsDesc.SetActive(levelmodel.IsTargetExist == true);

        var ballsprite = GlobalFactory.Balls.GetSprite(Profile.Avatar.BallId);
        foreach (var ballimage in ballImages)
            ballimage.sprite = ballsprite;

        targetItem0Label.text = levelmodel.targetItem0.ToString();
        targetItem1Label.text = levelmodel.targetItem1.ToString();
        targetBallsLabel.text = levelmodel.targetBalls.ToString();
        targetBlocksLabel.text = levelmodel.targetBlocks.ToString();

        targetItem0Label.transform.parent.gameObject.SetActive(levelmodel.targetItem0 > 0);
        targetItem1Label.transform.parent.gameObject.SetActive(levelmodel.targetItem1 > 0);
        targetBallsLabel.transform.parent.gameObject.SetActive(levelmodel.targetBalls> 0);
        targetBlocksLabel.transform.parent.gameObject.SetActive(levelmodel.targetBlocks > 0);

        startButton.onClick.AddListener(() =>
        {
            if (Profile.Hearts > 0)
            {
                Back();

                PlayModel.Reset(PlayModel.Type.Levels);
                PlayModel.ballId = Profile.Avatar.BallId;
                PlayModel.level = levelmodel;

                UIBackground.Hide();
                Game.Instance.OpenState<State_Playing>();

                GlobalAnalytics.LevelStart(season.Id, index);
                PlayModel.onWin = () => GlobalAnalytics.LevelWin(season.Id, index, PlayModel.GetRewardStars());
                PlayModel.onLose = () => GlobalAnalytics.LevelFailed(season.Id, index);

            }
            else Game.Instance.OpenPopup<Popup_BuyHearts>();
        });

        return this;
    }

    private void Start()
    {
        UiShowHide.ShowAll(transform);
    }
}