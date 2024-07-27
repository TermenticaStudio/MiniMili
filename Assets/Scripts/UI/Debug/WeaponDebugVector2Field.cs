using System;
using TMPro;
using UnityEngine;

namespace GameDebug.Debug
{
    public class WeaponDebugVector2Field : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TMP_InputField xField;
        [SerializeField] private TMP_InputField yField;

        public void Setup(string title, Vector2 value, Action<Vector2> onChange)
        {
            titleText.text = title;
            xField.text = value.x.ToString();
            yField.text = value.y.ToString();

            xField.onEndEdit.AddListener(newValue =>
            {
                onChange?.Invoke(GetVector(xField.text, yField.text));
            });

            yField.onEndEdit.AddListener(newValueY =>
            {
                onChange?.Invoke(GetVector(xField.text, yField.text));
            });
        }

        private Vector2 GetVector(string x, string y)
        {
            var xF = 0f;
            var yF = 0f;

            if (!string.IsNullOrEmpty(x) && float.TryParse(x, out var numX))
                xF = numX;

            if (!string.IsNullOrEmpty(y) && float.TryParse(y, out var numY))
                yF = numY;

            return new Vector2(xF, yF);
        }
    }
}