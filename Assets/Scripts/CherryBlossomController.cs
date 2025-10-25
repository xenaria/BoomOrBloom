using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CherryBlossomController : MonoBehaviour
{
    public LayerMask playerLayer;
    public LevelData levelData;

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

        // check if normal mode!! (else twist HEHEHE)
        if (levelData.gameMode == LevelData.GameMode.Normal)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                GameManager.Instance.OnBloomCollected();
                StartCoroutine(Bloom());
            }
        }
    }
    
    public void CollectWithTwist()
    {
        if (bloomSprite.enabled)
        {
            GameManager.Instance.OnBloomCollected(); 
            StartCoroutine(Bloom());
        }
    }
    
    IEnumerator Bloom()
    {
        bloomSprite.enabled = false;
        GetComponent<Collider2D>().enabled = false;

        if (bloomVfx)
            Instantiate(bloomVfx, transform.position, Quaternion.identity);
        
        if (bloomSfx && sfx)
        sfx.PlayOneShot(bloomSfx);

        yield return new WaitForSeconds(bloomSfx ? bloomSfx.length : 0.5f);

        Destroy(gameObject, 1f);
    }


}
