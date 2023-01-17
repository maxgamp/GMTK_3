using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WaveController : MonoBehaviour
{
    public Sprite[] EnemySprites;
    public Sprite[] BulletSprites;
    public AudioClip[] AudioClips;
    public AudioClip[] ExplosionAudioClips;
    public AudioClip[] HitShieldClips;

    public GameObject EnemyPrefab;
    public GameObject BulletPrefab;
    public GameObject Player;

    [Range(0.1f, 2f)]
    public float MaxSpread = 0.3f;
    public float BulletSpeed = 5f;
    public int WaveCounter;
    public int EnemyCount;
    public float SpawnCount;
    public float EnemyIncrease = 1.25f;
    private bool isWave;
    public float countdown;

    public Text score;

    private float height;
    private float width;

    void Start()
    {
        WaveCounter = 1;
        countdown = 5f;
        Player = FindObjectOfType<PlayerController>().gameObject;
        isWave = false;

        height = 2f * Camera.main.orthographicSize;
        width = height * Camera.main.aspect;
    }

    void SpawnWave()
    {
        score.text = "Wave " + WaveCounter;
        SpawnCount *= EnemyIncrease;
        for (var i = 0; i < SpawnCount; i++)
        {
            var spawnPos = new Vector3(Random.Range(-width / 2 + 1, width / 2 - 1), Random.Range(-height / 2 + 1, height / 2 - 1), 0);

            var dir = Player.transform.position - spawnPos;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            var rotation = Quaternion.AngleAxis(angle-90f, Vector3.forward);

            var tmp = Instantiate(EnemyPrefab, spawnPos, rotation);

            tmp.GetComponent<SpriteRenderer>().sprite = EnemySprites[Random.Range(0, EnemySprites.Length)];
            tmp.AddComponent<PolygonCollider2D>();
        }

        isWave = true;
    }

    void EndWave()
    {
        WaveCounter++;
        countdown = 5f;
        isWave = false;
    }

    public void DestroyEnemies()
    {
        //List<GameObject> tmp = new List<GameObject>();

        foreach (var enemy in FindObjectsOfType<EnemyController>())
        {
            Destroy(enemy.gameObject);
            //tmp.Add(enemy.gameObject);
        }

        //for (var i = tmp.Count - 1; i >= 0; i--)
        //{
        //    Destroy(tmp[i]);
        //}
    }

    void Update()
    {

        if (Player.GetComponent<PlayerController>().dead)
        {
            return;
        }

        EnemyCount = 0;
        foreach (var enemy in FindObjectsOfType<EnemyController>())
        {
            EnemyCount++;
        }

        if (EnemyCount == 0 && isWave)
        {
            EndWave();
        }
        else if (countdown >= 0)
        {
            countdown -= Time.deltaTime;
        }
        else if(countdown <= 0 && !isWave)
        {
            SpawnWave();
        }
    }
}
