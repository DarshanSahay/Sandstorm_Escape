using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject[] powerUpsPrefab;
    [SerializeField] private GameObject[] obstaclePrefab;

    [SerializeField] private float powerUpSpawnTime = 5f;
    [SerializeField] private float obstacleSpawnTime = 4f;
    [SerializeField] private float coinSpawnTime = 2f;

    private float timeUntilCoinSpawn = 0f;
    private float timeUntilPowerSpawn = 0f;
    private float timeUntilObstacleSpawn = 0f;

    private bool isSpawning = false;
    private bool isGameRunning = false;
    [SerializeField] private float objectMoveSpeed;
    [SerializeField] private Transform objectSpawnPos;

    private void Start()
    {
        EventManager.Instance.OnGameStart += SetGameStatusOn;
        EventManager.Instance.OnGameOver += SetGameStatusOff;
    }

    private void Update()
    {
        if (!isGameRunning) return;
        if (isSpawning) return;

        timeUntilCoinSpawn += Time.deltaTime;
        timeUntilPowerSpawn += Time.deltaTime;
        timeUntilObstacleSpawn += Time.deltaTime;

        if (timeUntilCoinSpawn >= coinSpawnTime ||
            timeUntilPowerSpawn >= powerUpSpawnTime ||
            timeUntilObstacleSpawn >= obstacleSpawnTime)
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    private void SetGameStatusOn()
    {
        isGameRunning = true;
    }

    private void SetGameStatusOff()
    {
        isGameRunning = false;
    }

    IEnumerator SpawnRoutine()
    {
        isSpawning = true;

        List<ObjectType> availableTypes = new();

        if (timeUntilCoinSpawn >= coinSpawnTime) availableTypes.Add(ObjectType.Coin);
        if (timeUntilPowerSpawn >= powerUpSpawnTime) availableTypes.Add(ObjectType.PowerUp);
        if (timeUntilObstacleSpawn >= obstacleSpawnTime) availableTypes.Add(ObjectType.Obstacle);

        ObjectType selectedType = availableTypes[Random.Range(0, availableTypes.Count)];

        switch (selectedType)
        {
            case ObjectType.Coin:
                SpawnCoins();
                timeUntilCoinSpawn = 0f;
                break;

            case ObjectType.PowerUp:
                SpawnSingle(powerUpsPrefab);
                timeUntilPowerSpawn = 0f;
                break;

            case ObjectType.Obstacle:
                SpawnSingle(obstaclePrefab);
                timeUntilObstacleSpawn = 0f;
                break;
        }

        // Short delay before allowing next spawn
        yield return new WaitForSeconds(2f);

        isSpawning = false;
    }

    void SpawnCoins()
    {
        int count = Random.Range(3, 6);
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = objectSpawnPos.position + new Vector3(i * 1.5f, 0f, 0f);
            ObjectPooler.Instance.GetFromPool(coinPrefab, spawnPos);
        }
    }

    void SpawnSingle(GameObject[] prefabArray)
    {
        int index = Random.Range(0, prefabArray.Length);
        ObjectPooler.Instance.GetFromPool(prefabArray[index], objectSpawnPos.position);
    }
}
