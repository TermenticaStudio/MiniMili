using Mirror;
using TMPro;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
    [SerializeField] private TextMeshPro nameText;

    private PlayerDB playerDB;

    [SyncVar(hook = nameof(OnChangeName))]
    private string currentName;

    private void Update()
    {
        nameText.transform.rotation = Quaternion.identity;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        PlayerSpawnHandler.Instance.OnSpawnPlayer += OnSpawnPlayer;
    }

    private void OnSpawnPlayer(PlayerInfo obj)
    {
        SetPlayerName(PlayerPrefs.GetString("PLAYER_NAME"));
    }

    [Command]
    public void SetPlayerName(string name)
    {
        SetNameRpc(name);
    }

    [ClientRpc]
    private void SetNameRpc(string name)
    {
        currentName = name;
        nameText.text = name;
    }

    private void OnChangeName(string oldName, string newName)
    {
        currentName = newName;
        nameText.text = currentName;
    }
}