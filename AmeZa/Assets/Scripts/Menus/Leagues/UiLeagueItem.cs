using SeganX;
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
                Game.Instance.OpenState<State_Leaderboards>().Setup(index);
            }
            else
            {
                Game.Instance.OpenPopup<Popup_Confirm>().Setup(111011, true, false, ok =>
                {
                    if (ok) Game.Instance.OpenPopup<Popup_Profile>().SetOnClose(() =>
                    {
                        if (Profile.HasNickname)
                            Game.Instance.OpenPopup<Popup_Confirm>().Setup(111012, true, false, null);
                    });
                });
            }
        });
    }
}
