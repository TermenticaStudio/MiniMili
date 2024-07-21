using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameDebug.WeaponDebug
{
    public class WeaponDebugBoolField : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Toggle valueField;

        public void Setup(string title, bool value, Action<bool> onChange)
        {
            titleText.text = title;
            valueField.isOn = value;
            valueField.onValueChanged.AddListener(newValue =>
            {
                onChange?.Invoke(newValue);
            });
        }
    }
}