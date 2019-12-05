using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_Playing : GameState
{
    [SerializeField] private Text ballsLabel = null;
    [SerializeField] private Button endTurnButton = null;
    [SerializeField] private Button pauseButton = null;

    private void Start()
    {
        endTurnButton.gameObject.SetActive(false);
        endTurnButton.onClick.AddListener(() => transform.Broadcast(Messages.Type.EndTurn));
        pauseButton.onClick.AddListener(() => gameManager.OpenPopup<Popup_Settings>());

        UIBackground.Hide();
        UiShowHide.ShowAll(transform);
        AudioManager.PlayRandom(1, 5, 0.25f, 2, 2);
    }

    private void OnMessage(Messages.Param param)
    {
        switch (param.type)
        {
            case Messages.Type.TurnStarted:
                endTurnButton.gameObject.SetActive(true);
                UpdateBallText(BallManager.balls.Count);
                break;

            case Messages.Type.TurnEnded:
                endTurnButton.gameObject.SetActive(false);
                UpdateBallText(BallManager.balls.Count);
                break;

            case Messages.Type.BallCount:
                UpdateBallText(param.As<int>());
                break;
        }
    }

    private void UpdateBallText(int i)
    {
        ballsLabel.transform.position = BallManager.SpawnPoint;
        ballsLabel.text = i.ToString();
        ballsLabel.gameObject.SetActive(i > 0);
    }

    private void CheckMission()
    {
        bool isPlayerWins = false;
        bool isTurnOut = PlayModel.IsTurnsFinished;
        bool blocksOut = PlayModel.IsBlockTargeted && BlockManager.blocks.Exists(x => x.Type == BlockType.Value || x.Type == BlockType.Ball) == false;
        bool targetOut = PlayModel.IsTargetExist && PlayModel.IsTargetsReached;

        if (isTurnOut || blocksOut || targetOut) // no blocks or target remained
        {
            // now check if player wins
            isPlayerWins = PlayModel.IsTargetExist == false || PlayModel.IsTargetsReached;
            if (isPlayerWins)
            {
                gameManager.ClosePopup(true);
                gameManager.OpenPopup<Popup_Win>().SetNextTask(() =>
                {
                    base.Back();
                    if (PlayModel.onWin != null)
                        PlayModel.onWin();
                });
            }
            else PlayerLose();
        }

        if (BlockManager.IsBlockReachedDown && isPlayerWins == false) // player is losing becase a block reached down
        {
            gameManager.ClosePopup(true);
            gameManager.OpenPopup<Popup_Lose>().Setup(ability => // check if player wants to use abilities
            {
                if (ability != AbilityType.Null)
                {
                    transform.Broadcast(Messages.Type.UseAbility, ability);
                    CheckMission();
                }
                else PlayerLose();
            });
        }
    }

    public override void Back()
    {
        gameManager.OpenPopup<Popup_Confirm>().Setup(111005, true, true, yes =>
        {
            if (yes)
                PlayerLose();
        });
    }

    public void PlayerLose()
    {
        if (Profile.Hearts > 0)
            Profile.Hearts--;
        base.Back();
        if (PlayModel.onLose != null)
            PlayModel.onLose();
    }
}
