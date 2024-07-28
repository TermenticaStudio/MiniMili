using UnityEngine;

namespace Feature.ChangeLog
{
    [CreateAssetMenu(menuName = "Features/Change Log/Data")]
    public class ChangeLogData : ScriptableObject
    {
        [TextArea] public string ChangeLog;
    }
}