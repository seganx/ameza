using SeganX;
using UnityEngine;
using UnityEngine.UI;

public class UiTutorial : MonoBehaviour
{
    [SerializeField] private LocalText label = null;
    [SerializeField] private Button backButton = null;
    [SerializeField] private UiCharacter character = null;

    public bool Visible { get; private set; }

    private System.Action onCloseFunc = null;

    private void Awake()
    {
        gameObject.SetActive(false);
        backButton.onClick.AddListener(() => Hide());
    }

    public bool Display(float delay, bool displayOnce, int stringId, System.Action onClose)
    {
        if (displayOnce)
        {
            bool displayed = PlayerPrefs.GetInt(name + ".Totorial." + stringId, 0) > 0;
            if (displayed) return false;
            PlayerPrefs.SetInt(name + ".Totorial." + stringId, 1);
        }

        return Display(delay, LocalizationService.Get(stringId), onClose);
    }

    public bool Display(float delay, string str, System.Action onClose)
    {
        Visible = true;
        label.SetText(str);
        onCloseFunc = onClose;
        Invoke(nameof(Show), delay);
        return true;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    public bool DisplayJoke(float delay)
    {
        if (GlobalFactory.Jokes.Exist == false) return false;
        Invoke(nameof(DoDisplayJoke), delay);
        return true;
    }

    public void DoDisplayJoke()
    {
        var jokparts = GlobalFactory.Jokes.Pick();
        if (jokparts.IsNullOrEmpty() || jokparts.Length < 1) return;

        Display(0, jokparts[0], () =>
        {
            if (jokparts.Length > 1)
            {
                if (character) character.SetBody(1).SetFace(0);
                Display(0, jokparts[1], () =>
                {
                    if (jokparts.Length > 2)
                        Display(0, jokparts[2], () =>
                        {
                            if (jokparts.Length > 3)
                                Display(0, jokparts[3], () => { if (character) character.SetBody(0).SetFace(1); });
                            else if (character) character.SetBody(0).SetFace(1);
                        });
                    else if (character) character.SetBody(0).SetFace(1);
                });
            }
            else if (character) character.SetBody(0).SetFace(1);
        });
    }

    // return true if tutorial was opened and false if not
    public bool Hide()
    {
        CancelInvoke();
        if (Visible)
        {
            Visible = false;
            gameObject.SetActive(false);
            if (onCloseFunc != null) onCloseFunc();
            return true;
        }
        else return false;
    }
}
