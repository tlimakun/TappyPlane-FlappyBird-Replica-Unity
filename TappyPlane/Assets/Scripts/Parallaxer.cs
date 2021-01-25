using UnityEngine;

public class Parallaxer : MonoBehaviour
{
    class PoolObject
    {
        internal Transform transform;
        internal bool inUse;
        public PoolObject(Transform t) { transform = t; }

        public void Use()
        {
            inUse = true;
        }

        public void Dispose()
        {
            inUse = false;
        }
    }

    [System.Serializable]
    public struct YSpawnRange
    {
        [SerializeField] internal float min;
        [SerializeField] internal float max;
    }

    [SerializeField] private GameObject Prefab;
    [SerializeField] private int poolSize;
    [SerializeField] private float shiftSpeed;
    [SerializeField] private float spawnRate;

    [SerializeField] private YSpawnRange ySpawnRange;
    [SerializeField] private Vector2 defaultSpawnPos;
    [SerializeField] private bool spawnImmediate; // particle prewarm
    [SerializeField] private Vector2 immediateSpawnPos;
    [SerializeField] private Vector2 targetAspectRatio;

    private float spawnTimer;
    private float targetAspect;
    PoolObject[] poolObjects;

    GameManager game;

    private void Awake()
    {
        Configure();
    }

    private void Start()
    {
        game = GameManager.instance;
    }

    private void OnEnable()
    {
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    private void OnDisable()
    {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    private void OnGameOverConfirmed()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].Dispose();
            poolObjects[i].transform.position = Vector2.one * 1000;
        }
        if (spawnImmediate)
        {
            SpawnImmidiate();
        }
    }

    private void Update()
    {
        if (game.IsGameOver) { return; }

        Shift();
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnRate)
        {
            Spawn();
            spawnTimer = 0;
        }
    }

    private void Configure()
    {
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        poolObjects = new PoolObject[poolSize];
        for (int i = 0; i < poolObjects.Length; i++)
        {
            GameObject go = Instantiate(Prefab);
            Transform t = go.transform;
            t.SetParent(transform);
            t.position = Vector3.one * 1000;
            poolObjects[i] = new PoolObject(t);
        }

        if (spawnImmediate)
        {
            SpawnImmidiate();
        }
    }

    private void Spawn()
    {
        Transform t = GetPoolObject();

        if (t == null) return;

        Vector2 pos = Vector2.zero;
        pos.x = (defaultSpawnPos.x * Camera.main.aspect) / targetAspect;
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = pos;
    }

    private void SpawnImmidiate()
    {
        Transform t = GetPoolObject();

        if (t == null) return;

        Vector2 pos = Vector2.zero;
        pos.x = (immediateSpawnPos.x * Camera.main.aspect) / targetAspect;
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        t.position = pos;
        Spawn();
    }

    private void Shift()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].transform.position += -Vector3.right * shiftSpeed * Time.deltaTime;
            CheckDisposeObject(poolObjects[i]);
        }
    }

    private void CheckDisposeObject(PoolObject poolObject)
    {
        if (poolObject.transform.position.x < (-defaultSpawnPos.x * Camera.main.aspect) / targetAspect)
        {
            poolObject.Dispose();
            poolObject.transform.position = Vector2.one * 1000;
        }
    }

    private Transform GetPoolObject()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            if (!poolObjects[i].inUse)
            {
                poolObjects[i].Use();
                return poolObjects[i].transform;
            }
        }

        return null;
    }
}
