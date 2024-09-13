using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
public class ChatUI : MonoBehaviour
{
    public TMP_Text _chatText;
    [SerializeField] private ushort maxLines = 8;
    public float chatVanishingSpeed = 2f;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Color nicknameColor;
    [SerializeField] private int chatMaximumCharacters;
    [SerializeField] private Button _showChatButton; // Button to show chat input on mobile
    [SerializeField] private Button _sendButton;     // Button to send message
    public Action<string> onSendChat;
    //   [SerializeField] ContentBackground _background;

    public bool ChatWriting;
    private void Start()
    {
        _chatText.text = string.Empty;
      //  _inputField.gameObject.SetActive(false);
        // Add listeners for mobile buttons
        _showChatButton.onClick.AddListener(() => ShowChatInput(true));
        _sendButton.onClick.AddListener(Send);

       // _sendButton.gameObject.SetActive(false); // Hide send button initially

    }


    public void ShowChatInput(bool show)
    {
        ChatWriting = show;
        ShowChat(show);

    }
    public void Send()
    {
        if (string.IsNullOrWhiteSpace(GetMessage())) { return; }
        onSendChat?.Invoke(GetMessage());
        ShowChatInput(false);
    }



    public void WriteMessageToChat(string _msg, string sender) //adding message to all other messages and deleting additional lines
    {
        string newMessage = $" {"<b>" + $"<color=#{ColorUtility.ToHtmlStringRGBA(nicknameColor)}>" + sender + "</b>" + "</color>" + ": " + _msg}";
        string content = _chatText.text + $"\n {newMessage}";
        int extraLines = Utils.GetLineCount(content) - maxLines;
        if (extraLines > 0)
        {
            content = Utils.DeleteLines(content, extraLines);
        }
        _chatText.text = content;
      //  StopAllCoroutines();
      //  StartVanishingChat();
    }

    public void ShowChat(bool show)
    {

       // _sendButton.gameObject.SetActive(show); // Show send button on mobile when chat is active

       // _inputField.gameObject.SetActive(show);
        StopAllCoroutines();
        if (show)
        {
          //  _inputField.Select();
          //  _inputField.ActivateInputField();
            _chatText.color = Color.white;
        }
        else
        {
            _inputField.text = string.Empty;
        }
    }
    public void StartVanishingChat()
    {
        StartCoroutine(VanishChat());
    }
    IEnumerator VanishChat()
    {
        _chatText.color = Color.white;
        yield return new WaitForSeconds(5f);
        while (_chatText.color.a > 0.01f)
        {
            _chatText.color = Color.LerpUnclamped(_chatText.color, Color.clear, chatVanishingSpeed * Time.deltaTime);
            yield return null;
        }
        _chatText.color = Color.clear;
        _chatText.text = string.Empty;
    }
    public string GetMessage()
    {
        return _inputField.text;
    }


}

