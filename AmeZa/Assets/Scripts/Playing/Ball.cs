using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Collider2D Collider { get; private set; } = null;
    public Rigidbody2D Rigidbody { get; private set; } = null;

    private void Awake()
    {
        Collider = GetComponent<Collider2D>();
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnMessage(Messages.Param param)
    {
        switch (param.type)
        {
            case Messages.Type.EndTurn:
                var speed = Rigidbody.velocity.magnitude;
                if (speed > 0.1f)
                {
                    //var dir = BallManager.SpawnPoint - transform.localPosition + new Vector3(Random.Range(-1f, 1f), -1, 0);
                    var dir = new Vector3(Random.Range(-1f, 1f), -3, 0);
                    Rigidbody.velocity = dir.normalized * speed;
                }
                break;

            case Messages.Type.TurnEnded:
                {
                    Rigidbody.velocity = Vector2.zero;
                    Collider.enabled = false;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var contact = collision.GetContact(0);
        var dotValue = Vector3.Dot(contact.relativeVelocity.normalized, Vector3.right) * 10;
        if (Mathf.Abs(dotValue) > 9.995f)
        {
            Rigidbody.velocity += Vector2.down;
        }
    }
}
