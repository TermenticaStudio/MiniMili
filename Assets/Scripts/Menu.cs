using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private PlayerDB playerDB;

    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void Start()
    {
        hostButton.onClick.AddListener(NetworkManager.singleton.StartHost);
        clientButton.onClick.AddListener(NetworkManager.singleton.StartClient);

        playerName.onValueChanged.AddListener((value) =>
        {
            playerDB.Name = value;
        });

        playerName.text = "Kunde" + Random.Range(0, 99);
    }
}