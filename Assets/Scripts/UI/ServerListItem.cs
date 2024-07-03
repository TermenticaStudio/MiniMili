using System;
using TMPro;
using UnityEngine;

public class ServerListItem : MonoBehaviour
{
    private string serverName;
    public Uri uri;
    [SerializeField] private TextMeshProUGUI text;
    public void Setup(Uri uri, string serverName)
    {   
        this.uri = uri;
        this.serverName = serverName;
        
        text.text = serverName;
    }
}
