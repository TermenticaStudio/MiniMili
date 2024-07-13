using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MapPreview : MonoBehaviour
{
    [SerializeField] private Image mapPreview;
    [SerializeField] private TextMeshProUGUI mapNameText;
    [SerializeField] private GameObject selectedFrame;

    public bool IsSelected { get; private set; }
    public MapInfoSO Info { get; private set; }

    private Button btn;

    public void Init(MapInfoSO info, Action onClick)
    {
        Info = info;

        mapPreview.sprite = info.Preview;
        mapNameText.text = info.Title;

        btn = GetComponent<Button>();
        btn.onClick.AddListener(() => onClick?.Invoke());
        btn.onClick.AddListener(Select);

        Deselect();
    }

    public void Select()
    {
        selectedFrame.SetActive(true);
        IsSelected = true;
    }

    public void Deselect()
    {
        selectedFrame.SetActive(false);
        IsSelected = false;
    }
}