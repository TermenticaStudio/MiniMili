using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private int lastFrameIndex;
    private float[] frameDeltaTimeArray;

    private void Awake()
    {
        frameDeltaTimeArray = new float[50];
    }

    private void Update()
    {
        //if (!ControlPanel.Instance.ShowFPS)
        //{
        //    text.text = string.Empty;
        //    return;
        //}

        if (Time.timeScale == 0)
            return;

        frameDeltaTimeArray[lastFrameIndex] = Time.deltaTime;
        lastFrameIndex = (lastFrameIndex + 1) % frameDeltaTimeArray.Length;

        text.text = Mathf.RoundToInt(CalculateFPS()).ToString();
    }

    private float CalculateFPS()
    {
        var total = 0f;
        foreach (var deltaTime in frameDeltaTimeArray)
            total += deltaTime;

        return frameDeltaTimeArray.Length / total;
    }
}