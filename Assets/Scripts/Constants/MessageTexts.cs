using System.Collections.Generic;
using UnityEngine;

public static class MessageTexts
{
    public class MessageContent
    {
        public string Text;
        public Color TextColor;

        public MessageContent(string text, Color textColor)
        {
            Text = text;
            TextColor = textColor;
        }

        public string GetFormattedText(params string[] arg)
        {
            return string.Format(Text, arg);
        }
    }

    public enum MessageType { Suicide, Kill, Joined, Leaved, Kicked }

    public static readonly Dictionary<MessageType, MessageContent> MESSAGES = new Dictionary<MessageType, MessageContent>()
    {
        {MessageType.Suicide , new MessageContent("{0} commited suicide!", Color.red)  },
        {MessageType.Kill , new MessageContent("{0} got killed by {1}", Color.red)},
        {MessageType.Joined , new MessageContent("{0} joined the game", Color.blue)},
        {MessageType.Leaved , new MessageContent("{0} leaved the game", Color.blue)},
        {MessageType.Kicked , new MessageContent("{0} kicked by {1} from the game", Color.red)}
    };

    public static MessageContent GetMessageContent(MessageType type)
    {
        if (!MESSAGES.ContainsKey(type))
            throw new System.Exception("MessageTexts:: GetMessageContent:: entered type is not defined in messages dictionary.");

        return MESSAGES[type];
    }
}