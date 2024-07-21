using System;
using TMPro;
using UnityEngine;

namespace GameDebug.WeaponDebug
{
    public class WeaponDebugNumericField : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TMP_InputField valueField;

        public void Setup(string title, float value, Action<float> onChange)
        {
            titleText.text = title;
            valueField.text = value.ToString();
            valueField.onEndEdit.AddListener(newValue =>
            {
                if (string.IsNullOrEmpty(newValue))
                    return;

                onChange?.Invoke(Convert.ToSingle(newValue));
            });
        }
    }
}