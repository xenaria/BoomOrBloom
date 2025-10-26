using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BombController : MonoBehaviour
{
    public LevelData levelData;
    float fuseTime => levelData.fuseTime;
    float radiusMultiplier => levelData.radiusMultiplier;
    public float explosionRadius;
    

    public LayerMask playerLayer;

    [Header("SFX")]
    public AudioClip timerSfx;
    public AudioClip explodeSfx;

    [Header("Refs")]
    public SpriteRenderer bombSprite;
    public GameObject explosionVfx;

    AudioSource sfx;
    bool armed, exploded;

    void Awake()
    {
        sfx = GetComponent<AudioSource>();
        if (bombSprite) bombSprite.enabled = false;
    }

    void Start()
    {
        explosionRadius = levelData.explosionRadius * radiusMultiplier;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (armed || exploded) return;
        if ((playerLayer.value & (1 << other.gameObject.layer)) == 0) return;

        armed = true;

        if (levelData.gameMode == LevelData.GameMode.Twist)
        {
            StartCoroutine(TwistModeSequenceCoroutine());
        }
        else
        {
            StartCoroutine(NormalModeSequenceCoroutine());
        }
    }

    private IEnumerator NormalModeSequenceCoroutine()
    {
        Debug.Log("Bomb fuse started (Normal mode)");
        yield return StartCoroutine(Fuse());

        exploded = true;

        if (explodeSfx)
        {
            sfx.spatialBlend = 0f;
            sfx.PlayOneShot(explodeSfx);
        }

        if (explosionVfx)
            Instantiate(explosionVfx, transform.position, Quaternion.identity);

        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
       
        PlayerController playerHit = null;
        var hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerLayer);
        foreach (var h in hits)
        {
            PlayerController player = h.GetComponent<PlayerController>();
            if (player != null && player.IsAlive())
            {
                playerHit = player;
                playerHit.DisableMovement();
                Debug.Log("Bomb hit player! Waiting for animation...");
                break;
            }
        }

        float killDelay = explodeSfx ? explodeSfx.length * 0.5f : 0.5f;
        yield return new WaitForSeconds(killDelay);

        if (playerHit != null)
        {
            playerHit.Kill();
            Debug.Log("Player killed after explosion animation.");
        }

        Destroy(gameObject, 0.2f);
    }
    
    private IEnumerator TwistModeSequenceCoroutine()
    {
       Debug.Log("Bomb fuse started (Twist mode)");

        yield return StartCoroutine(Fuse());

        exploded = true;
        if (explodeSfx)
        {
            sfx.spatialBlend = 0f;
            sfx.PlayOneShot(explodeSfx);
        }

        if (explosionVfx)
            Instantiate(explosionVfx, transform.position, Quaternion.identity);

        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;

        var hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var h in hits)
        {
            CherryBlossomController bloom = h.GetComponent<CherryBlossomController>();
            if (bloom != null)
                bloom.CollectByBomb();
        }

        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject);
    }




    


    IEnumerator Fuse()
    {
        if (bombSprite) bombSprite.enabled = true;
        if (timerSfx && sfx) sfx.PlayOneShot(timerSfx);   // countdown cue
        Debug.Log("Bomb is fused!");
        yield return new WaitForSeconds(fuseTime);
    }

    // IEnumerator Explode()
    // {
    //     if (exploded) yield return null;
    //     exploded = true;

    //     if (explodeSfx)
    //     {
    //         sfx.spatialBlend = 0f;    
    //         sfx.PlayOneShot(explodeSfx); 
    //     }

    //     if (explosionVfx)
    //         Instantiate(explosionVfx, transform.position, Quaternion.identity);

    //     GetComponent<Collider2D>().enabled = false;
    //     GetComponent<SpriteRenderer>().enabled = false;

        // if (levelData.gameMode == LevelData.GameMode.Twist)
        // {
        //     var hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        //     foreach (var h in hits)
        //     {
        //         CherryBlossomController bloom = h.GetComponent<CherryBlossomController>();
        //         if (bloom != null)
        //             bloom.CollectWithTwist();
        //     }
        // }
        // else
        // {
        //     PlayerController playerHit = null;
        //     var hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerLayer);
        //     foreach (var h in hits)
        //     {
        //         PlayerController player = h.GetComponent<PlayerController>();
        //         if (player != null && player.IsAlive())
        //         {
        //             playerHit = player;
        //             playerHit.DisableMovement();
        //             Debug.Log("Bomb hit player! Waiting for animation...");
        //             break;
        //         }
        //     }

        //     float delay = explodeSfx ? explodeSfx.length * 0.5f : 0.5f;
        //     yield return new WaitForSeconds(delay);

        //     if (playerHit != null)
        //     {
        //         playerHit.Kill();
        //         Debug.Log("Player killed after explosion animation.");
        //     }
        // }
        
    //     // Destroy(gameObject, 0.2f);
    // }
    
    
    // editor helper
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
