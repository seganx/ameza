using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiLeagueItem : Base
{
    [SerializeField] private int index = 0;
    [SerializeField] private Image image = null;
    [SerializeField] private LocalText nameLabel = null;
    [SerializeField] private LocalText descLabel = null;
    [SerializeField] private LocalText ribbonLabel = null;
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
        image.sprite = GlobalFactory.Leagues.GetCupSprite(league.playType);
        nameLabel.SetText(league.name);
        descLabel.SetText(league.desc);

        if (league.ribbon.HasContent())
            ribbonLabel.SetText(league.ribbon);
        else
            ribbonLabel.gameObject.SetActive(false);

        button.onClick.AddListener(() =>
        {
            if (Profile.HasNickname)
            {
                gameManager.OpenState<State_Leaderboards>().Setup(index);
            }
            else
            {
                gameManager.OpenPopup<Popup_Confirm>().Setup(111011, true, false, ok =>
                {
                    if (ok) gameManager.OpenPopup<Popup_Profile>().SetOnClose(() =>
                    {
                        if (Profile.HasNickname)
                            gameManager.OpenPopup<Popup_Confirm>().Setup(111012, true, false, null);
                    });
                });
            }
        });
    }
}
