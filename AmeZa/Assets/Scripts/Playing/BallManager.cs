using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    public static Vector3 SpawnPoint = Vector3.zero;
    public static Ball mainBall = null;
    public static List<Ball> balls = new List<Ball>(128);

    private static List<Ball> tmp = new List<Ball>(128);
    private Ball ballPrefab = null;
    private bool turnStarted = false;
    private bool ballsShooting = false;

    private void Start()
    {
        ballPrefab = GlobalFactory.Balls.GetPrefab(PlayModel.ballId);
        SpawnPoint.x = 0;
        SpawnPoint.y = -BlockManager.TopEdge - ballPrefab.transform.localScale.y * 0.5f;
        mainBall = null;
        balls.Clear();

        for (int i = 0; i < PlayModel.level.startBallCount; i++)
            SpawnBall(SpawnPoint);

        transform.root.Broadcast(Messages.Type.BallCount, PlayModel.level.startBallCount);
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
        float contactX = (collision.contacts.Length < 1) ? collision.transform.position.x : collision.contacts[0].point.x;
        collision.rigidbody.velocity = Vector3.zero;
        collision.transform.position = new Vector2(contactX, SpawnPoint.y);
        collision.rigidbody.Sleep();

        // update spawn point
        if (collision.transform == mainBall.transform)
            SpawnPoint.x = contactX;

        // check balls for turn over
        if (turnStarted && ballsShooting == false)
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
                    ballsShooting = false;
                }
                break;
            case Messages.Type.BlockDead:
                {
                    if (param.Is<BlockBall>())
                    {
                        var newball = SpawnBall(param.As<BlockBall>().transform.localPosition);
                        newball.Rigidbody.velocity = PlayModel.level.startBallSpeed * new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
                    }
                }
                break;
        }
    }

    private IEnumerator DoStartTurn(Vector2 direction)
    {
        ballsShooting = true;
        tmp.Clear();
        tmp.AddRange(balls);
        for (int i = 0; i < tmp.Count; i++)
        {
            yield return new WaitForSeconds(0.1f);
            tmp[i].Rigidbody.velocity = direction;
            transform.root.Broadcast(Messages.Type.BallCount, tmp.Count - i - 1);
        }
        ballsShooting = false;
    }
}
