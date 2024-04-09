using Mirror;
using System.Net.Sockets;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private PlayerDB playerDB;

    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private TMP_InputField networkAddress;
    [SerializeField] private TextMeshProUGUI systemIpText;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void Start()
    {
        Application.targetFrameRate = 60;

        hostButton.onClick.AddListener(HostGame);
        clientButton.onClick.AddListener(JoinGame);

        playerName.onValueChanged.AddListener((value) =>
        {
            PlayerPrefs.SetString("PLAYER_NAME", value);
        });

        playerName.text = "Kunde" + Random.Range(0, 99);

        networkAddress.onValueChanged.AddListener((value) =>
        {
            NetworkManager.singleton.networkAddress = value;
        });

        Debug.Log(GetLocalIPAddress());

        GetLocalIPAddress();
    }

    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                systemIpText.text = ip.ToString();
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    public void JoinGame()
    {
        if (string.IsNullOrEmpty(networkAddress.text))
            return;

        NetworkManager.singleton.StartClient();
    }

    private void HostGame()
    {
        NetworkManager.singleton.networkAddress = GetLocalIPAddress();
        NetworkManager.singleton.StartHost();
    }
}