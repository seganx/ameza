using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiCharacter : MonoBehaviour
{
    [SerializeField] private Transform bodies = null;
    [SerializeField] private Transform heads = null;
    [SerializeField] private GameObject eyes = null;
    [SerializeField] private SimpleAnimation hand = null;
    [SerializeField] private int defaultBody = 0;
    [SerializeField] private int defaultHead = 1;

    public UiCharacter SetBody(int index)
    {
        if (index.Between(0, bodies.childCount - 1))
            bodies.SetActiveChild(index);
        return this;
    }

    public UiCharacter SetFace(int index)
    {
        if (index.Between(0, heads.childCount - 1))
            heads.SetActiveChild(index);
        return this;
    }

    private IEnumerator ShakeHand()
    {
        while (true)
        {
            hand.PlayRandom(0);
            yield return new WaitForSecondsRealtime(2);
        }
    }

    private IEnumerator BlinkEyes()
    {
        while (true)
        {
            if (Random.Range(0, 100) < 60)
            {
                eyes.SetActive(true);
                yield return new WaitForSecondsRealtime(Random.Range(0.2f, 0.5f));
                eyes.SetActive(false);
            }

            yield return new WaitForSecondsRealtime(2);
        }
    }


    private void OnEnable()
    {
        bodies.SetActiveChild(defaultBody);
        heads.SetActiveChild(defaultHead);
        eyes.SetActive(false);

        if (hand) StartCoroutine(ShakeHand());
        StartCoroutine(BlinkEyes());
    }
}
