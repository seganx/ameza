using SeganX;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UiProfileBallItem : MonoBehaviour
{
    private enum State { Owned, Enabled, Disabled }

    [SerializeField] private Image backImage = null;
    [SerializeField] private Image ballImage = null;
    [SerializeField] private LocalText priceLabel = null;
    [SerializeField] private Button button = null;

    private int id = 0;
    private State currState = State.Owned;

    public UiProfileBallItem Setup(int ballId)
    {
        id = ballId;
        ballImage.sprite = GlobalFactory.Balls.GetSprite(ballId);
        UpdateState();

        button.onClick.AddListener(() =>
        {
            switch (currState)
            {
                case State.Owned:
                    {
                        Profile.Avatar.BallId = ballId;
                        transform.root.Broadcast(Messages.Type.AvatarChanged, Profile.Avatar.Current);
                    }
                    break;

                case State.Enabled:
                    {
                        var price = GlobalFactory.Balls.GetPrice(ballId);
                        Game.SpendGems(price, () =>
                        {
                            Profile.AddBall(ballId);
                            Profile.Avatar.BallId = ballId;
                            Game.Instance.OpenPopup<Popup_Rewards>().Setup(ballId, 0, 0, 0, 0, true, false, () =>
                            {
                                transform.parent.Broadcast(Messages.Type.BallPurchased);
                                transform.root.Broadcast(Messages.Type.AvatarChanged, Profile.Avatar.Current);
                            });
                            GlobalAnalytics.SinkGem(price, "profile", "ball");
                        });
                    }
                    break;

                case State.Disabled:
                    break;
            }
        });
        return this;
    }

    private void OnMessage(Messages.Param param)
    {
        if (param.Is(Messages.Type.BallPurchased))
        {
            UpdateState();
        }
    }

    private void UpdateState()
    {
        if (Profile.HasBall(id).value)
        {
            currState = lastState = State.Owned;
            backImage.color = Color.white;
            if (priceLabel)
            {
                priceLabel.transform.parent.gameObject.SetActive(false);
                Destroy(priceLabel.transform.parent.gameObject);
            }
        }
        else if (lastState == State.Owned) // remain from previuse item
        {
            currState = lastState = State.Enabled;
            backImage.color = Color.white;
            priceLabel.SetText(GlobalFactory.Balls.GetPrice(id).ToString());
        }
        else
        {
            currState = lastState = State.Disabled;
            backImage.color = Color.gray;
#if UNITY_EDITOR && OFF
            var price = GlobalFactory.Balls.GetPrice(id);
            totalPrice += price;
            priceLabel.SetText(price.ToString());
            Debug.LogWarning(totalPrice);
#else
            priceLabel.SetText("?");
#endif
        }
    }


    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        if (currState == State.Owned)
            unlockedPosition = transform.GetAnchordPosition();
    }

    //////////////////////////////////////////////////
    /// STATIC MEMBERS
    //////////////////////////////////////////////////
    public static Vector2 unlockedPosition = Vector2.zero;
    private static State lastState = State.Owned;
#if UNITY_EDITOR && OFF
    private static int totalPrice = 0;
#endif
}
