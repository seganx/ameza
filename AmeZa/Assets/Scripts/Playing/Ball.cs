using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        switch (param.type)
        {
            case Messages.Type.EndTurn:
                var speed = rigid.velocity.magnitude;
                if (speed > 0.1f)
                {
                    var dir = BallManager.SpawnPoint - transform.localPosition + Vector3.down * 0.1f;
                    rigid.velocity = dir.normalized * speed;
                }
                break;

            case Messages.Type.TurnEnded:
                {
                    rigid.velocity = Vector2.zero;
                    StopAllCoroutines();
                    if (Vector3.Distance(transform.localPosition, BallManager.SpawnPoint) > 0.05f)
                        StartCoroutine(MoveToMain(param.As<BallManager>()));
                }
                break;
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
