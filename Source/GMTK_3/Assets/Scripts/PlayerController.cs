using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector2 Offset;
    public AudioClip[] ApartClips;

    public AudioClip[] Tracks;

    [Range(0.5f, 10f)]
    public float Speed = 2f;

    [HideInInspector] public GameObject ApartShipA;
    [HideInInspector] public GameObject ApartShipB;
    private GameObject LineRendererObj;


    [HideInInspector] public bool Apart;
    [HideInInspector] public bool dead;


    private LineRenderer LR;
    private SpriteRenderer SR;
    private EdgeCollider2D EC;

    private AudioSource SC;
    private AudioSource ApartAudioSource;

    public Animator ImageAnim;
    public Animator GameOverAnimator;

    void Start()
    {
        StartCoroutine(PlayMusic());

        SR = GetComponent<SpriteRenderer>();
        LR = FindObjectOfType<LineRenderer>();
        LineRendererObj = LR.gameObject;
        LR.positionCount = 3;
        EC = FindObjectOfType<EdgeCollider2D>();
        SC = GetComponent<AudioSource>();
        ApartAudioSource = gameObject.AddComponent<AudioSource>();
        ApartAudioSource.volume = 0.6f;
        ApartAudioSource.playOnAwake = false;

        AudioListener.volume = StaticOptions.MasterVolume;

        var gradient = new Gradient();
        GradientColorKey[] colorKey;
        GradientAlphaKey[] alphaKey;

        colorKey = new GradientColorKey[3];
        colorKey[0].color = Color.blue;
        colorKey[0].time = 0.0f;
        colorKey[1].color = Color.white;
        colorKey[1].time = 0.5f;
        colorKey[1].color = Color.blue;
        colorKey[1].time = 1.0f;

        alphaKey = new GradientAlphaKey[3];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 0.8f;
        alphaKey[1].time = 1.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;

        gradient.SetKeys(colorKey, alphaKey);

        LR.colorGradient = gradient;

        foreach (var obj in FindObjectsOfType<Transform>())
        {
            if (obj.name.Equals("ApartShipA"))
            {
                ApartShipA = obj.gameObject;
                ApartShipA.SetActive(false);
            }
            else if(obj.name.Equals("ApartShipB"))
            {
                ApartShipB = obj.gameObject;
                ApartShipB.SetActive(false);
            }
        }

        
    }

    IEnumerator PlayMusic()
    {
        AudioSource AudioSource;
        AudioSource = gameObject.AddComponent<AudioSource>();
        var i = 0;

        while (!dead)
        {
            if (i >= Tracks.Length)
                i = 0;

            AudioSource.PlayOneShot(Tracks[i]);
            
            while (AudioSource.isPlaying)
                yield return null;
            i++;
        }
    }

    void ApartMovement()
    {

        checkIfOutOfBounds(ApartShipA.transform);
        checkIfOutOfBounds(ApartShipB.transform);

        var aPos = ApartShipA.transform.position;
        var bPos = ApartShipB.transform.position;

        aPos.z = 0;
        bPos.z = 0;

        LR.SetPosition(0, aPos);
        LR.SetPosition(1, aPos + (bPos - aPos) / 2);
        LR.SetPosition(2, bPos);


        EC.SetPoints(new List<Vector2>{aPos, aPos + (bPos - aPos) / 2, bPos});
    }

    void TogetherMovement()
    {
        checkIfOutOfBounds(transform);
    }

    void checkIfOutOfBounds(Transform t)
    {
        var height = 2f * Camera.main.orthographicSize;
        var width = height * Camera.main.aspect;
        var i = 0;
        while (Mathf.Abs(t.position.x) > width / 2f || Mathf.Abs(t.position.y) > height / 2f)
        {
            transform.position += (-t.position).normalized * 0.2f;

            if (i >= 100)
            {
                Debug.Log("It's over 100!");
                break;
            }
            i++;
        }
    }

    public void Die()
    {
        GameOverAnimator.SetTrigger("GameOverTrigger");
        SR.enabled = false;
        ApartShipA.SetActive(false);
        ApartShipB.SetActive(false);

        LineRendererObj.SetActive(false);
        SC.Stop();
    }

    void Update()
    {
        if (dead)
        {
            SR.enabled = false;
            ApartShipA.SetActive(false);
            ApartShipB.SetActive(false);

            LineRendererObj.SetActive(false);
            SC.Stop();

            if (Input.GetKey(KeyCode.Escape))
            {
                StartCoroutine(StaticOptions.LoadLevel(0, ImageAnim));
            }
            else if (Input.GetKey(KeyCode.R))
            {
                StartCoroutine(StaticOptions.FastLoadLevel(1));
            }
            return;
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            StartCoroutine(StaticOptions.LoadLevel(0, ImageAnim));
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            ApartAudioSource.clip = ApartClips[Random.Range(0, ApartClips.Length)];
            ApartAudioSource.Play();
            switch (Apart)
            {
                case true:
                    ApartShipA.SetActive(false);
                    ApartShipB.SetActive(false);

                    LineRendererObj.SetActive(false);
                    gameObject.GetComponent<SpriteRenderer>().enabled = true;
                    gameObject.GetComponent<PolygonCollider2D>().enabled = true;
                    gameObject.GetComponent<TrailRenderer>().enabled = true;

                    break;
                case false:
                    ApartShipA.SetActive(true);
                    ApartShipB.SetActive(true);

                    LineRendererObj.SetActive(true);
                    gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    gameObject.GetComponent<PolygonCollider2D>().enabled = false;
                    gameObject.GetComponent<TrailRenderer>().enabled = false;
                    break;
            }

            Apart = !Apart;
        }

        if (Apart)
        {
            ApartMovement();
        }
        else
        {
            TogetherMovement();
        }

        var pos = Camera.main.WorldToScreenPoint(transform.position);
        var dir = Input.mousePosition - pos;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle-90f, Vector3.forward);

        var movX = Input.GetAxis("Horizontal");
        var movY = Input.GetAxis("Vertical");

        var mov = new Vector3(movX * Speed * Time.deltaTime, movY * Speed * Time.deltaTime, 0f);

        transform.position += mov;

        SC.volume = (mov.sqrMagnitude * 5 + 0.1f) * 2;
    }
}
