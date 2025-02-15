using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathZone : MonoBehaviour
{
    [Header("Ayarlar")]
    [SerializeField] private float deathDelay = 1f; // Ölüm sonrası bekleme süresi
    [SerializeField] private ParticleSystem deathEffect; // Opsiyonel ölüm efekti
    
    private bool isDead = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        
        if (other.CompareTag("Player"))
        {
            PlayerDeath(other.gameObject);
        }
    }

    private void PlayerDeath(GameObject player)
    {
        isDead = true;

        // Oyuncunun hareketini durdur
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // Ölüm efektini oynat
        if (deathEffect != null)
        {
            deathEffect.transform.position = player.transform.position;
            deathEffect.Play();
        }

        // Level'i yeniden başlat
        Invoke("RestartLevel", deathDelay);
    }

    private void RestartLevel()
    {
        // Mevcut sahneyi yeniden yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
} 