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

    public UiLevelItem Setup(SeasonConfig season, int index)
    {
        numberLabel.text = (index + 1).ToString();
        var earnstars = Profile.GetLevelStars(season.Id, index);
        for (int i = 0; i < stars.Length; i++)
            stars[i].SetActive(earnstars > i);

        button.SetInteractable(Profile.CanOpenLevel(season.Id, index));
        button.onClick.AddListener(() =>
        {
            PlayModel.Reset(PlayModel.Type.Levels);
            PlayModel.ballId = Profile.Avatar.ballId;
            PlayModel.level = season.GetLevelModel(index);

            UIBackground.Hide();
            Game.Instance.OpenState<State_Playing>();
        });

        return this;
    }
}
