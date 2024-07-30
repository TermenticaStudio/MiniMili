using Feature.SceneLoader;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AISpawner : MonoBehaviour
{
    [SerializeField] private AIController[] aiControllers;
    [SerializeField] private int aliveAI = 10;
    [SerializeField] private float minRespawnDelay = 0.5f;
    [SerializeField] private float maxRespawnDelay = 1f;

    private List<AIController> _currentAIs = new();
    private List<AIController> _aliveAIs = new();
    private PlayerSpawnPoint[] _spawnPoints;

    private void Start()
    {
        SceneController.Instance.OnSceneLoaded += Init;
    }

    private void OnDestroy()
    {
        SceneController.Instance.OnSceneLoaded -= Init;
    }

    private void Init()
    {
        _spawnPoints = FindObjectsOfType<PlayerSpawnPoint>();

        if (aiControllers.Length == 0)
            return;

        for (int i = 0; i < aliveAI; i++)
            SpawnAI();
    }

    private void SpawnAI()
    {
        var ai = aiControllers[Random.Range(0, aiControllers.Length)];
        var availablePoints = _spawnPoints.Where(x => x.PlayerInArea() == false && CameraSightChecker.IsObjectInCameraSight(x.gameObject) == false).ToList();
        var spawnPoint = availablePoints[Random.Range(0, availablePoints.Count)];
        var instance = Instantiate(ai, spawnPoint.transform.position, Quaternion.identity, null);

        _currentAIs.Add(instance);
        _aliveAIs.Add(instance);
    }

    public void RespawnAI(AIController aiController)
    {
        _aliveAIs.Remove(aiController);
        StartCoroutine(RespawnAICoroutine(aiController));
    }

    private IEnumerator RespawnAICoroutine(AIController aiController)
    {
        yield return new WaitForSeconds(Random.Range(minRespawnDelay, maxRespawnDelay));

        // TODO: respawn fuckin ai
        _aliveAIs.Add(aiController);
    }
}