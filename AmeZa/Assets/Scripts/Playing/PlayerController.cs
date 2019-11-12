using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : Base
{
    [SerializeField] private float rangeAngle = 83;
    [SerializeField] private Transform previewArrow = null;
    [SerializeField] private LineRenderer previewLine = null;
    [SerializeField] private Transform previewBall = null;

    private Vector3 direction = Vector3.zero;
    private RaycastHit2D[] previewHits = new RaycastHit2D[100];
    private ContactFilter2D previewFilter = new ContactFilter2D();
    private float ballRadius = 0.1f;

    private bool isShooting = false;
    private bool isDown = false;
    private bool isHold = false;
    private bool isUp = false;
    private bool CanShoot { get { return isShooting == false && isDown == false && isHold == false && isUp == false; } }


    private void Awake()
    {
        ballRadius = GlobalFactory.Balls.GetPrefab(0).transform.localScale.y;
        previewArrow.localScale = Vector3.one * ballRadius;

        previewArrow.gameObject.SetActive(false);
        previewFilter.useLayerMask = true;
        previewFilter.layerMask = 1 << 9 | 1 << 11;
        previewFilter.useTriggers = false;
        ballRadius *= 0.5f;
    }

    private void Update()
    {
        if (gameManager.CurrentPopup != null) return;

        if (isUp)
        {
            isUp = false;
            isHold = false;
            isDown = false;
            isShooting = true;
            previewArrow.gameObject.SetActive(false);
            transform.root.Broadcast(Messages.Type.StartTurn, direction);
        }
        else if (isHold)
        {
            isUp = Input.GetMouseButtonUp(0);
            var hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), 20, 1 << 8);
            Vector3 currentPoint = hit.point;
            var distance = (currentPoint - BallManager.SpawnPoint);
            if (distance.magnitude > 0.1f)
            {
                var angle = Vector2.SignedAngle(Vector2.up, distance);
                if (-rangeAngle < angle && angle < rangeAngle)
                {
                    direction = distance.normalized;
                    DisplayPreview(angle);
                }
            }
        }
        else if (isDown)
        {
            isHold = Input.GetMouseButton(0);
            direction = Vector2.down;
            previewArrow.transform.position = BallManager.SpawnPoint;
        }
        else if (CanShoot)
        {
            isDown = Input.GetMouseButtonDown(0);
        }
    }

    private void DisplayPreview(float angle)
    {
        previewArrow.gameObject.SetActive(true);
        previewArrow.localPosition = BallManager.SpawnPoint;
        previewArrow.localEulerAngles = Vector3.forward * (90 + angle);
        Physics2D.CircleCast(BallManager.SpawnPoint, ballRadius, direction, previewFilter, previewHits, 20);
        previewBall.position = previewHits[0].point + previewHits[0].normal * ballRadius;
        previewLine.SetPosition(0, BallManager.SpawnPoint + direction * ballRadius);
        previewLine.SetPosition(1, previewBall.position);
    }

    private void OnMessage(Messages.Param param)
    {
        if (param.Is(Messages.Type.TurnEnded))
        {
            isShooting = false;
        }
    }
}
