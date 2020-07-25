using SeganX;
using UnityEngine;

public class BlockWarning : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriter = null;

    private void Update()
    {
        var color = spriter.color;
        color.a = Mathf.Sin(Time.time * 5) * 0.5f + 0.5f;
        spriter.color = color;
    }

    private void Reset()
    {
        if (spriter == null)
            spriter = transform.GetComponent<SpriteRenderer>(true, true);
    }
}
