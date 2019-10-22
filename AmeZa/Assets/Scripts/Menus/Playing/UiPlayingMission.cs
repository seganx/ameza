using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiPlayingMission : MonoBehaviour
{
    [SerializeField] private LocalText blocksLabel = null;
    [SerializeField] private LocalText ballsLabel = null;
    [SerializeField] private Image item0Image = null;
    [SerializeField] private LocalText item0Label = null;
    [SerializeField] private Image item1Image = null;
    [SerializeField] private LocalText item1Label = null;

    private IEnumerator Start()
    {
        PlayModel.level.targetBlocks = Random.Range(1, 10);
        PlayModel.level.targetBalls = Random.Range(1, 10);
        PlayModel.level.targetItem0 = Random.Range(1, 10);
        PlayModel.level.targetItem1 = Random.Range(1, 10);

        blocksLabel.transform.parent.gameObject.SetActive(PlayModel.level.targetBlocks > 0);
        ballsLabel.transform.parent.gameObject.SetActive(PlayModel.level.targetBalls > 0);

        if (PlayModel.level.targetItem0 > 0)
        {
            item0Image.gameObject.SetActive(true);
            item0Image.sprite = GlobalFactory.Theme.GetSprite(PlayModel.level.theme, 0);
        }
        else item0Image.gameObject.SetActive(false);

        if (PlayModel.level.targetItem1 > 0)
        {
            item1Image.gameObject.SetActive(true);
            item1Image.sprite = GlobalFactory.Theme.GetSprite(PlayModel.level.theme, 1);
        }
        else item1Image.gameObject.SetActive(false);

        var wait = new WaitForSeconds(0.1f);
        while (true)
        {
            if (PlayModel.level.targetBlocks > 0)
            {
                var diff = PlayModel.level.targetBlocks - PlayModel.stats.totalBlocks;
                if (diff >= 0) blocksLabel.SetText(diff.ToString());
            }

            if (PlayModel.level.targetBalls > 0)
            {
                var diff = PlayModel.level.targetBalls - PlayModel.stats.totalBalls;
                if (diff >= 0) ballsLabel.SetText(diff.ToString());
            }

            if (PlayModel.level.targetItem0 > 0)
            {
                var diff = PlayModel.level.targetItem0 - PlayModel.stats.totalItem0;
                if (diff >= 0) item0Label.SetText(diff.ToString());
            }

            if (PlayModel.level.targetItem1 > 0)
            {
                var diff = PlayModel.level.targetItem1 - PlayModel.stats.totalItem1;
                if (diff >= 0) item1Label.SetText(diff.ToString());
            }

            yield return wait;
        }
    }
}
