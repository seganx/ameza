using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBall : MonoBehaviour
{
    public BlockBall Setup(int x, int y, int ballId)
    {
        transform.localPosition = new Vector3(x, y, 0);
        var item = GlobalFactory.Balls.GetPrefab(ballId).Clone<Transform>(transform);
        Destroy(item.GetComponent<Ball>());
        Destroy(item.GetComponent<CircleCollider2D>());
        Destroy(item.GetComponent<Rigidbody2D>());
        item.localPosition = Vector3.zero;
        return this;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(GetComponent<BoxCollider2D>());
        StartCoroutine(MoveDown());
    }

    private IEnumerator MoveDown()
    {
        var wait = new WaitForEndOfFrame();
        var target = new Vector3(transform.localPosition.x, BallManager.SpawnPoint.y, -0.2f);
        while (Mathf.Approximately(transform.localPosition.y, target.y) == false)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, Time.deltaTime);
            yield return wait;
        }
    }

    private void OnMessage(Messages.Param param)
    {
        if (param.Is(Messages.Type.TurnEnded))
        {
            StopAllCoroutines();
            StartCoroutine(MoveToMain(param.As<BallManager>()));
        }
    }

    private IEnumerator MoveToMain(BallManager sender)
    {
        float time = 0;
        var wait = new WaitForEndOfFrame();
        var startPosit = transform.localPosition;
        var startScale = transform.localPosition;
        while (time < 1)
        {
            time += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(startPosit, BallManager.SpawnPoint, time);
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, time);
            yield return wait;
        }
        sender.SpawnBall();
    }

}
