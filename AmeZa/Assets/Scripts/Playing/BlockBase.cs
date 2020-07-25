using System.Collections;
using UnityEngine;

public class BlockBase : MonoBehaviour
{
    public virtual BlockType Type => BlockType.Null;

    public bool IsDangerous => Type != BlockType.Obstacle && Type != BlockType.Ball && Type != BlockType.Null;

    private Vector3 position = Vector3.zero;

    public Vector2Int Position
    {
        get
        {
            return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        }
    }

    public BlockBase SetPosition(int x, int y)
    {
        position = transform.localPosition = new Vector3(x, y, 0);
        return this;
    }


    public virtual void UsedAbility(AbilityType ability)
    {
        // overide this to implement abilities
    }

    public virtual void GoDown(int step = 1)
    {
        position.y -= step;
        StartCoroutine(DoGoDown());
    }

    private IEnumerator DoGoDown()
    {
        var startPos = transform.localPosition;
        var wait = new WaitForEndOfFrame();
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * 2;
            transform.localPosition = Vector3.LerpUnclamped(startPos, position, GlobalFactory.Blocks.MoveDownCurve.Evaluate(time));
            yield return wait;
        }
    }
}
