using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_Playing : GameState
{
    [SerializeField] private Image backgroundImage = null;
    [SerializeField] private Text ballsLabel = null;
    [SerializeField] private Button abilityButton = null;
    [SerializeField] private Button endTurnButton = null;
    [SerializeField] private Button pauseButton = null;
    [SerializeField] private UiTutorial tutorial = null;

    private IEnumerator Start()
    {
        backgroundImage.sprite = GlobalFactory.Theme.GetBackground(PlayModel.level.theme);
        endTurnButton.gameObject.SetActive(false);
        endTurnButton.onClick.AddListener(() => transform.Broadcast(Messages.Type.EndTurn));
        pauseButton.onClick.AddListener(() => gameManager.OpenPopup<Popup_Settings>());

        abilityButton.transform.SetActiveChild(0);
        abilityButton.onClick.AddListener(() =>
        {
            gameManager.ClosePopup(true);
            gameManager.OpenPopup<Popup_Lose>().Setup(false, ability => // check if player wants to use abilities
            {
                if (ability != AbilityType.Null)
                    gameManager.OpenPopup<Popup_Effects>().Setup(ability, () => transform.Broadcast(Messages.Type.UseAbility, ability), CheckMission);
            });

        });

        UIBackground.Hide();
        UiShowHide.ShowAll(transform);
        AudioManager.PlayRandom(1, 50, 0.2f, 2, 2);

        yield return new WaitForSeconds(0.5f);
        if (PlayModel.level.season == 0 && PlayModel.level.index < 3)
        {
            tutorial.transform.GetChild(0).gameObject.SetActive(true);
            tutorial.transform.GetChild(1).gameObject.SetActive(false);
            tutorial.Display(true, 111037, () => tutorial.transform.GetChild(0).gameObject.SetActive(false));
        }
        else
        {
            tutorial.transform.GetChild(0).gameObject.SetActive(false);
            tutorial.transform.GetChild(1).gameObject.SetActive(true);
            tutorial.Display(true, 111038, () => tutorial.transform.GetChild(1).gameObject.SetActive(false));
        }
    }

    private void OnMessage(Messages.Param param)
    {
        switch (param.type)
        {
            case Messages.Type.TurnStarted:
                ballsLabel.transform.position = BallManager.SpawnPoint;
                endTurnButton.gameObject.SetActive(true);
                abilityButton.gameObject.SetActive(false);
                UpdateBallText(BallManager.balls.Count);
                break;

            case Messages.Type.TurnEnded:
                ballsLabel.transform.position = BallManager.SpawnPoint;
                endTurnButton.gameObject.SetActive(false);
                abilityButton.gameObject.SetActive(true);
                UpdateBallText(BallManager.balls.Count);
                break;

            case Messages.Type.BallCount:
                UpdateBallText(param.As<int>());
                break;

            case Messages.Type.UseAbility:
                abilityButton.transform.SetActiveChild(BlockManager.IsBlockReachedWarn ? 1 : 0);
                break;
        }
    }

    private void UpdateBallText(int i)
    {
        ballsLabel.text = i.ToString();
        ballsLabel.gameObject.SetActive(i > 0);
    }

    private void CheckMission()
    {
        bool isPlayerWins = false;
        bool isTurnOut = PlayModel.IsTurnsFinished;
        bool blocksOut = PlayModel.IsClearBlocks && BlockManager.blocks.Exists(x => x.Type == BlockType.Value || x.Type == BlockType.Ball) == false;
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

        if (isPlayerWins == false)
        {
            if (BlockManager.IsBlockReachedDown) // player is losing becase a block reached down
            {
                gameManager.ClosePopup(true);
                gameManager.OpenPopup<Popup_Lose>().Setup(true, ability => // check if player wants to use abilities
                {
                    if (ability != AbilityType.Null)
                        gameManager.OpenPopup<Popup_Effects>().Setup(ability, () => transform.Broadcast(Messages.Type.UseAbility, ability), CheckMission);
                    else
                        PlayerLose();
                });
            }
        }

        abilityButton.transform.SetActiveChild(BlockManager.IsBlockReachedWarn ? 1 : 0);
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
