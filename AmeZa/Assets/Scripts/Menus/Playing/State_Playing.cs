using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_Playing : GameState
{
    [SerializeField] private Button endTurnButton = null;

    private void Start()
    {
        endTurnButton.onClick.AddListener(() => transform.Broadcast(Messages.Type.EndTurn));
        endTurnButton.gameObject.SetActive(false);

        UIBackground.Hide();
        UiShowHide.ShowAll(transform);
    }

    private void OnMessage(Messages.Param param)
    {
        switch (param.type)
        {
            case Messages.Type.TurnStarted:
                endTurnButton.gameObject.SetActive(true);
                break;

            case Messages.Type.TurnEnded:
                endTurnButton.gameObject.SetActive(false);
                break;
        }
    }

    private void CheckMission()
    {
        if (BlockManager.IsBlockReachedDown) // player is losing becase a block reached down
        {
            gameManager.OpenPopup<Popup_Lose>().Setup(ability => // check if player wants to use abilities
            {
                if (ability != AbilityType.Null)
                {
                    transform.Broadcast(Messages.Type.UseAbility, ability);
                    CheckMission();
                }
                else Back();
            });
        }
        else
        {
            var isTurnOut = PlayModel.IsTurnsFinished;
            var blocksOut = BlockManager.blocks.Exists(x => x.Type == BlockType.Value || x.Type == BlockType.Ball) == false;
            var targetOut = PlayModel.IsTargetExist && PlayModel.IsTargetsReached;

            if (isTurnOut || blocksOut || targetOut) // no blocks or target remained
            {
                // now check if player wins
                var playerWin = PlayModel.IsTargetExist == false || PlayModel.IsTargetsReached;
                if (playerWin)
                {
                    if (PlayModel.onWin != null)
                        PlayModel.onWin();
                    else
                        gameManager.OpenPopup<Popup_Win>().SetNextTask(() => base.Back());
                }
                else Back();
            }
        }
    }

    public override void Back()
    {
        if (PlayModel.onLose != null)
            PlayModel.onLose();
    }
}
