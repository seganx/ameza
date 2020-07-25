using SeganX;
using UnityEngine;


public class Popup_MedalInfo : GameState
{
    [SerializeField] private LocalText[] nameLabel = null;
    [SerializeField] private LocalText[] infoLabel = null;
    [SerializeField] private LocalText[] rewardLabel = null;

    public Popup_MedalInfo Setup(GlobalConfig.Data.League info)
    {
        for (int i = 0; i < nameLabel.Length; i++)
            nameLabel[i].SetText(info.subleagus[i].name);

        for (int i = 0; i < infoLabel.Length; i++)
            infoLabel[i].SetFormatedText(info.subleagus[i].startScore);

        for (int i = 0; i < rewardLabel.Length; i++)
            rewardLabel[i].SetText(info.subleagus[i].rewardGems.ToString());

        return this;
    }

    private void Start()
    {
        UiShowHide.ShowAll(transform);
    }

}
