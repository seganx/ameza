using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeSounds : MonoBehaviour
{
    [SerializeField] private AudioSource[] hitSounds = null;
    [SerializeField] private AudioSource[] breakSounds = null;
    [SerializeField] private AudioSource[] releaseBallSounds = null;

    private void OnMessage(Messages.Param param)
    {
        switch (param.type)
        {
            case Messages.Type.OnBlockHit:
                {
                    if (param.Is<BlockSimple>()) PlayHit();
                }
                break;

            case Messages.Type.OnBlockDeath:
                {
                    transform.SetParent(null);
                    Destroy(gameObject, 2);
                    if (param.Is<BlockSimple>()) PlayBreak();
                    else if (param.Is<BlockBall>()) PlayReleaseBall();
                }
                break;
        }
    }

    private void PlayHit()
    {
        var index = Random.Range(0, 1000);
        hitSounds[index % hitSounds.Length].Play();
    }

    private void PlayBreak()
    {
        var index = Random.Range(0, 1000);
        breakSounds[index % breakSounds.Length].Play();
    }

    private void PlayReleaseBall()
    {
        var index = Random.Range(0, 1000);
        releaseBallSounds[index % releaseBallSounds.Length].Play();
    }
}
