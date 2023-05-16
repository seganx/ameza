using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiVipBox : MonoBehaviour
{
    [SerializeField] private Button bombButton = null;
    [SerializeField] private LocalText bombLabel = null;
    [SerializeField] private GameObject bombLock = null;
    [SerializeField] private Button hammerButton = null;
    [SerializeField] private LocalText hammerLabel = null;
    [SerializeField] private GameObject hammerLock = null;
    [SerializeField] private Button missleButton = null;
    [SerializeField] private LocalText missleLabel = null;
    [SerializeField] private GameObject missleLock = null;

    public UiVipBox Setup(System.Action<AbilityType> callback)
    {
        UpdateLocks();
        UpdateTexts();

        bombButton.onClick.AddListener(() => CheckVIP(() =>
        {
            if (Profile.Bombs > 0)
            {
                Profile.Bombs--;
                Profile.Skill -= GlobalConfig.Difficulty.loseFactor / 1;
                callback(AbilityType.Bomb);
                UpdateTexts();
            }
            else BuyBooster(GlobalConfig.ProfilePreset.bombs, GlobalConfig.Shop.bombPrice, "bomb", count => Profile.Bombs += count);
        }));

        hammerButton.onClick.AddListener(() => CheckVIP(() =>
        {
            if (Profile.Hammers > 0)
            {
                Profile.Hammers--;
                Profile.Skill -= GlobalConfig.Difficulty.loseFactor / 3;
                callback(AbilityType.Hammer);
                UpdateTexts();
            }
            else BuyBooster(GlobalConfig.ProfilePreset.hammers, GlobalConfig.Shop.hammerPrice, "hammer", count => Profile.Hammers += count);
        }));

        missleButton.onClick.AddListener(() => CheckVIP(() =>
        {
            if (Profile.Missiles > 0)
            {
                Profile.Missiles--;
                Profile.Skill -= GlobalConfig.Difficulty.loseFactor / 2;
                callback(AbilityType.Missle);
                UpdateTexts();
            }
            else BuyBooster(GlobalConfig.ProfilePreset.missles, GlobalConfig.Shop.misslePrice, "scissor", count => Profile.Missiles += count);
        }));

        return this;
    }

    private void UpdateTexts()
    {
        bombLabel.SetText(Profile.Bombs > 0 ? $"x{Profile.Bombs}" : "+");
        hammerLabel.SetText(Profile.Hammers > 0 ? $"x{Profile.Hammers}" : "+");
        missleLabel.SetText(Profile.Missiles > 0 ? $"x{Profile.Missiles}" : "+");
    }

    private void UpdateLocks()
    {
        var islock = Profile.IsVIP == false;
        bombLock.SetActive(islock);
        hammerLock.SetActive(islock);
        missleLock.SetActive(islock);
    }

    private void CheckVIP(System.Action callback)
    {
        if (Profile.IsVIP)
        {
            callback?.Invoke();
        }
        else
        {
            Game.Instance.OpenPopup<Popup_Confirm>().Setup(111130, true, false, yes =>
            {
                Game.Instance.OpenPopup<Popup_Shop>().SetOnClose(UpdateLocks);
            });
        }
    }

    private void BuyBooster(int count, int price, string booster, System.Action<int> onSuccess)
    {
        Game.Instance.OpenPopup<Popup_Confirm>().SetText(111003, count, price).Setup(true, true, yes =>
        {
            if (yes == false) return;
            Game.SpendGems(price, () =>
            {
                onSuccess(count);
                UpdateTexts();
                if (PlayModel.IsClassic)
                    GlobalAnalytics.SinkGem(price, "classic", booster);
                else if (PlayModel.IsLeague)
                    GlobalAnalytics.SinkGem(price, "leagues", booster);
                else if (PlayModel.IsLevels)
                    GlobalAnalytics.SinkGem(price, "levels", booster);

            });
        });
    }
}
