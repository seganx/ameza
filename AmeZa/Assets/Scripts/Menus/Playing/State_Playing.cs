using SeganX;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class State_Playing : GameState
{
    [SerializeField] private Image backgroundImage = null;
    [SerializeField] private Text ballsLabel = null;
    [SerializeField] private UiVipBox vipBox = null;
    [SerializeField] private Button endTurnButton = null;
    [SerializeField] private Button pauseButton = null;
    [SerializeField] private UiTutorial tutorial = null;

    private ThemeSounds sounds = null;

    private bool BoosterVisibility => PlayModel.IsClassic || PlayModel.level.season > 0 || PlayModel.level.index > 1;

    private IEnumerator Start()
    {
        Profile.SetEnergy(Profile.Energy.value - 1);
        GlobalFactory.Theme.Select(PlayModel.level.theme);
        sounds = GlobalFactory.Theme.Selected.sounds.Clone<ThemeSounds>(transform);

        backgroundImage.sprite = GlobalFactory.Theme.Selected.playingBackground;
        endTurnButton.gameObject.SetActive(false);
        endTurnButton.onClick.AddListener(() => transform.Broadcast(Messages.Type.EndTurn));
        pauseButton.onClick.AddListener(() => Game.Instance.OpenPopup<Popup_Settings>());

        vipBox.gameObject.SetActive(BoosterVisibility);
        vipBox.Setup(ability =>
        {
            Game.Instance.OpenPopup<Popup_Effects>().Setup(ability, () => transform.Broadcast(Messages.Type.UseAbility, ability), CheckMission);
        });

        UIBackground.Hide();
        UiShowHide.ShowAll(transform);
        AudioManager.SetMusicVolumFactor(0.15f);

        yield return new WaitWhile(() => Game.Instance.CurrentPopup);
        yield return new WaitForSeconds(0.5f);
        if (PlayModel.IsLevels && PlayModel.level.season == 0 && PlayModel.level.index == 0)
        {
            tutorial.transform.GetChild(0).gameObject.SetActive(true);
            tutorial.transform.GetChild(1).gameObject.SetActive(false);
            tutorial.Display(0, false, 111037, null);
        }
    }

    private void Update()
    {
        ThemeSounds.UpdateMe(Time.unscaledTime);
    }

    private void OnMessage(Messages.Param param)
    {
        sounds.OnMessage(param);

        switch (param.type)
        {
            case Messages.Type.TurnStarted:
                ballsLabel.transform.position = BallManager.SpawnPoint;
                endTurnButton.gameObject.SetActive(true);
                vipBox.gameObject.SetActive(false);
                UpdateBallText(BallManager.balls.Count);

                if (PlayModel.level.index == 1)
                {
                    DelayCall(0.2f, () =>
                    {
                        tutorial.transform.GetChild(0).gameObject.SetActive(false);
                        tutorial.transform.GetChild(1).gameObject.SetActive(true);
                        if (tutorial.Display(0, true, 111038, () => Time.timeScale = 1))
                            Time.timeScale = 0.005f;
                    });
                }
                break;

            case Messages.Type.TurnEnded:
                ballsLabel.transform.position = BallManager.SpawnPoint;
                endTurnButton.gameObject.SetActive(false);
                vipBox.gameObject.SetActive(BoosterVisibility);
                UpdateBallText(BallManager.balls.Count);
                break;

            case Messages.Type.BallCount:
                UpdateBallText(param.As<int>());
                break;

            case Messages.Type.UseAbility:
                //abilityButton.transform.SetActiveChild(BlockManager.IsBlockReachedWarn ? 1 : 0);
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
        bool blocksOut = PlayModel.IsClearBlocks && BlockManager.blocks.Exists(x => x.IsDangerous) == false;
        bool targetOut = PlayModel.IsTargetExist && PlayModel.IsTargetsReached;

        if (isTurnOut || blocksOut || targetOut) // no blocks or target remained
        {
            // now check if player wins
            isPlayerWins = PlayModel.IsTargetExist == false || PlayModel.IsTargetsReached;
            if (isPlayerWins)
                PlayModel.onWin?.Invoke(ok => { if (ok) base.Back(); });
            else
                PlayerLose();
        }

        if (isPlayerWins == false)
        {
            if (BlockManager.IsBlockReachedDown) // player is losing because a block reached down
            {
                Game.Instance.ClosePopup(true);
                Game.Instance.OpenPopup<Popup_PreLose>().Setup(true, ability => // check if player wants to use abilities
                {
                    if (ability != AbilityType.Null)
                        Game.Instance.OpenPopup<Popup_Effects>().Setup(ability, () => transform.Broadcast(Messages.Type.UseAbility, ability), CheckMission);
                    else
                        PlayerLose();
                });
            }
        }

        //abilityButton.transform.SetActiveChild(BlockManager.IsBlockReachedWarn ? 1 : 0);
    }

    public override void Back()
    {
        if (tutorial.Hide()) return;

        Game.Instance.OpenPopup<Popup_Confirm>().Setup(111005, true, true, yes =>
        {
            if (yes)
                PlayerLose();
        }).GetComponent<UiCharacter>(true, true).SetBody(1).SetFace(2);
    }

    public void PlayerLose()
    {
        if (PlayModel.onLose != null)
            PlayModel.onLose(ok => { if (ok) base.Back(); });
        else
            base.Back();
    }
}
