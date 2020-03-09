using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiPlayingMission : MonoBehaviour
{
    [SerializeField] private LocalText levelNameLabel = null;
    [SerializeField] private LocalText blocksLabel = null;
    [SerializeField] private GameObject blocksChecked = null;
    [SerializeField] private Image ballsImage = null;
    [SerializeField] private LocalText ballsLabel = null;
    [SerializeField] private GameObject ballsChecked = null;
    [SerializeField] private Image item0Image = null;
    [SerializeField] private LocalText item0Label = null;
    [SerializeField] private GameObject item0Checked = null;
    [SerializeField] private Image item1Image = null;
    [SerializeField] private LocalText item1Label = null;
    [SerializeField] private GameObject item1Checked = null;
    [SerializeField] private Image leagueScoreImage = null;
    [SerializeField] private LocalText leagueScoreLabel = null;
    [SerializeField] private AnimationCurve itemMoveCurve = null;
    [SerializeField] private AnimationCurve itemScaleCurve = null;

    private void Awake()
    {
        blocksChecked.gameObject.SetActive(false);
        ballsChecked.gameObject.SetActive(false);
        item0Checked.gameObject.SetActive(false);
        item1Checked.gameObject.SetActive(false);
    }

    private IEnumerator Start()
    {
        levelNameLabel.SetText(PlayModel.level.name);

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        // enable or disbale information billboards
        if (PlayModel.IsLeague)
        {
            leagueScoreImage.sprite = GlobalFactory.Leagues.GetCupSprite(PlayModel.type);
            leagueScoreImage.gameObject.SetActive(true);
        }
        else leagueScoreImage.gameObject.SetActive(false);

        if (PlayModel.level.targetBlocks > 0)
        {
            blocksLabel.transform.parent.gameObject.SetActive(true);
        }
        else blocksLabel.transform.parent.gameObject.SetActive(false);

        if (PlayModel.level.targetBalls > 0)
        {
            ballsLabel.transform.parent.gameObject.SetActive(true);
            ballsImage.sprite = GlobalFactory.Balls.GetSprite(PlayModel.ballId);
        }
        else ballsLabel.transform.parent.gameObject.SetActive(false);

        if (PlayModel.level.targetItem0 > 0)
        {
            item0Label.transform.parent.gameObject.SetActive(true);
            item0Image.sprite = GlobalFactory.Theme.Selected.items[0];
        }
        else item0Label.transform.parent.gameObject.SetActive(false);

        if (PlayModel.level.targetItem1 > 0)
        {
            item1Label.transform.parent.gameObject.SetActive(true);
            item1Image.sprite = GlobalFactory.Theme.Selected.items[1];
        }
        else item1Label.transform.parent.gameObject.SetActive(false);
        /////////////////////////////////////////////////////////////////////////////////////////////////////


        /////////////////////////////////////////////////////////////////////////////////////////////////////
        /// entering loop and update information billboards
        var wait = new WaitForSeconds(0.1f);
        while (true)
        {
            if (PlayModel.level.targetBlocks > 0)
            {
                var diff = PlayModel.level.targetBlocks - PlayModel.stats.totalBlocks;
                if (diff < 1)
                {
                    blocksLabel.gameObject.SetActive(false);
                    blocksChecked.gameObject.SetActive(true);
                }
                else blocksLabel.SetText(diff.ToString());
            }

            if (PlayModel.level.targetBalls > 0)
            {
                var diff = PlayModel.level.targetBalls - PlayModel.stats.totalBalls;
                if (diff < 1)
                {
                    ballsLabel.gameObject.SetActive(false);
                    ballsChecked.gameObject.SetActive(true);
                }
                else ballsLabel.SetText(diff.ToString());
            }

            if (PlayModel.level.targetItem0 > 0)
            {
                var diff = PlayModel.level.targetItem0 - PlayModel.stats.totalItem0;
                if (diff < 1)
                {
                    item0Label.gameObject.SetActive(false);
                    item0Checked.gameObject.SetActive(true);
                }
                else item0Label.SetText(diff.ToString());
            }

            if (PlayModel.level.targetItem1 > 0)
            {
                var diff = PlayModel.level.targetItem1 - PlayModel.stats.totalItem1;
                if (diff < 1)
                {
                    item1Label.gameObject.SetActive(false);
                    item1Checked.gameObject.SetActive(true);
                }
                else item1Label.SetText(diff.ToString());
            }

            switch (PlayModel.type)
            {
                case PlayModel.Type.LeagueBalls:
                    leagueScoreLabel.SetText(PlayModel.stats.totalBalls.ToString());
                    break;
                case PlayModel.Type.LeagueBlocks:
                    leagueScoreLabel.SetText(PlayModel.stats.totalBlocks.ToString());
                    break;
                case PlayModel.Type.LeagueLegends:
                    leagueScoreLabel.SetText(PlayModel.stats.totalLegends.ToString());
                    break;
            }

            yield return wait;
        }
    }

    private void OnMessage(Messages.Param param)
    {
        if (PlayModel.level.targetItem0 < 1 && PlayModel.level.targetItem1 < 1) return;

        if (param.Is(Messages.Type.BlockDead) && param.Is<BlockValue>())
        {
            var subject = param.As<BlockValue>();
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
            time += Time.deltaTime;
            subject.position = Vector3.Lerp(startPosit, dest, itemMoveCurve.Evaluate(time));
            subject.localScale = Vector3.Lerp(startScale, Vector3.zero, itemScaleCurve.Evaluate(time));
            subject.Rotate(0, 0, 10);
            yield return wait;
        }
        subject.localScale = Vector3.zero;
    }
}
