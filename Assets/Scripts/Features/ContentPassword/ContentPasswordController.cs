using Feature.Notifier;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Feature.ContentPassword
{
    public class ContentPasswordController : Singleton<ContentPasswordController>
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_InputField passwordField;
        [SerializeField] private Button submitBtn;
        [SerializeField] private Button cancelBtn;

        private event Action _onPass;
        private IEncryptedContent _currentContent;

        private void Start()
        {
            submitBtn.onClick.AddListener(CheckPassword);
            cancelBtn.onClick.AddListener(ClosePanel);

            ClosePanel();
        }

        public void Pass(IEncryptedContent content, Action onPass)
        {
            if (!content.IsLocked)
            {
                onPass?.Invoke();
                return;
            }

            _currentContent = content;
            OpenPanel();

            passwordField.Select();
            _onPass = onPass;
        }

        private void OpenPanel()
        {
            panel.SetActive(true);
        }

        private void ClosePanel()
        {
            passwordField.text = string.Empty;
            panel.SetActive(false);

            _onPass = null;
        }

        private void CheckPassword()
        {
            if (_currentContent == null)
                return;

            if (string.Equals(_currentContent.Password, passwordField.text.Trim()))
            {
                _onPass?.Invoke();
                ClosePanel();
            }
            else
            {
                NotifyManager.Instance.Notify(new MessageTexts.MessageContent("Wrong password!", Color.red));
                passwordField.text = string.Empty;
            }
        }
    }
}