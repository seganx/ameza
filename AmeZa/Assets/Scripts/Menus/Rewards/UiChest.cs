using SeganX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiChest : Base
{
    [SerializeField] private Animation animator = null;
    [SerializeField] private Button button = null;
    [SerializeField] private GameObject buttonText = null;
    [SerializeField] private GameObject closed = null;
    [SerializeField] private GameObject opened = null;

    public bool IsOpened { get; private set; }

    private void Start()
    {
        button.onClick.AddListener(Open);
    }

    public void Open()
    {
        buttonText.SetActive(false);
        button.interactable = false;
        animator.Play();
        DelayCall(1, () =>
        {
            opened.gameObject.SetActive(true);
            closed.gameObject.SetActive(false);
            IsOpened = true;
        });
    }
}
