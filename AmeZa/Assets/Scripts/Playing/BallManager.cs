using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public static Vector3 SpawnPoint = Vector3.zero;
    public static Ball mainBall = null;
    public static List<Ball> balls = new List<Ball>(128);

    private Ball ballPrefab = null;
    private bool turnStarted = false;

    private void Start()
    {
        ballPrefab = GlobalFactory.Balls.GetPrefab(PlayModel.ballId);
        SpawnPoint.x = 0;
        SpawnPoint.y = -BlockManager.OriginY - ballPrefab.transform.localScale.y * 0.5f;
        mainBall = null;
        balls.Clear();

        for (int i = 0; i < PlayModel.level.startBallCount; i++)
            SpawnBall(SpawnPoint);
    }

    public Ball SpawnBall(Vector3 position)
    {
        ballPrefab.transform.localPosition = position;
        var ball = ballPrefab.Clone<Ball>(transform);
        if (mainBall == null)
            mainBall = ball;
        balls.Add(ball);
        return ball;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts.Length < 1) return;

        collision.rigidbody.velocity = Vector3.zero;
        collision.transform.position = new Vector2(collision.contacts[0].point.x, SpawnPoint.y);
        if (collision.transform == mainBall.transform)
            SpawnPoint.x = collision.contacts[0].point.x;
        collision.rigidbody.Sleep();

        if (turnStarted)
        {
            bool turnOver = true;
            for (int i = 0; i < balls.Count; i++)
                if (balls[i].Rigidbody.velocity.sqrMagnitude > 0.05f)
                    turnOver = false;

            if (turnOver)
            {
                turnStarted = false;
                transform.root.Broadcast(Messages.Type.TurnEnded, this);
            }
        }
    }


    private void OnMessage(Messages.Param param)
    {
        switch (param.type)
        {
            case Messages.Type.StartTurn:
                {
                    turnStarted = true;
                    StartCoroutine(DoStartTurn(param.As<Vector3>().normalized * PlayModel.level.startBallSpeed));
                    transform.root.Broadcast(Messages.Type.TurnStarted, this);
                }
                break;
            case Messages.Type.EndTurn:
                {
                    StopAllCoroutines();
                }
                break;
            case Messages.Type.BlockDead:
                {
                    if (param.Is<BlockBall>())
                    {
                        var newball = SpawnBall(param.As<BlockBall>().transform.localPosition);
                        newball.Rigidbody.velocity = Vector2.down * PlayModel.level.startBallSpeed;
                    }
                }
                break;
        }
    }

    private IEnumerator DoStartTurn(Vector2 direction)
    {
        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].Rigidbody.velocity = direction;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
