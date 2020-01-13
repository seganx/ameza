using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiTutorial : MonoBehaviour
{
    [SerializeField] private LocalText label = null;
    [SerializeField] private Button backButton = null;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public UiTutorial Display(bool displayOnce, int stringId, System.Action onClose)
    {
        if (displayOnce)
        {
            bool displayed = PlayerPrefs.GetInt(name + ".Totorial." + stringId, 0) > 0;
            if (displayed) return this;
            PlayerPrefs.SetInt(name + ".Totorial." + stringId, 1);
        }

        label.SetText(LocalizationService.Get(stringId));

        backButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            if (onClose != null) onClose();
        });

        gameObject.SetActive(true);
        return this;
    }


}
