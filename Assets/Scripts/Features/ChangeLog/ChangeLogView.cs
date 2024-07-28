using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Feature.ChangeLog
{
    public class ChangeLogView : MonoBehaviour
    {
        [SerializeField] private GameObject changeLogPanel;
        [SerializeField] private TextMeshProUGUI currentVersionText;
        [SerializeField] private TextMeshProUGUI changeLogText;
        [SerializeField] private Button okButton;

        public void Init()
        {
            HideChangeLog();

            okButton.onClick.AddListener(HideChangeLog);
        }

        public void ShowChangeLog(string log)
        {
            currentVersionText.text = $"v{Application.version}";
            changeLogText.text = log;
            changeLogPanel.SetActive(true);
        }

        public void HideChangeLog()
        {
            changeLogPanel.SetActive(false);
        }
    }
}