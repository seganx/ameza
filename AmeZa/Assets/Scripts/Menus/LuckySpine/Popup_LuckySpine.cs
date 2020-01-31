using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup_LuckySpine : GameState
{
    [System.Serializable]
    public class Column
    {
        public RectTransform content = null;
        public float last = 0;
        public float current = 100;
        public float destination = 100;
        public AudioSource stepSound = null;
        public AudioSource endSound = null;
        public bool done { get { return Mathf.Abs(destination - current) < 20; } }
    }

    [SerializeField] private float speed = 800;
    [SerializeField] private Material blurMaterial = null;
    [SerializeField] private GameObject descStart = null;
    [SerializeField] private GameObject descAgain = null;
    [SerializeField] private Button stopButton = null;
    [SerializeField] private Column[] columns = null;

    private Vector3Int rewards = Vector3Int.zero;

    private int RandomReward { get { return Random.Range(0, 3) + Random.Range(0, 3); } }

    // Use this for initialization
    private void Start()
    {
#if UNITY_EDITOR
        {
            int[] items = new int[5] { 0, 0, 0, 0, 0 };
            for (int i = 0; i < 1000; i++)
            {
                Init();
                rewards.x = (rewards.x + 10) / 10 + 19 + RandomReward;
                rewards.y = (rewards.y + 10) / 10 + 29 + RandomReward;
                rewards.z = (rewards.z + 10) / 10 + 39 + RandomReward;

                rewards.x = rewards.x % 5;
                rewards.y = rewards.y % 5;
                rewards.z = rewards.z % 5;
                items[rewards.x]++;
                items[rewards.y]++;
                items[rewards.z]++;
            }
            string d = "Rewards:";
            d += "Bombs[" + items[0] + "] ";
            d += "Scissors[" + items[1] + "] ";
            d += "gems[" + items[2] + "] ";
            d += "hammer[" + items[3] + "] ";
            d += "hearts[" + items[4] + "] ";
            Debug.Log(d);
        }
#endif

        descStart.SetActive(true);
        descAgain.SetActive(false);
        Init();
        UiShowHide.ShowAll(transform);


        stopButton.onClick.AddListener(() =>
        {
            stopButton.SetInteractable(false);

            Init();
            rewards.x = (rewards.x + 10) / 10 + 19 + RandomReward;
            rewards.y = (rewards.y + 10) / 10 + 29 + RandomReward;
            rewards.z = (rewards.z + 10) / 10 + 39 + RandomReward;

            columns[0].destination = -100 * rewards.x;
            columns[1].destination = -100 * rewards.y;
            columns[2].destination = -100 * rewards.z;

            for (int i = 0; i < columns.Length; i++)
                SetImagesMaterial(columns[i], blurMaterial);
        });



    }

    // Update is called once per frame
    private void Update()
    {
        for (int i = 0; i < columns.Length; i++)
        {
            var item = columns[i];

            if (item.done == false)
            {
                item.current = Mathf.MoveTowards(item.current, item.destination, Time.deltaTime * speed);

                if (item.done)
                {
                    item.current = item.destination;
                    SetImagesMaterial(item, null);
                    item.endSound.pitch = Random.Range(0.99f, 1.01f);
                    item.endSound.volume = Random.Range(0.7f, 1.0f);
                    item.endSound.PlayOneShot(item.endSound.clip);
                    CheckSpinEnds();
                }
                else if (Mathf.Abs(item.current - item.last) > 100)
                {
                    item.last = item.current;
                    if (item.stepSound && item.stepSound.gameObject.activeSelf && item.stepSound.enabled)
                    {
                        item.stepSound.pitch = Random.Range(0.99f, 1.01f);
                        item.stepSound.volume = Random.Range(0.3f, 0.6f);
                        item.stepSound.PlayOneShot(item.stepSound.clip);
                    }
                }
            }
        }

        UpdateColumnsPosition();
    }

    private void Init()
    {
        rewards.x = RandomReward;
        rewards.y = RandomReward;
        rewards.z = RandomReward;

        for (int i = 0; i < columns.Length; i++)
            columns[i].last = columns[i].destination = columns[i].current = -100 * rewards[i];
    }

    private void UpdateColumnsPosition()
    {
        var height = columns[0].content.rect.height - 240 * 2;
        for (int i = 0; i < columns.Length; i++)
        {
            if (columns[i].current >= 0)
                columns[i].content.SetAnchordPositionY(240 + columns[i].current % height);
            else
                columns[i].content.SetAnchordPositionY(740 + columns[i].current % height);
        }
    }

    private void SetImagesMaterial(Column column, Material mat)
    {
        var images = column.content.GetComponentsInChildren<Image>();
        foreach (var item in images)
            item.material = mat;
    }

    private void CheckSpinEnds()
    {
        for (int i = 0; i < columns.Length; i++)
            if (columns[i].done == false) return;

        int[] items = new int[5] { 0, 0, 0, 0, 0 };

        rewards.x = rewards.x % 5;
        rewards.y = rewards.y % 5;
        rewards.z = rewards.z % 5;
        items[rewards.x]++;
        items[rewards.y]++;
        items[rewards.z]++;

        string d = "Rewards:";
        d += "Bombs[" + items[0] + "] ";
        d += "Scissors[" + items[1] + "] ";
        d += "gems[" + items[2] + "] ";
        d += "hammer[" + items[3] + "] ";
        d += "hearts[" + items[4] + "] ";
        Debug.Log(d);

        if (items[0] > 1 || items[1] > 1 || items[2] > 1 || items[3] > 1)
        {
            var gems = items[2] * Random.Range(100, 200);
            Profile.EarnGems(gems);
            Profile.Bombs += Mathf.Max(0, items[0] - 1);
            Profile.Missiles += Mathf.Max(0, items[1] - 1);
            Profile.Hammers += Mathf.Max(0, items[3] - 1);
            gameManager.OpenPopup<Popup_Rewards>().Setup(0, gems, Mathf.Max(0, items[0] - 1), Mathf.Max(0, items[3] - 1), Mathf.Max(0, items[1] - 1), true);
            descAgain.SetActive(false);
        }
        else if (items[4] > 1)
        {
            Profile.Hearts += items[4] - 1;
            gameManager.OpenPopup<Popup_Confirm>().SetText(111017, items[4] - 1).Setup(true, false, null);
            descAgain.SetActive(false);
        }
        else
        {
            descStart.SetActive(false);
            descAgain.SetActive(true);
            stopButton.SetInteractable(true);
        }
    }
}
