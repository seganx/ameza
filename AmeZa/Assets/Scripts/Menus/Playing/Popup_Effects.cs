using SeganX;
using System.Collections;
using UnityEngine;

public class Popup_Effects : GameState
{
    [SerializeField] private GameObject bomb = null;
    [SerializeField] private GameObject hammer = null;
    [SerializeField] private GameObject dynamite = null;

    private float timer = 0;
    private System.Action onCloseFunc = null;
    private System.Action onActionFunc = null;
    private AbilityType type = AbilityType.Null;

    public Popup_Effects Setup(AbilityType ability, System.Action onAction, System.Action onClose)
    {
        type = ability;
        onActionFunc = onAction;
        onCloseFunc = onClose;
        return this;
    }

    private void Awake()
    {
        bomb.SetActive(false);
        hammer.SetActive(false);
        dynamite.SetActive(false);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        bomb.SetActive(type == AbilityType.Bomb);
        hammer.SetActive(type == AbilityType.Hammer);
        dynamite.SetActive(type == AbilityType.Missle);

        switch (type)
        {
            case AbilityType.Bomb:
                yield return new WaitForSeconds(0.85f);
                Vibration.Vibrate(200);
                break;
            case AbilityType.Hammer:
                yield return new WaitForSeconds(1);
                Vibration.Vibrate(100);
                break;
            case AbilityType.Missle:
                yield return new WaitForSeconds(0.75f);
                Vibration.Vibrate(150);
                break;
        }

        onActionFunc();

        yield return new WaitForSeconds(1);
        onCloseFunc?.Invoke();
        base.Back();
    }

    public override void Back()
    {

    }
}
