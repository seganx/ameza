using SeganX;
using UnityEngine;
using UnityEngine.UI;

public class Popup_PreLose : GameState
{
    [SerializeField] private UiVipBox vipBox = null;
    [SerializeField] private GameObject backButton = null;
    [SerializeField] private GameObject homeContent = null;
    [SerializeField] private Button homeButton = null;


    private System.Action<AbilityType> callbackFunc = null;

    public Popup_PreLose Setup(bool displayHomeButton, System.Action<AbilityType> callback)
    {
        callbackFunc = callback;

        vipBox.Setup(ability =>
        {
            callback?.Invoke(ability);
            base.Back();
        });

        homeButton.onClick.AddListener(() =>
        {
            if (PlayModel.onPreLose != null)
                PlayModel.onPreLose(ok => { if (ok) Back(); });
            else
                Back();
        });

        backButton.SetActive(displayHomeButton == false);
        homeContent.SetActive(displayHomeButton == true);

        UiShowHide.ShowAll(transform);
        return this;
    }

    public override void Back()
    {
        base.Back();
        callbackFunc(AbilityType.Null);
    }
}
