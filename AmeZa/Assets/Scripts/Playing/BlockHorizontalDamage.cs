using SeganX;
using UnityEngine;

public class BlockHorizontalDamage : BlockValue
{
    [SerializeField] private ParticleSystem effects = null;

    private RaycastHit2D[] results = new RaycastHit2D[20];

    public override BlockType Type { get { return BlockType.HorizontalDamage; } }


    public BlockHorizontalDamage Setup(int health)
    {
        ItemIndex = -1;
        Health = Mathf.Max(health, 1);
        numberLabel.text = Health.ToString().Persian();
        return this;
    }

    public override void Hit(int damage)
    {
        if (Health >= damage)
        {
            int count = Physics2D.LinecastNonAlloc(transform.position + Vector3.left * 10, transform.position + Vector3.right * 10, results, 1 << 11);
            for (int i = 0; i < count; i++)
            {
                var com = results[i].transform.GetComponent<BlockValue>();
                if (com != null && com.Type == BlockType.Value)
                    com.Hit(1);
            }

            if (effects.isPlaying == false)
                effects.Play(true);
        }

        base.Hit(damage);
    }
}
