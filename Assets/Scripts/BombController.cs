using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BombController : MonoBehaviour
{
    [Header("Fuse/Explosion")]
    public float fuseTime = 1f;
    public float explosionRadius = 1.2f;
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (armed || exploded) return;
        if ((playerLayer.value & (1 << other.gameObject.layer)) == 0) return;
        StartCoroutine(Fuse());
    }

    IEnumerator Fuse()
    {
        armed = true;
        if (bombSprite) bombSprite.enabled = true;
        if (timerSfx && sfx) sfx.PlayOneShot(timerSfx);   // countdown cue
        Debug.Log("Bomb is fused!");
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    void Explode()
    {
        if (exploded) return;
        exploded = true;

        if (explodeSfx)
        {
            sfx.spatialBlend = 0f;    
            sfx.PlayOneShot(explodeSfx); 
        }

        if (explosionVfx)
            Instantiate(explosionVfx, transform.position, Quaternion.identity);

        var hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerLayer);
        foreach (var h in hits)
            // h.GetComponent<PlayerController>()?.Kill();
            Debug.Log("Player killed!");

        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 0.2f);
    }
    
    
    // editor helper
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
