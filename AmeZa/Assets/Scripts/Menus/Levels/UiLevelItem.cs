using SeganX;
using UnityEngine;
using UnityEngine.UI;

public class UiLevelItem : MonoBehaviour
{
    [SerializeField] private Button button = null;
    [SerializeField] private Text numberLabel = null;
    [SerializeField] private GameObject[] stars = null;
    [SerializeField] private Animation animator = null;

    public UiLevelItem Setup(SeasonModel season, int index, bool enabled)
    {
        numberLabel.text = GlobalFactory.Seasons.GetLevelNumber(season.id, index + 1).ToString();
        var earnstars = Profile.GetLevelStars(season.id, index);
        for (int i = 0; i < stars.Length; i++)
            stars[i].SetActive(earnstars > i);
        if (enabled && earnstars < 1)
            animator.Play();

        if (GlobalConfig.DebugMode == false)
            button.SetInteractable(enabled);

        button.onClick.AddListener(() =>
        {
            if (Profile.Energy.value > 0)
            {
                Game.Instance.OpenPopup<Popup_LevelInfo>().Setup(season, index, true);
            }
            else Game.Instance.OpenPopup<Popup_BuyHearts>();
        });

        return this;
    }
}
