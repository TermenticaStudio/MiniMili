using System;
using UnityEngine;
using UnityEngine.UI;

public class ServersListDisplayer : MonoBehaviour
{
    [SerializeField] private GameObject serverItemPrefab;
    [SerializeField] private Transform Container;
    public Action<Uri> OnServerListButton;
    private LanDiscovery lanDiscovery;
    public void DisplayServers(IServerInfo[] serverInfo)
    {
        for (int i = 0; i < Container.childCount; i++)
        {
            Destroy(Container.GetChild(i).gameObject);
        }
        foreach (var item in serverInfo)
        {
            GameObject go = Instantiate(serverItemPrefab, Container);
            ServerListItem s = go.GetComponent<ServerListItem>();
            s.Setup(item.uri, item.serverName);
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnServerListButton.Invoke(s.uri);
            });
        }
    }

    private void Start()
    {
        Setup();
    }
    private void Setup()
    {
        lanDiscovery = LANNetworkManager.singleton.gameObject.GetComponent<LanDiscovery>();
    }
    private void DisplayServers()
    {
        DisplayServers(lanDiscovery.GetDiscoveredServers().ToArray());

    }
    public void Retry()
    {
        DisplayServers();
    }
    private void OnEnable()
    {
        try
        {
            DisplayServers();
        }
        catch
        {
            Setup();
            DisplayServers();
        }
    }
  
}