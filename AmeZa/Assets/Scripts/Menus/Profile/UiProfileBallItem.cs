using SeganX;
using System.Collections;
using System.Collections.Generic;
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
                        Profile.Avatar.ballId = ballId;
                        transform.root.Broadcast(Messages.Type.AvatarChanged, Profile.Avatar);
                    }
                    break;

                case State.Enabled:
                    {
                        var price = GlobalFactory.Balls.GetPrice(ballId);
                        Game.SpendGems(price, () =>
                        {
                            Profile.AddBall(ballId);
                            Profile.Avatar.ballId = ballId;
                            Game.Instance.OpenPopup<Popup_Rewards>().Setup(0, ballId, true, () =>
                            {
                                transform.parent.Broadcast(Messages.Type.BallPurchased);
                                transform.root.Broadcast(Messages.Type.AvatarChanged, Profile.Avatar);
                            });
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
        if (Profile.HasBall(id))
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
            priceLabel.SetText("?");
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
}
