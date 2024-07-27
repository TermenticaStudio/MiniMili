using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TabManager))]
public class TabManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var tabManager = (TabManager)target;
        
        var allTabs = tabManager.GetTabs();

        if (allTabs != null && allTabs.Length > 0)
        {
            EditorGUILayout.BeginHorizontal("box");

            for (int i = 0; i < allTabs.Length; i++)
            {
                var title = allTabs[i].Title;

                if (title.Length == 0)
                    title = (i + 1).ToString();

                if (GUILayout.Button(title))
                {
                    var index = i;
                    tabManager.SelectTab(index, false);
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}