using SeganX;
using UnityEngine;
using UnityEngine.UI;

public class UiAvatar : MonoBehaviour
{
    [SerializeField] private Image hair = null;
    [SerializeField] private Image head = null;

    private ProfileData.AvatarData data = null;

    public UiAvatar Setup(string json)
    {
        try
        {
            data = json.HasContent(3) ? JsonUtility.FromJson<ProfileData.AvatarData>(json) : new ProfileData.AvatarData();
        }
        catch { }

        return Setup(data == null ? new ProfileData.AvatarData() : data);
    }

    public UiAvatar Setup(ProfileData.AvatarData avatar)
    {
        data = avatar;
        UpdateVisual();
        return this;
    }

    // Use this for initialization
    private void Start()
    {
        if (data == null)
        {
            data = Profile.Avatar.Current;
            UpdateVisual();
        }
    }

    private void OnMessage(Messages.Param param)
    {
        if (param.Is(Messages.Type.AvatarChanged))
            UpdateVisual();
    }

    public void UpdateVisual()
    {
        head.sprite = GlobalFactory.Balls.GetSprite(data.ballId);
        hair.rectTransform.localEulerAngles = Vector3.forward * data.angle;
        var color = head.sprite.GetAverageColor();
        color.a = 1;
        hair.color = color;
    }
}
