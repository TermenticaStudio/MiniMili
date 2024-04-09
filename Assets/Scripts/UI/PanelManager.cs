using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private Panel[] panels;
    [SerializeField] private int defaultPanel;

    private Panel currentPanel;

    private Stack<Panel> backHistory = new();

    [Serializable]
    public class Panel
    {
        public string Name;
        public GameObject PanelObj;
        public Button OpenButton;
        public Button[] CloseButtons;

        public void Close()
        {
            PanelObj.SetActive(false);
        }

        public void Open()
        {
            PanelObj.SetActive(true);
        }
    }

    private void Start()
    {
        InitPanels();
        OpenPanel(panels[defaultPanel].Name);
        //OpenDefaultPanel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Back();
    }

    private void InitPanels()
    {
        foreach (var panel in panels)
        {
            if (panel.OpenButton)
                panel.OpenButton.onClick.AddListener(() => OpenPanel(panel.Name));

            foreach (var btn in panel.CloseButtons)
            {
                btn.onClick.AddListener(Back);
            }

            panel.PanelObj.SetActive(false);
        }
    }

    public void OpenDefaultPanel()
    {
        var selectedPanel = panels[defaultPanel];

        currentPanel = selectedPanel;
        selectedPanel.Open();
    }

    public void OpenPanel(string name)
    {
        var selectedPanel = panels.FirstOrDefault(x => x.Name == name);

        if (currentPanel != null)
        {
            currentPanel.Close();
            backHistory.Push(currentPanel);
        }

        currentPanel = selectedPanel;
        currentPanel.Open();

    }

    public void Back()
    {
        if (backHistory.Count == 0)
            return;

        currentPanel.Close();

        var panelToOpen = backHistory.Pop();
        currentPanel = panelToOpen;
        panelToOpen.Open();
    }
}