using UnityEngine;

public class InGameMessage : MonoBehaviour
{
    public static InGameMessage Instance;

    [SerializeField] private Message message;
    [SerializeField] private Transform holder;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (Transform mes in holder.transform)
            Destroy(mes.gameObject);
    }

    public void Notify(MessageTexts.MessageContent content, params string[] stringArgs)
    {
        var instance = Instantiate(message, holder);
        instance.Init(content.GetFormattedText(stringArgs), content.TextColor);
    }
}