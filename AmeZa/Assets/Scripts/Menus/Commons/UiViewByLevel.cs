using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class UiViewByLevel : MonoBehaviour
{
    [SerializeField] private int level = 0;
    [SerializeField] private int session = 0;

    private void Awake()
    {
        if (Profile.Sessions < session || Profile.GetLevelsPassed() < level)
            gameObject.SetActive(false);
    }
}
