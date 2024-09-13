using Logic.Player;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    [Scene]
    public string gameScene;
    struct Match
    {
        public string id;

        public Scene scene;
    }
  /*  struct MatchDetails
    {
        public string id;
        public 
    }*/
    readonly List<Match> openMatches = new List<Match>();
    public override void OnStartServer()
    {
        base.OnStartServer();
        if(mode == NetworkManagerMode.ServerOnly)
        {
            FindObjectOfType<HttpServer>().StartHttpServer();
        }
        PlayerSpawnHandler.OnStart();
    }
    public override void OnStopServer()
    {
        NetworkServer.SendToAll(new SceneMessage { sceneName = gameScene, sceneOperation = SceneOperation.UnloadAdditive });
        StartCoroutine(ServerUnloadSubScenes());
    }

    // Unload the subScenes and unused assets and clear the subScenes list.
    IEnumerator ServerUnloadSubScenes()
    {
        for (int index = 0; index < openMatches.Count; index++)
            if (openMatches[index].scene.IsValid())
                yield return SceneManager.UnloadSceneAsync(openMatches[index].scene);

        openMatches.Clear();

        yield return Resources.UnloadUnusedAssets();
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject gO = Instantiate(playerPrefab);
        gO.name = conn.connectionId.ToString();
        Player player = gO.GetComponent<Player>();
     //   player.MultiSetup();
        PlayerInfo playerInfo = gO.GetComponent<PlayerInfo>();
        gO.transform.position = GetStartPosition().position;

        NetworkServer.AddPlayerForConnection(conn, gO);
        PlayerSpawnHandler.SpawnPickup(GetStartPosition().position);
    }
    [Server]
    public void StartNewGame(StartGameRequest matchReq)
    {
         
    }
    IEnumerator ServerLoadSubScene(string id)
    {
        
            yield return SceneManager.LoadSceneAsync(gameScene, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics3D });

            Scene newScene = SceneManager.GetSceneAt(openMatches.Count);
            Match match = new Match();
            match.scene = newScene;
            match.id = id;
            openMatches.Add(match);
           // Spawner.InitialSpawn(newScene);
        
    }
    public override void OnStopClient()
    {
        // Make sure we're not in ServerOnly mode now after stopping host client
        if (mode == NetworkManagerMode.Offline)
            StartCoroutine(ClientUnloadSubScenes());
    }

    // Unload all but the active scene, which is the "container" scene
    IEnumerator ClientUnloadSubScenes()
    {
        for (int index = 0; index < SceneManager.sceneCount; index++)
            if (SceneManager.GetSceneAt(index) != SceneManager.GetActiveScene())
                yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(index));
    }
    IEnumerator OnServerAddPlayerDelayed(NetworkConnectionToClient conn)
    {

        // Send Scene message to client to additively load the game scene
        conn.Send(new SceneMessage { sceneName = gameScene, sceneOperation = SceneOperation.LoadAdditive });

        // Wait for end of frame before adding the player to ensure Scene Message goes first
        yield return new WaitForEndOfFrame();

        base.OnServerAddPlayer(conn);

/*        PlayerScore playerScore = conn.identity.GetComponent<PlayerScore>();
        playerScore.playerNumber = clientIndex;
        playerScore.scoreIndex = clientIndex / openMatches.Count;
        playerScore.matchIndex = clientIndex % openMatches.Count;

        // Do this only on server, not on clients
        // This is what allows Scene Interest Management
        // to isolate matches per scene instance on server.
        if (subScenes.Count > 0)
            SceneManager.MoveGameObjectToScene(conn.identity.gameObject, openMatches[clientIndex % subScenes.Count]);

        clientIndex++;
*/    }
}
