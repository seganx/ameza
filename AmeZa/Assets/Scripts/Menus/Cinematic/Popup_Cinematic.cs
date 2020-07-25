using SeganX;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

public class Popup_Cinematic : GameState
{
    [SerializeField] private Image backgroundImage = null;
    [SerializeField] private Color emptyBackgroundColor = Color.black;
    [SerializeField] private UiCharacter[] ame = null;
    [SerializeField] private UiCharacter[] ameza = null;
    [SerializeField] private UiCharacter[] jocker = null;
    [SerializeField] private LocalText dialogLabel = null;
    [SerializeField] private Button nextButton = null;

    private int side = 0;
    private int index = 0;
    private CinematicConfig.Character lastChar = CinematicConfig.Character.Empty;


    public Popup_Cinematic Setup(CinematicConfig cinematic, System.Action onClose)
    {
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
                onClose?.Invoke();
            }
        });

        UiShowHide.ShowAll(transform);

        for (int i = 0; i < 2; i++)
        {
            UiShowHide.HideAll(ame[i].transform);
            UiShowHide.HideAll(ameza[i].transform);
            UiShowHide.HideAll(jocker[i].transform);
        }

        nextButton.onClick.Invoke();
        return this;
    }

    public override void Back()
    {
        nextButton.onClick.Invoke();
    }

    private void DisplaySquence(CinematicConfig.Sequence sequence)
    {
        if (sequence.character != lastChar)
        {
            UiShowHide.HideAll(ame[side].transform);
            UiShowHide.HideAll(ameza[side].transform);
            UiShowHide.HideAll(jocker[side].transform);
            side = (side == 0) ? 1 : 0;
            lastChar = sequence.character;
        }

        switch (sequence.character)
        {
            case CinematicConfig.Character.Ameza:
                UiShowHide.ShowAll(ameza[side].transform);
                ameza[side].SetBody((int)sequence.body);
                ameza[side].SetFace((int)sequence.face);
                AudioManager.PlaySound(GlobalFactory.Cinematics.soundAmeZa);
                break;
            case CinematicConfig.Character.Ame:
                UiShowHide.ShowAll(ame[side].transform);
                ame[side].SetBody((int)sequence.body);
                ame[side].SetFace((int)sequence.face);
                AudioManager.PlaySound(GlobalFactory.Cinematics.soundAme);
                break;
            case CinematicConfig.Character.Joker:
                UiShowHide.ShowAll(jocker[side].transform);
                jocker[side].SetBody((int)sequence.body);
                jocker[side].SetFace((int)sequence.face);
                jocker[side].SetItem((int)sequence.item);
                AudioManager.PlaySound(GlobalFactory.Cinematics.soundJocker);
                break;
        }

        dialogLabel.SetText(sequence.text);
    }

    //////////////////////////////////////////////////////
    /// STATIC MEMBERS
    //////////////////////////////////////////////////////
    [Preserve, Console("test", "cinema")]
    public static void TestCinema(int index)
    {
        if (index < GlobalFactory.Cinematics.All.Count)
        {
            Game.Instance.OpenPopup<Popup_Cinematic>().Setup(GlobalFactory.Cinematics.All[index], () => TestCinema(index + 1));
        }
    }
}
