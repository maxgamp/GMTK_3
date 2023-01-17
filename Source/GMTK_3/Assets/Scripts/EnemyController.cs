using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    [HideInInspector] public PlayerController Player;

    [HideInInspector] public WaveController WC;
    [HideInInspector] public GameObject BulletPrefab;
    public GameObject ParticleExplosionPrefab;

    private float AttackCoolDown;
    private float ticker;

    void Start()
    {
        WC = FindObjectOfType<WaveController>();
        Player = WC.Player.GetComponent<PlayerController>();

        AttackCoolDown = Mathf.Max(5f - WC.WaveCounter * 0.5f, 2f);

        BulletPrefab = WC.BulletPrefab;
        ticker = Random.Range(0, AttackCoolDown * 10) / 10f;
    }

    void OnDestroy()
    {
        Camera.main.gameObject.GetComponent<ShakeBehaviour>().TriggerShake(0.5f);
        AudioSource.PlayClipAtPoint(WC.ExplosionAudioClips[Random.Range(0, WC.ExplosionAudioClips.Length)], transform.position, 10f);
        Instantiate(ParticleExplosionPrefab, transform.position, quaternion.identity);
    }

    void Fire()
    {
        var dir = Player.transform.position - transform.position;

        var bullet = Instantiate(BulletPrefab, transform.position + dir.normalized * 0.5f, transform.rotation);
        bullet.GetComponent<SpriteRenderer>().sprite = WC.BulletSprites[Random.Range(0, 3)];

        if (Player.Apart)
        {
            if((Player.ApartShipA.transform.position - transform.position).sqrMagnitude < (Player.ApartShipB.transform.position - transform.position).sqrMagnitude)
                dir = Player.ApartShipA.transform.position - transform.position;
            else
                dir = Player.ApartShipB.transform.position - transform.position;
        }

        var BC = bullet.GetComponent<BulletController>();

        BC.dir = dir.normalized;
        BC.Speed = WC.BulletSpeed;
        BC.MaxSpread = WC.MaxSpread;
        BC.WC = WC;
        BC.HitShield = WC.HitShieldClips[Random.Range(0, WC.HitShieldClips.Length)];
        bullet.GetComponent<AudioSource>().clip = WC.AudioClips[Random.Range(0, WC.AudioClips.Length)];
    }

    void Update()
    {
        if (ticker < 0f)
        {
            Fire();
            ticker = AttackCoolDown;
        }

        var dir = Player.transform.position - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

        ticker -= Time.deltaTime;
    }
}
