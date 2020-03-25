using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockObstacle : BlockBase
{
    [SerializeField] private SpriteRenderer spriter = null;
    [SerializeField] private Animation animator = null;

    public override BlockType Type { get { return BlockType.Obstacle; } }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (animator.isPlaying == false) animator.Play("BlockHit");
        transform.root.Message(Messages.Type.OnBlockHit, this);
    }

    public override void GoDown(int step)
    {
        if (Position.y > BlockManager.BottomEdge)
        {
            base.GoDown(step);
        }
        else
        {
            Invoke("DeleteMe", 0.1f);
        }
    }

    public void DeleteMe()
    {
        spriter.sortingOrder++;
        Destroy(GetComponent<Collider2D>());
        animator.Play("BlockDeath");
        transform.root.Message(Messages.Type.OnBlockDeath, this);
        transform.root.Broadcast(Messages.Type.BlockDead, this);
        Destroy(gameObject, 3);
    }
}
