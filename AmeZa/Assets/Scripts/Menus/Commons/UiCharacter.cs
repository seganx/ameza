using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiCharacter : MonoBehaviour
{
    [SerializeField] private GameObject eyes = null;
    [SerializeField] private SimpleAnimation hand = null;

    private IEnumerator Start()
    {
        eyes.SetActive(false);

        while (true)
        {
            if (Random.Range(0, 100) < 60)
                StartCoroutine(BlinkEyes());

            hand.PlayRandom(0, SimpleAnimation.Mode.DiscardNext);

            yield return new WaitForSecondsRealtime(2);
        }
    }

    private IEnumerator BlinkEyes()
    {
        eyes.SetActive(true);
        yield return new WaitForSecondsRealtime(Random.Range(0.2f, 0.5f));
        eyes.SetActive(false);
    }
}
