using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentPassword : MonoBehaviour
{
    #region Singleton
    public static ContentPassword Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        transform.SetParent(null);
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private Button submitBtn;
    [SerializeField] private Button cancelBtn;

    private event Action OnPass;

    private IEncryptedContent currentContent;

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

        currentContent = content;
        OpenPanel();

        passwordField.Select();
        OnPass = onPass;
    }

    private void OpenPanel()
    {
        panel.SetActive(true);
    }

    private void ClosePanel()
    {
        passwordField.text = string.Empty;
        panel.SetActive(false);

        OnPass = null;
    }

    private void CheckPassword()
    {
        if (currentContent == null)
            return;

        if (string.Equals(currentContent.Password, passwordField.text.Trim()))
        {
            OnPass?.Invoke();
            ClosePanel();
        }
        else
        {
            InGameMessage.Instance.Notify(new MessageTexts.MessageContent("Wrong password!", Color.red));
            passwordField.text = string.Empty;
        }
    }
}
