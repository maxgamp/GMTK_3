using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public Vector3 dir;
    public float Speed;
    public float MaxSpread;

    public AudioClip HitShield;
    private AudioSource AS;
    public WaveController WC;

    void Start()
    {
        GetComponent<AudioSource>().Play();

        AS = GetComponent<AudioSource>();
    }

    void OnCollisionStay2D(Collision2D col)
    {
        OnCollisionEnter2D(col);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        switch (col.transform.name)
        {
            case var a when a.Contains("Wall"):
                Destroy(gameObject);
                break;
            case var b when b.Equals("Player"):
            case var c when c.Contains("ApartShip"):
                var PC = FindObjectOfType<PlayerController>();
                if (PC.dead)
                    return;
                
                PC.gameObject.GetComponent<ParticleSystem>().Play();
                PC.dead = true;
                PC.Die();
                WC.DestroyEnemies();
                break;
            case var d when d.Equals("LineRenderer"):
                AS.clip = HitShield;
                AS.Play();

                dir = Vector2.Reflect(dir, col.contacts[0].normal);

                if (StaticOptions.AimHelp)
                {
                    var tmp = new List<Vector3>();
                    foreach (var enemy in FindObjectsOfType<EnemyController>())
                    {
                        tmp.Add(enemy.transform.position - transform.position);
                    }

                    var query = tmp.OrderBy(e => (e.normalized - dir).sqrMagnitude);

                    if ((query.First().normalized - dir).sqrMagnitude < MaxSpread)
                    {
                        Debug.DrawRay(transform.position, query.First().normalized * 5f, Color.red, 10f, false);
                        dir = query.First().normalized;
                    }
                }
                break;
            case var e when e.Contains("Enemy"):
                Destroy(col.gameObject);
                break;
        }

    }



    void FixedUpdate()
    {
        transform.Rotate(Vector3.forward);

        transform.position += dir * Speed;

        if(Mathf.Abs(transform.position.x) > 50 || Mathf.Abs(transform.position.y) > 50)
            Destroy(gameObject);
    }
}
