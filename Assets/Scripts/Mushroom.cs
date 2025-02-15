using UnityEngine;
using System.Collections;

public class Mushroom : MonoBehaviour
{
    [Header("Zıplama Ayarları")]
    [SerializeField] private float jumpForce = 15f; // Zıplama kuvveti
    [SerializeField] private float bounceBackForce = 5f; // Yatay geri tepme kuvveti
    [SerializeField] private ParticleSystem bounceEffect; // Zıplama efekti
    
    [Header("Animasyon")]
    [SerializeField] private Animator mushroomAnimator; // Mantar animatörü
    [SerializeField] private float squishAmount = 0.2f; // Sıkışma miktarı
    [SerializeField] private float squishDuration = 0.1f; // Sıkışma süresi
    
    private Vector3 originalScale;
    private bool isSquishing = false;

    private void Start()
    {
        if (mushroomAnimator == null)
        {
            mushroomAnimator = GetComponent<Animator>();
        }
        originalScale = transform.localScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Bounce(collision.gameObject);
            StartSquishAnimation();
        }
    }

    private void Bounce(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float horizontalVelocity = rb.velocity.x;
            rb.velocity = new Vector2(horizontalVelocity, jumpForce);
            
            if (horizontalVelocity != 0)
            {
                float bounceDirection = Mathf.Sign(horizontalVelocity) * -1;
                rb.AddForce(new Vector2(bounceDirection * bounceBackForce, 0), ForceMode2D.Impulse);
            }
            
            if (bounceEffect != null)
            {
                bounceEffect.Play();
            }
        }
    }

    private void StartSquishAnimation()
    {
        if (!isSquishing)
        {
            StartCoroutine(SquishCoroutine());
        }
    }

    private IEnumerator SquishCoroutine()
    {
        isSquishing = true;

        // Sıkışma animasyonu
        Vector3 squishScale = new Vector3(
            originalScale.x * (1 + squishAmount),
            originalScale.y * (1 - squishAmount),
            originalScale.z
        );

        // Yumuşak geçiş için Lerp kullan
        float elapsed = 0;
        while (elapsed < squishDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / squishDuration;
            transform.localScale = Vector3.Lerp(originalScale, squishScale, t);
            yield return null;
        }

        // Geri dönüş animasyonu
        elapsed = 0;
        while (elapsed < squishDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / squishDuration;
            transform.localScale = Vector3.Lerp(squishScale, originalScale, t);
            yield return null;
        }

        transform.localScale = originalScale;
        isSquishing = false;
    }
} 