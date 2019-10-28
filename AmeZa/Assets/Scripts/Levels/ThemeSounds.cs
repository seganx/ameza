using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeSounds : MonoBehaviour
{
    [SerializeField] private AudioSource[] hits = null;
    [SerializeField] private AudioSource[] breaks = null;
    
    private void OnMessage(Messages.Param data)
    {
        switch (data.type)
        {
            case Messages.Type.BlockHit:
                {
                    var index = Random.Range(0, 1000);
                    hits[index % hits.Length].Play();
                }
                break;

            case Messages.Type.BlockDeath:
                {
                    transform.SetParent(null);
                    var index = Random.Range(0, 1000);
                    breaks[index % breaks.Length].Play();
                    Destroy(gameObject, 2);
                }
                break;
        }
    }
}
