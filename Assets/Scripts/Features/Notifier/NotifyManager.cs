using UnityEngine;

namespace Feature.Notifier
{
    public class NotifyManager : Singleton<NotifyManager>
    {
        [SerializeField] private NotifyMessage message;
        [SerializeField] private Transform holder;

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
}