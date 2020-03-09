using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBall : BlockBase
{
    [SerializeField] private SpriteRenderer ballSpriter = null;
    [SerializeField] private SpriteRenderer blockSpriter = null;

    public bool IsPregnant { get; private set; }
    public override BlockType Type { get { return BlockType.Ball; } }

    private bool released = false;

    private void Start()
    {
        IsPregnant = true;
        ballSpriter.sprite = GlobalFactory.Balls.GetSprite(PlayModel.ballId);
        blockSpriter.color = ballSpriter.sprite.GetAverageColor();
    }

    private void Update()
    {
        blockSpriter.transform.Rotate(0, 0, 90 * Time.deltaTime);
    }

    private void Hit(bool pregnant)
    {
        released = true;
        IsPregnant = pregnant;
        PlayModel.stats.totalBalls++;
        Destroy(GetComponent<BoxCollider2D>());
        transform.Broadcast(Messages.Type.OnBlockDeath, this);
        transform.root.Broadcast(Messages.Type.BlockDead, this);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Hit(true);
    }

    public override void GoDown(int step)
    {
        if (released) return;

        if (Position.y > BlockManager.BottomEdge)
            base.GoDown(step);
        else
            Hit(false);
    }

    public override void UsedAbility(AbilityType ability)
    {
        switch (ability)
        {
            case AbilityType.Bomb: Hit(false); break;
            case AbilityType.Missle: if (Position.y < (BlockManager.BottomEdge + 3)) Hit(false); break;
        }
    }
}
