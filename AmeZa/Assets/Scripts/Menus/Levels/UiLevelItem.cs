using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiLevelItem : MonoBehaviour
{
    [SerializeField] private Button button = null;
    [SerializeField] private Text numberLabel = null;
    [SerializeField] private GameObject[] stars = null;

    public UiLevelItem Setup(SeasonConfig season, int index, bool enabled)
    {
        numberLabel.text = (index + 1).ToString();
        var earnstars = Profile.GetLevelStars(season.Id, index);
        for (int i = 0; i < stars.Length; i++)
            stars[i].SetActive(earnstars > i);

        button.SetInteractable(enabled);
        button.onClick.AddListener(() =>
        {
            if (Profile.Hearts > 0)
            {
                PlayModel.Reset(PlayModel.Type.Levels);
                PlayModel.ballId = Profile.Avatar.BallId;
                PlayModel.level = season.GetLevelModel(index);

                UIBackground.Hide();
                Game.Instance.OpenState<State_Playing>();

                GlobalAnalytics.LevelStart(season.Id, index);
                PlayModel.onWin = () => GlobalAnalytics.LevelWin(season.Id, index, PlayModel.GetRewardStars());
                PlayModel.onLose = () => GlobalAnalytics.LevelFailed(season.Id, index);
            }
            else Game.Instance.OpenPopup<Popup_BuyHearts>();
        });

        return this;
    }
}
