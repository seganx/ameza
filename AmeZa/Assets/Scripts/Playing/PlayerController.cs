using SeganX;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : Base
{
    [SerializeField] private float rangeAngle = 83;
    [SerializeField] private Transform previewArrow = null;
    [SerializeField] private LineRenderer previewLine = null;
    [SerializeField] private LineRenderer previewReflect = null;
    [SerializeField] private Transform previewBall = null;

    private Vector3 direction = Vector3.zero;
    private RaycastHit2D[] previewHits = new RaycastHit2D[100];
    private ContactFilter2D previewFilter = new ContactFilter2D();
    private float ballRadius = 0.1f;

    //private float activeDelay
    private bool isShooting = false;
    private bool isDown = false;
    private bool isHold = false;
    private bool isUp = false;
    private bool CanShoot { get { return isShooting == false && isDown == false && isHold == false && isUp == false; } }

    private bool CanHandleInput
    {
        get
        {
            if (EventSystem.current.IsPointerOverGameObject()) return false;
            if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return false;
            return true;
        }
    }

    private void Awake()
    {
        Input.multiTouchEnabled = false;
        ballRadius = GlobalFactory.Balls.GetPrefab(0).transform.localScale.y;
        previewArrow.localScale = Vector3.one * ballRadius;

        previewArrow.gameObject.SetActive(false);
        previewFilter.useLayerMask = true;
        previewFilter.layerMask = 1 << 9 | 1 << 11;
        previewFilter.useTriggers = false;
        ballRadius *= 0.5f;

        rangeAngle = GlobalAnalytics.ABTest.Get(GlobalAnalytics.rc_shoot_angle, rangeAngle);
    }

    private void Update()
    {
        if (isUp)
        {
            isUp = false;
            isHold = false;
            isDown = false;
            if (previewArrow.gameObject.activeSelf)
            {
                isShooting = true;
                transform.root.Broadcast(Messages.Type.StartTurn, direction.normalized);
            }
            previewArrow.gameObject.SetActive(false);
        }
        else if (isHold)
        {
            isUp = Input.GetMouseButtonUp(0);
            Vector3 hitpoint = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), 20, 1 << 8).point;
            var distance = (hitpoint - BallManager.SpawnPoint);
            var angle = Vector2.SignedAngle(Vector2.up, distance);

            if (distance.magnitude >= 1 && -rangeAngle < angle && angle < rangeAngle)
                previewArrow.gameObject.SetActive(true);
            else if (distance.magnitude < 0.5f)
                previewArrow.gameObject.SetActive(false);

            if (-rangeAngle < angle && angle < rangeAngle)
            {
                direction = distance.normalized;
                DisplayPreview(angle);
            }
        }
        else if (isDown)
        {
            direction = Vector2.down;
            isHold = Input.GetMouseButton(0);
            previewArrow.transform.position = BallManager.SpawnPoint;
        }
        else if (CanShoot && CanHandleInput)
        {
            isDown = Input.GetMouseButtonDown(0);
        }
    }


    private void DisplayPreview(float angle)
    {
        previewArrow.localPosition = BallManager.SpawnPoint;
        previewArrow.localEulerAngles = Vector3.forward * (90 + angle);
        Physics2D.CircleCast(BallManager.SpawnPoint + direction * 0.01f, ballRadius, direction, previewFilter, previewHits, 20);
        previewBall.position = previewHits[0].point + previewHits[0].normal * ballRadius;
        previewLine.SetPosition(0, BallManager.SpawnPoint + direction * ballRadius);
        previewLine.SetPosition(1, previewBall.position);
        previewReflect.SetPosition(0, previewBall.position);
        previewReflect.SetPosition(1, previewBall.position + Vector3.Reflect(direction, previewHits[0].normal) * 5);
    }

    private void OnMessage(Messages.Param param)
    {
        if (param.Is(Messages.Type.TurnEnded))
        {
            isShooting = false;
        }
    }
}
