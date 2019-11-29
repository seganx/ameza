using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiLeagueItem : Base
{
    [SerializeField] private int index = 0;
    [SerializeField] private LocalText nameLabel = null;
    [SerializeField] private LocalText descLabel = null;
    [SerializeField] private Button button = null;

    private void Awake()
    {
        if (index >= GlobalConfig.Leagues.Count)
            Destroy(gameObject);
    }

    // Use this for initialization
    private void Start()
    {
        var league = GlobalConfig.Leagues[index];
        nameLabel.SetText(league.name);
        descLabel.SetText(league.desc);
        button.onClick.AddListener(() => gameManager.OpenState<State_LeagueInfo>().SetLeagueIndex(index));
    }
}
