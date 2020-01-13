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
                    if (param.Is<BlockValue>()) PlayHit();
                }
                break;

            case Messages.Type.OnBlockDeath:
                {
                    transform.SetParent(null);
                    Destroy(gameObject, 2);
                    if (param.Is<BlockValue>()) PlayBreak();
                    else if (param.Is<BlockBall>()) PlayReleaseBall();
                }
                break;
        }
    }

    private void PlayHit()
    {
        var index = Random.Range(0, 1000);
        var source = hitSounds[index % hitSounds.Length];
        source.PlayOneShot(source.clip, 0.75f);
        //source.Play();
    }

    private void PlayBreak()
    {
        var index = Random.Range(0, 1000);
        var source = breakSounds[index % breakSounds.Length];
        //source.PlayOneShot(source.clip);
        source.Play();
    }

    private void PlayReleaseBall()
    {
        var index = Random.Range(0, 1000);
        var source = releaseBallSounds[index % releaseBallSounds.Length];
        //source.PlayOneShot(source.clip);
        source.Play();
    }
}
