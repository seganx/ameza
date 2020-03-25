using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_SelectLeague : GameState
{
    private void Start()
    {
        UIBackground.Show();
        if (Profile.IsLoggedIn)
        {
            UiShowHide.ShowAll(transform);
        }
        else
        {
            Loading.Show();
            Profile.Sync(false, success =>
            {
                Loading.Hide();
                if (success)
                {
                    UiShowHide.ShowAll(transform);
                }
                else
                {
                    Back();
                }
            });
        }

    }
}
