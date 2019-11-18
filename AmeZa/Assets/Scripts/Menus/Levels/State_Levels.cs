using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State_Levels : GameState
{
    [SerializeField] private GameObject chestObject = null;
    [SerializeField] private Button claimRewardButton = null;
    [SerializeField] private UiLevelItem levelItem = null;
    [SerializeField] private Button nextButton = null;
    [SerializeField] private Button prevButton = null;

    private RectTransform content = null;

    private void Awake()
    {
        content = levelItem.transform.parent.AsRectTransform();
    }

    private void Start()
    {
        UiShowHide.ShowAll(transform);
    }

    private void UpdateVisual()
    {

    }

    private void DisplayLevels()
    {

    }
}
