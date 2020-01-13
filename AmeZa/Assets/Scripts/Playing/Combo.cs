using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combo : Base
{
    private bool started = false;
    private int count = 0;
    private long countTime = 0;

    private void Awake()
    {
        transform.SetActiveChild(-1);
    }

    private void OnMessage(Messages.Param param)
    {
        switch (param.type)
        {
            case Messages.Type.TurnStarted:
                started = true;
                count = 0;
                break;
            case Messages.Type.TurnEnded:
                started = false;
                count = 0;
                break;
            case Messages.Type.BlockDead:
                if (started)
                {
                    count++;

                    var ticks = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
                    var delta = ticks - countTime;
                    if (delta > 200)
                    {
                        countTime = ticks;
                        CheckCount(param);                        
                    }
                }
                break;
        }
    }

    private void CheckCount(Messages.Param param)
    {
        switch (count % 17)
        {
            case 03: if (Random.Range(0, 100) < 70) Display(0, param); break;
            case 04: if (Random.Range(0, 100) < 70) Display(1, param); break;
            case 05: if (Random.Range(0, 100) < 70) Display(2, param); break;
            case 06: if (Random.Range(0, 100) < 70) Display(3, param); break;
            case 07: if (Random.Range(0, 100) < 70) Display(4, param); break;
            case 08: if (Random.Range(0, 100) < 80) Display(5, param); break;
            case 10: if (Random.Range(0, 100) < 90) Display(6, param); break;
            case 12: if (Random.Range(0, 100) < 90) Display(7, param); break;
            case 14: if (Random.Range(0, 100) < 90) Display(8, param); break;
            case 16: if (Random.Range(0, 100) < 80) Display(6, param); break;
        }
    }

    private void Display(int index, Messages.Param param)
    {
        var child = transform.GetChild(index);
        if (child == null) return;
        var pos = param.As<BlockBase>().transform.position + Vector3.back;
        pos.x *= 0.7f;
        child.position = pos;
        child.gameObject.SetActive(true);
        var particle = child.GetComponent<ParticleSystem>();
        particle.Simulate(0, true, true);
        particle.Play(true);
    }
}
