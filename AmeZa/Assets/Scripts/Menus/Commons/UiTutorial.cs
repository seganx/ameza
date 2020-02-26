using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiTutorial : MonoBehaviour
{
    [SerializeField] private LocalText label = null;
    [SerializeField] private Button backButton = null;

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
        Invoke("Show", delay);
        return true;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    public bool DisplayJoke(float delay)
    {
        if (GlobalFactory.Joke.Exist == false) return false;
        var jokparts = GlobalFactory.Joke.Pick();

        if (jokparts.Length < 1) return false;
        Display(delay, jokparts[0], () =>
        {
            if (jokparts.Length < 2) return;
            Display(0, jokparts[1], () =>
            {
                if (jokparts.Length < 3) return;
                Display(0, jokparts[2], () =>
                {
                    if (jokparts.Length < 4) return;
                    Display(0, jokparts[3], null);
                });
            });
        });

        return true;
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
