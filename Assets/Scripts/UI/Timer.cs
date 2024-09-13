using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Utilities;


public class Timer : MonoBehaviour
{
    private TMP_Text text;
    public int duration = 10;
    public int timeRemaining;
    public bool isCountingDown = false;
    private void Start()
    {
        text = gameObject.GetComponentOrThrow<TMP_Text>();
    }

    public void Begin()
    {
        if (!isCountingDown)
        {
            isCountingDown = true;
            timeRemaining = duration;
            Invoke("_tick", 1f);
        }
    }

    private void _tick()
    {
        timeRemaining--;
        text.text = timeRemaining.ToString();
        if (timeRemaining > 0)
        {
            Invoke("_tick", 1f);
        }
        else
        {
            isCountingDown = false;
        }
    }

}

