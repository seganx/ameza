﻿using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockValue : BlockBase
{
    [SerializeField] private SpriteRenderer spriter = null;
    [SerializeField] private SpriteRenderer effector = null;
    [SerializeField] private Animation animator = null;
    [SerializeField] private TextMesh numberLabel = null;

    public int ItemIndex { get; set; }
    public int Health { get; set; }

    public override BlockType Type { get { return BlockType.Value; } }

    private void Awake()
    {
        effector.gameObject.SetActive(false);
    }

    public BlockValue Setup(int itemIndex, int health)
    {
        Health = health;
        ItemIndex = itemIndex % GlobalFactory.Theme.GetSpriteCount(PlayModel.level.theme);
        spriter.sprite = effector.sprite = GlobalFactory.Theme.GetSprite(PlayModel.level.theme, ItemIndex);
        numberLabel.text = Health.ToString().Persian();
        return this;
    }

    public void Hit(int damage)
    {
        if (Health > damage)
        {
            Health -= damage;
            numberLabel.text = Health.ToString().Persian();
            if (animator.isPlaying == false) animator.Play("BlockHit");
            transform.Broadcast(Messages.Type.OnBlockHit, this);
        }
        else
        {
            PlayModel.stats.totalBlocks++;
            spriter.sortingOrder++;
            Destroy(GetComponent<Collider2D>());
            Destroy(effector.gameObject);
            Destroy(numberLabel.gameObject);

            if (PlayModel.level.targetItem0 > 0 && ItemIndex == 0)
                PlayModel.stats.totalItem0++;
            else if (PlayModel.level.targetItem1 > 0 && ItemIndex == 1)
                PlayModel.stats.totalItem1++;
            else
                animator.Play("BlockDeath");
            Destroy(gameObject, 3);

            transform.Broadcast(Messages.Type.OnBlockDeath, this);
            transform.root.Broadcast(Messages.Type.BlockDead, this);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Hit(1);
    }

    public override void UsedAbility(AbilityType ability)
    {
        switch (ability)
        {
            case AbilityType.Hammer: Hit(1); break;
            case AbilityType.Bomb: Hit(100); break;
            case AbilityType.Missle: if (Position.y < -(BlockManager.OriginY - 3)) Hit(999999); break;
        }
    }
}