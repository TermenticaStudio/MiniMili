using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MessageBtn : MonoBehaviour
{
    [SerializeField] private MessageTexts.MessageContent message;

    private Button btn;

    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(() => InGameMessage.Instance.Notify(message));
    }
}