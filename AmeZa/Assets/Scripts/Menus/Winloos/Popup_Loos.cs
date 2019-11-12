using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_Loos : GameState
{
    private System.Action nextTaskFunc = null;

    // Use this for initialization
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

    }

    public Popup_Loos SetNextTask(System.Action nextTask)
    {
        nextTaskFunc = nextTask;
        return this;
    }

    public override void Back()
    {
        base.Back();
        if (nextTaskFunc != null)
            nextTaskFunc();
    }
}
