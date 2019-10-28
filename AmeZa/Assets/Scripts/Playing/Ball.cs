using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeganX;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rigid = null;

    public Rigidbody2D Rigidbody { get { return rigid; } }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void OnMessage(Messages.Param param)
    {
        if (param.Is(Messages.Type.TurnEnded))
        {
            rigid.velocity = Vector2.zero;
            StopAllCoroutines();
            StartCoroutine(MoveToMain(param.As<BallManager>()));
        }
    }

    private IEnumerator MoveToMain(BallManager sender)
    {
        float time = 0;
        var wait = new WaitForEndOfFrame();
        var startPosit = transform.localPosition;
        while (time < 1)
        {
            time += Time.deltaTime * 4;
            transform.localPosition = Vector3.Lerp(startPosit, BallManager.SpawnPoint, time);
            yield return wait;
        }
    }
}
