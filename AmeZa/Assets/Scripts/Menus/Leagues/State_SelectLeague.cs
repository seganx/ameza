using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_SelectLeague : GameState
{
    private void Start()
    {
        UIBackground.Show();
        UiShowHide.ShowAll(transform);
    }
}
