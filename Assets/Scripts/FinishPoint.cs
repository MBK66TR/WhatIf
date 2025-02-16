using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPoint : MonoBehaviour
{
    [Header("Ayarlar")]
    [SerializeField] private string nextLevel = "Level2"; // Sonraki level'ın sahne adı
    [SerializeField] private float transitionDelay = 1f; // Geçiş için beklenecek süre
    [SerializeField] private ParticleSystem completionEffect; // Opsiyonel parçacık efekti
    
    [Header("Çiçek Ayarları")]
    [SerializeField] private Animator flowerAnimator; // Çiçek animatörü
    
    private bool levelCompleted = false;

    private void Start()
    {
        // Eğer animator component aynı objede ise otomatik al
        if (flowerAnimator == null)
        {
            flowerAnimator = GetComponent<Animator>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (levelCompleted) return;
        
        // Eğer çarpışan obje Player ise
        if (other.CompareTag("Player"))
        {
            LevelCompleted();
        }
    }

    private void LevelCompleted()
    {
        if (levelCompleted) return;
        
        levelCompleted = true;
        
        // Oyuncunun animasyon kontrolcüsünü bul ve finish animasyonunu tetikle
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerAnimationController playerAnim = player.GetComponent<PlayerAnimationController>();
            if (playerAnim != null)
            {
                playerAnim.TriggerFinishAnimation();
                Debug.Log("Trying to trigger finish animation");
            }
        }
        
        // Parçacık efektini oynat (eğer varsa)
        if (completionEffect != null)
        {
            completionEffect.Play();
        }
        
        // Sonraki levele geçiş
        Invoke("NextLevelTransition", transitionDelay);

        SoundManager.Instance.PlaySound("FinishEffect");
    }

    private void NextLevelTransition()
    {
        // Sonraki sahneyi yükle
        SceneManager.LoadScene(nextLevel);
    }
} 