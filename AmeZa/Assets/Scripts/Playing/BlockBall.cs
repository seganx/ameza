using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBall : BlockBase
{
    [SerializeField] private SpriteRenderer ballSpriter = null;
    [SerializeField] private SpriteRenderer blockSpriter = null;

    public override BlockType Type { get { return BlockType.Ball; } }

    private bool released = false;

    private void Start()
    {
        ballSpriter.sprite = GlobalFactory.Balls.GetSprite(PlayModel.ballId);
        blockSpriter.color = ballSpriter.sprite.GetAverageColor();
    }

    private void Update()
    {
        blockSpriter.transform.Rotate(0, 0, 90 * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        released = true;
        PlayModel.stats.totalBalls++;
        Destroy(GetComponent<BoxCollider2D>());
        transform.Broadcast(Messages.Type.OnBlockDeath, this);
        transform.root.Broadcast(Messages.Type.BlockDead, this);
        Destroy(gameObject);
    }

    public override void GoDown(int step)
    {
        if (released) return;

        var edge = -BlockManager.OriginY + 1;
        if (Position.y > edge)
            base.GoDown(step);
        else
            Destroy(gameObject);
    }
}
