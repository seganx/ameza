using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockValue : BlockBase
{
    [SerializeField] private SpriteRenderer spriter = null;
    [SerializeField] private SpriteRenderer effector = null;
    [SerializeField] private Animation animator = null;
    [SerializeField] protected TextMesh numberLabel = null;
    [SerializeField] private BlockWarning warning = null;

    public int ItemIndex { get; set; }
    public int Health { get; set; }

    public override BlockType Type { get { return BlockType.Value; } }

    private void Awake()
    {
        effector.gameObject.SetActive(false);
        warning.gameObject.SetActive(false);
    }

    public BlockValue Setup(int itemIndex, int health)
    {
        Health = Mathf.Max(health, 1);
        ItemIndex = itemIndex % GlobalFactory.Theme.Selected.items.Length;
        spriter.sprite = effector.sprite = GlobalFactory.Theme.Selected.items[ItemIndex];
        numberLabel.text = Health.ToString().Persian();
        return this;
    }

    public virtual void Hit(int damage)
    {
        if (Health > damage)
        {
            Health -= damage;
            numberLabel.text = Health.ToString().Persian();
            if (animator.isPlaying == false) animator.Play("BlockHit");
            transform.root.Message(Messages.Type.OnBlockHit, this);
        }
        else
        {
            PlayModel.result.totalBlocks++;
            spriter.sortingOrder++;
            Destroy(GetComponent<Collider2D>());
            Destroy(warning.gameObject);
            Destroy(effector.gameObject);
            Destroy(numberLabel.gameObject);

            if (PlayModel.level.targetItem0 > 0 && ItemIndex == 0)
                PlayModel.result.totalItem0++;
            else if (PlayModel.level.targetItem1 > 0 && ItemIndex == 1)
                PlayModel.result.totalItem1++;
            else
                animator.Play("BlockDeath");
            Destroy(gameObject, 3);

            transform.root.Message(Messages.Type.OnBlockDeath, this);
            transform.root.Broadcast(Messages.Type.BlockDead, this);
        }
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        Hit(1);
    }

    public override void UsedAbility(AbilityType ability)
    {
        switch (ability)
        {
            case AbilityType.Hammer: Hit(10); break;
            case AbilityType.Bomb: Hit(999999); break;
            case AbilityType.Missle: if (Position.y < (BlockManager.BottomEdge + 3)) Hit(999999); break;
        }
    }

    public override void GoDown(int step = 1)
    {
        base.GoDown(step);
        if (Position.y < BlockManager.BottomEdge + 1)
        {
            warning.gameObject.SetActive(true);
        }
    }
}
