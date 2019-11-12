using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiPlayingMission : MonoBehaviour
{
    [SerializeField] private LocalText levelNumberLabel = null;
    [SerializeField] private LocalText blocksLabel = null;
    [SerializeField] private Image ballsImage = null;
    [SerializeField] private LocalText ballsLabel = null;
    [SerializeField] private Image item0Image = null;
    [SerializeField] private LocalText item0Label = null;
    [SerializeField] private Image item1Image = null;
    [SerializeField] private LocalText item1Label = null;
    [SerializeField] private LocalText scoreBallsLabel = null;
    [SerializeField] private LocalText scoreBlocksLabel = null;
    [SerializeField] private AnimationCurve itemMoveCurve = null;
    [SerializeField] private AnimationCurve itemScaleCurve = null;

    private IEnumerator Start()
    {
        blocksLabel.transform.parent.gameObject.SetActive(PlayModel.level.targetBlocks > 0);
        scoreBallsLabel.transform.parent.gameObject.SetActive(PlayModel.type == PlayModel.Type.LeagueBalls);
        scoreBlocksLabel.transform.parent.gameObject.SetActive(PlayModel.type == PlayModel.Type.LeagueBlocks);

        if (PlayModel.type == PlayModel.Type.Levels)
        {
            levelNumberLabel.transform.parent.gameObject.SetActive(true);
            levelNumberLabel.SetFormatedText(PlayModel.level.index + 1);
        }
        else levelNumberLabel.transform.parent.gameObject.SetActive(false);

        if (PlayModel.level.targetBalls > 0)
        {
            ballsImage.transform.parent.gameObject.SetActive(true);
            ballsImage.sprite = GlobalFactory.Balls.GetSprite(PlayModel.ballId);
        }
        else ballsImage.transform.parent.gameObject.SetActive(false);

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

            if (PlayModel.type == PlayModel.Type.LeagueBalls)
                scoreBallsLabel.SetFormatedText(PlayModel.stats.totalBalls);

            if (PlayModel.type == PlayModel.Type.LeagueBlocks)
                scoreBlocksLabel.SetFormatedText(PlayModel.stats.totalBlocks);

            yield return wait;
        }
    }

    private void OnMessage(Messages.Param param)
    {
        if (PlayModel.level.targetItem0 < 1 && PlayModel.level.targetItem1 < 1) return;

        if (param.Is(Messages.Type.BlockDead) && param.Is<BlockSimple>())
        {
            var subject = param.As<BlockSimple>();
            if (PlayModel.level.targetItem0 > 0 && subject.ItemIndex == 0)
                StartCoroutine(DoMoveToMissionUi(subject.transform, item0Image.transform.position));
            else if (PlayModel.level.targetItem1 > 0 && subject.ItemIndex == 1)
                StartCoroutine(DoMoveToMissionUi(subject.transform, item1Image.transform.position));
        }
    }

    private IEnumerator DoMoveToMissionUi(Transform subject, Vector3 dest)
    {
        float time = 0;
        var wait = new WaitForEndOfFrame();
        var startPosit = subject.transform.position;
        var startScale = subject.transform.localScale;
        while (time < 1)
        {
            time += Time.deltaTime * 2;
            subject.position = Vector3.Lerp(startPosit, dest, itemMoveCurve.Evaluate(time));
            subject.localScale = Vector3.Lerp(startScale, Vector3.zero, itemScaleCurve.Evaluate(time));
            subject.Rotate(0, 0, 10);
            yield return wait;
        }
        subject.localScale = Vector3.zero;
    }
}
