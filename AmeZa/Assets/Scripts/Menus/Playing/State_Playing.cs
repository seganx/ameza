using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_Playing : GameState
{
    [SerializeField] private Button endTurnButton = null;

    // Use this for initialization
    private void Start()
    {
        endTurnButton.onClick.AddListener(() => transform.Broadcast(Messages.Type.EndTurn));
    }

    // Update is called once per frame
    private void Update()
    {

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
}
