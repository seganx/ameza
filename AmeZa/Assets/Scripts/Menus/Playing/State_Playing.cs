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
        GlobalFactory.Theme.Select(PlayModel.level.theme);
        backgroundImage.sprite = GlobalFactory.Theme.Selected.playingBackground;
        endTurnButton.gameObject.SetActive(false);
        endTurnButton.onClick.AddListener(() => transform.Broadcast(Messages.Type.EndTurn));
        pauseButton.onClick.AddListener(() => game.OpenPopup<Popup_Settings>());

        abilityButton.transform.SetActiveChild(0);
        abilityButton.onClick.AddListener(() =>
        {
            game.ClosePopup(true);
            game.OpenPopup<Popup_Lose>().Setup(false, ability => // check if player wants to use abilities
            {
                if (ability != AbilityType.Null)
                    game.OpenPopup<Popup_Effects>().Setup(ability, () => transform.Broadcast(Messages.Type.UseAbility, ability), CheckMission);
            });

        });

        UIBackground.Hide();
        UiShowHide.ShowAll(transform);
        AudioManager.PlayRandom(1, 50, 0.2f, 2, 2);

        yield return new WaitForSeconds(0.5f);
        if (PlayModel.level.season == 0 && (PlayModel.level.index == 0 || PlayModel.level.index == 1))
        {
            tutorial.transform.GetChild(0).gameObject.SetActive(true);
            tutorial.transform.GetChild(1).gameObject.SetActive(false);
            tutorial.transform.GetChild(2).gameObject.SetActive(false);
            tutorial.Display(0, false, 111037, null);
        }
        else if (PlayModel.level.index > 1)
        {
            tutorial.transform.GetChild(0).gameObject.SetActive(false);
            tutorial.transform.GetChild(1).gameObject.SetActive(false);
            tutorial.transform.GetChild(2).gameObject.SetActive(true);
            tutorial.Display(0, true, 111044, null);
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

                if (PlayModel.level.index == 1)
                {
                    DelayCall(0.2f, () =>
                    {
                        tutorial.transform.GetChild(0).gameObject.SetActive(false);
                        tutorial.transform.GetChild(1).gameObject.SetActive(true);
                        tutorial.transform.GetChild(2).gameObject.SetActive(false);
                        if (tutorial.Display(0, true, 111038, () => Time.timeScale = 1))
                            Time.timeScale = 0.005f;
                    });
                }
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
                game.ClosePopup(true);
                game.OpenPopup<Popup_Win>().SetNextTask(() =>
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
                game.ClosePopup(true);
                game.OpenPopup<Popup_Lose>().Setup(true, ability => // check if player wants to use abilities
                {
                    if (ability != AbilityType.Null)
                        game.OpenPopup<Popup_Effects>().Setup(ability, () => transform.Broadcast(Messages.Type.UseAbility, ability), CheckMission);
                    else
                        PlayerLose();
                });
            }
        }

        abilityButton.transform.SetActiveChild(BlockManager.IsBlockReachedWarn ? 1 : 0);
    }

    public override void Back()
    {
        if (tutorial.Hide()) return;

        game.OpenPopup<Popup_Confirm>().Setup(111005, true, true, yes =>
        {
            if (yes)
                PlayerLose();
        }).GetComponent<UiCharacter>(true, true).SetBody(1).SetFace(2);
    }

    public void PlayerLose()
    {
        if (Profile.Hearts > 0)
            Profile.Hearts--;
        base.Back();
        if (PlayModel.onLose != null)
            PlayModel.onLose();
    }

#if UNITY_EDITOR
#else
    private void OnDestroy()
    {
        Online.Stats.Set(GlobalConfig.Instance.version, Profile.Gems, Profile.Skill, Profile.GetLevelsPassed(), success => { });
    }
#endif
}
