using System;
using System.Text;
using UnityEngine;

namespace Feature.ChangeLog
{
    [RequireComponent(typeof(ChangeLogView))]
    public class ChangeLogController : MonoBehaviour
    {
        [SerializeField] private ChangeLogData data;

        private const string _CHANGELOG_PREFS = "changelog_hash_v{0}";
        private ChangeLogView _view;

        private void Start()
        {
            _view = GetComponent<ChangeLogView>();
            _view.Init();

            if (CheckForChangelog(out var result))
                _view.ShowChangeLog(result);
        }

        private bool CheckForChangelog(out string changeLog)
        {
            var descHash = GetStringSha256Hash(data.ChangeLog);

            if (PlayerPrefs.GetString(string.Format(_CHANGELOG_PREFS, Application.version)) != descHash)
            {
                PlayerPrefs.SetString(string.Format(_CHANGELOG_PREFS, Application.version), descHash);

                changeLog = data.ChangeLog;
                return true;
            }

            changeLog = string.Empty;
            return false;
        }

        private string GetStringSha256Hash(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                var textData = Encoding.UTF8.GetBytes(text);
                var hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }
    }
}