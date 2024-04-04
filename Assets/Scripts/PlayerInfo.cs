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

    [Server]
    public void SetPlayerDB(PlayerDB playerDB)
    {
        this.playerDB = playerDB;
        SetName(playerDB.Name);
    }

    [ClientRpc]
    private void SetName(string name)
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