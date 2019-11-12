using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_Shop : GameState
{
    private System.Action onCloseFunc = null;

    public Popup_Shop SetOnClose(System.Action onClose)
    {
        onCloseFunc = onClose;
        return this;
    }

    public override void Back()
    {
        base.Back();
        if (onCloseFunc != null)
            onCloseFunc();
    }

    // Use this for initialization
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

    }
}
