using System.Collections;
using UnityEngine;

public class CherryBlossomController : MonoBehaviour
{

    public LayerMask playerLayer;

    [Header("SFX")]
    public AudioClip bloomSfx;
    AudioSource sfx;

    [Header("Refs")]
    public SpriteRenderer bloomSprite;
    public GameObject bloomVfx;

    void Awake()
    {
        sfx = GetComponent<AudioSource>();
        bloomSprite = gameObject.GetComponent<SpriteRenderer>();
        if (bloomSprite) bloomSprite.enabled = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((playerLayer.value & (1 << other.gameObject.layer)) == 0) return;
        StartCoroutine(Bloom());
    }
    
    IEnumerator Bloom()
    {
        GameManager.instance.IncreaseScore(1);
        Debug.Log($"Score: {GameManager.instance.gameScore}");
        bloomSprite.enabled = false;

        if (bloomVfx)
            Instantiate(bloomVfx, transform.position, Quaternion.identity);
        
        sfx.PlayOneShot(bloomSfx);
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;

        // wait for the clip to finish
        yield return new WaitForSeconds(bloomSfx.length);
        Destroy(gameObject, 0.2f);
    }


}
