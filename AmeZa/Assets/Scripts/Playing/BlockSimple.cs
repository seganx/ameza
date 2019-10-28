using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSimple : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriter = null;
    [SerializeField] private SpriteRenderer effector = null;
    [SerializeField] private Animation animator = null;
    [SerializeField] private TextMesh numberLabel = null;

    public int ItemIndex { get; set; }
    public int Health { get; set; }

    private void Awake()
    {
        Health = 100;
        effector.gameObject.SetActive(false);
        numberLabel.text = Health.ToString().Persian();
        GlobalFactory.Theme.GetSounds(PlayModel.level.theme).Clone<ThemeSounds>(transform);
    }

    public BlockSimple Setup(int x, int y, int itemIndex, int health)
    {
        Health = health;
        ItemIndex = itemIndex % GlobalFactory.Theme.GetSpriteCount(PlayModel.level.theme);
        transform.localPosition = new Vector3(x, y, 0);
        spriter.sprite = effector.sprite = GlobalFactory.Theme.GetSprite(PlayModel.level.theme, ItemIndex);
        numberLabel.text = Health.ToString().Persian();
        return this;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Health > 1)
        {
            Health--;
            numberLabel.text = Health.ToString().Persian();
            if (animator.isPlaying == false) animator.Play("BlockHit");

            transform.root.Broadcast(Messages.Type.BlockHit, this);
        }
        else
        {
            PlayModel.stats.totalBlocks++;
            spriter.sortingOrder++;
            Destroy(GetComponent<Collider2D>());
            Destroy(effector.gameObject);
            Destroy(numberLabel.gameObject);
            if (ItemIndex < 2)
            {
                if (ItemIndex == 0) PlayModel.stats.totalItem0++;
                if (ItemIndex == 1) PlayModel.stats.totalItem1++;
            }
            else animator.Play("BlockDeath");
            Destroy(gameObject, 3);

            transform.root.Broadcast(Messages.Type.BlockDeath, this);
        }
    }

}
