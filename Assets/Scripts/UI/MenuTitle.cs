using TMPro;
using UnityEngine;

public class MenuTitle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameTitleText;
    [SerializeField] private TextMeshProUGUI gameVersionText;

    private void Start()
    {
        gameTitleText.text = Application.productName;
        gameVersionText.text = $"v{Application.version}";
    }
}