using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public static Vector3 SpawnPoint = Vector3.zero;
    public static Ball mainBall = null;
    public static List<Ball> balls = new List<Ball>(128);

    private void Awake()
    {
        SpawnPoint.y -= BlockManager.OriginY + GlobalFactory.Balls.GetPrefab(0).transform.localScale.y * 0.5f;
        mainBall = null;
        balls.Clear();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.rigidbody.velocity = Vector3.zero;
        collision.transform.position = new Vector2(collision.contacts[0].point.x, SpawnPoint.y);
        if (collision.transform == mainBall.transform)
            SpawnPoint.x = collision.contacts[0].point.x;

        bool turnOver = true;
        for (int i = 0; i < balls.Count; i++)
            if (balls[i].Rigidbody.velocity.sqrMagnitude > 1)
                turnOver = false;

        if (turnOver)
            transform.root.Broadcast(Messages.Type.EndTurn);
    }

    public Ball SpawnBall()
    {
        var ball = GlobalFactory.Balls.GetPrefab(PlayModel.ballId).Clone<Ball>(transform);
        if (mainBall == null)
            mainBall = ball;
        balls.Add(ball);
        return ball;
    }

    private void OnMessage(Messages.Param param)
    {
        switch (param.type)
        {
            case Messages.Type.StartTurn:
                {
                    StartCoroutine(DoStartTurn(param.As<Vector3>().normalized * 10));
                    transform.root.Broadcast(Messages.Type.TurnStarted, this);
                }
                break;
            case Messages.Type.EndTurn:
                {
                    StopAllCoroutines();
                    transform.root.Broadcast(Messages.Type.TurnEnded, this);
                }
                break;
        }
    }

    private IEnumerator DoStartTurn(Vector2 direction)
    {
        foreach (var ball in balls)
        {
            ball.GetComponent<Rigidbody2D>().velocity = direction;
            yield return new WaitForSeconds(0.1f);
        }
    }


#if UNITY_EDITOR
    private void Start()
    {
        int i = 0;
        while (i++ < 10)
        {
            var item = SpawnBall();
            item.transform.localPosition = SpawnPoint;
        }
    }
#endif
}
