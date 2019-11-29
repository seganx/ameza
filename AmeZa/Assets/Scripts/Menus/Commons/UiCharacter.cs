using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SeganX;

public class UiCharacter : MonoBehaviour
{
    [SerializeField] private GameObject eyes = null;
    [SerializeField] private SimpleAnimation hand = null;

    private IEnumerator Start()
    {
        eyes.SetActive(false);

        var wait = new WaitForSeconds(2);
        while (true)
        {
            if (Random.Range(0, 100) < 60)
                StartCoroutine(BlinkEyes());

            if (Random.Range(0, 100) < 60)
                hand.PlayRandom(0, SimpleAnimation.Mode.DiscardNext);

            yield return wait;
        }
    }

    private IEnumerator BlinkEyes()
    {
        eyes.SetActive(true);
        yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
        eyes.SetActive(false);
    }
}
