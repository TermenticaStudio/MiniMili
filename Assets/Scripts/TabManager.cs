using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    [SerializeField] private Tab[] tabs;
    [SerializeField] private int defaultTabIndex;

    [SerializeField] private TabButtonStyle selectedTabButton;
    [SerializeField] private TabButtonStyle deSelectedTabButton;

    private Tab currentTab;
    private int lastSelectedTab;

    [Serializable]
    public class Tab
    {
        public string Title;
        public GameObject Panel;
        public Button Button;

        [Space(3)]
        public UnityEvent OnSelect;
        public UnityEvent OnSelectSilenced;
        public UnityEvent OnDeSelect;
    }

    [Serializable]
    public class TabButtonStyle
    {
        public Sprite Sprite;
        public Color Color;
        public Color TextColor;
    }

    private void Start()
    {
        InitTabs();
        SelectTab(defaultTabIndex, silenceMode: true);
    }

    private void InitTabs()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            var index = i;
            tabs[i].Button.onClick.AddListener(() => SelectTab(index));
            //tabs[i].Button.onClick.AddListener(AudioManager.Instance.PlayClickSFX);
            tabs[i].Panel.SetActive(false);
            tabs[i].OnDeSelect?.Invoke();
            SetColor(deSelectedTabButton, tabs[i], tabs[i].Button.GetComponentsInChildren<TextMeshProUGUI>().ToList());
        }
    }

    public void SelectTab(int index, bool invokeEvents = true, bool silenceMode = false)
    {
        if (tabs.Length == 0)
            return;

        if (currentTab != null)
        {
            if (currentTab == tabs[index])
                return;

            var prevTab = currentTab;

            if (prevTab.Panel == null)
                throw new NullReferenceException("TabManager::SelectTab:: Previous tab panel is null.");

            prevTab.Panel.SetActive(false);

            if (invokeEvents)
                prevTab.OnDeSelect?.Invoke();

            SetColor(deSelectedTabButton, prevTab, prevTab.Button.GetComponentsInChildren<TextMeshProUGUI>().ToList());
        }

        currentTab = tabs[index];
        lastSelectedTab = index;

        if (currentTab.Panel == null)
            throw new NullReferenceException("TabManager::SelectTab:: Current tab panel is null.");

        currentTab.Panel.SetActive(true);

        if (invokeEvents)
        {
            if (silenceMode)
                currentTab.OnSelectSilenced?.Invoke();
            else
                currentTab.OnSelect?.Invoke();
        }

        SetColor(selectedTabButton, currentTab, currentTab.Button.GetComponentsInChildren<TextMeshProUGUI>().ToList());
    }

    public void SelectTab(int index)
    {
        SelectTab(index, true, false);
    }

    public void SelectLastTab()
    {
        SelectTab(lastSelectedTab, true, false);
    }

    private void SetColor(TabButtonStyle style, Tab tab, List<TextMeshProUGUI> texts)
    {
        tab.Button.image.color = style.Color;
        tab.Button.image.sprite = style.Sprite;

        foreach (var text in texts)
            text.color = style.TextColor;
    }

    public Tab[] GetTabs() => tabs;
}