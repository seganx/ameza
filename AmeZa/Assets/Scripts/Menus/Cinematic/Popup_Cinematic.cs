using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_Cinematic : GameState
{
    [SerializeField] private Image backgroundImage = null;
    [SerializeField] private Color emptyBackgroundColor = Color.black;
    [SerializeField] private UiCharacter ameza = null;
    [SerializeField] private UiCharacter ame = null;
    [SerializeField] private LocalText dialogLabel = null;
    [SerializeField] private Button nextButton = null;

    private int index = 0;
    private CinematicConfig cinematic = null;

    public Popup_Cinematic Setup(CinematicConfig cinematic, System.Action onClose)
    {
        this.cinematic = cinematic;

        backgroundImage.sprite = cinematic.background;
        backgroundImage.color = cinematic.background == null ? emptyBackgroundColor : Color.white;

        nextButton.onClick.AddListener(() =>
        {
            if (index < cinematic.sequences.Count)
            {
                DisplaySquence(cinematic.sequences[index++]);
            }
            else
            {
                base.Back();
                if (onClose != null) onClose();
            }
        });

        UiShowHide.ShowAll(transform);
        nextButton.onClick.Invoke();
        return this;
    }

    public override void Back()
    {
        nextButton.onClick.Invoke();
    }

    private void DisplaySquence(CinematicConfig.Sequence sequence)
    {
        switch (sequence.character)
        {
            case CinematicConfig.Character.Empty:
                UiShowHide.HideAll(ameza.transform);
                UiShowHide.HideAll(ame.transform);
                break;
            case CinematicConfig.Character.Ameza:
                UiShowHide.HideAll(ame.transform);
                UiShowHide.ShowAll(ameza.transform);
                ameza.SetBody((int)sequence.body);
                ameza.SetFace((int)sequence.face);
                break;
            case CinematicConfig.Character.Ame:
                UiShowHide.HideAll(ameza.transform);
                UiShowHide.ShowAll(ame.transform);
                ame.SetBody((int)sequence.body);
                ame.SetFace((int)sequence.face);
                break;
        }

        dialogLabel.SetText(sequence.text);
    }
}
